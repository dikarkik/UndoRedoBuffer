using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UndoRedoBuffer;

public class ModificationsBuffer
{
    // Indexes.
    public int OldestModification { get; private set;} = 0;
    public int NewestModifcation { get; private set; } = 0;
    public int CurrentPosition { get; private set; } = 0;

    private IntPtr buffer;
    private int bufferSize; // total bytes.
    private int slotSize = 64;
    public int totalSlots { get; private set; } = 7;

    public ModificationsBuffer()
    {
        bufferSize = slotSize * totalSlots;
        buffer = Marshal.AllocHGlobal(bufferSize);
        
        unsafe
        {
            NativeMemory.Clear(buffer.ToPointer(), (uint)bufferSize);
        }

        // Register Cleanup.
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => FreeBuffer();
        Console.CancelKeyPress += (sender, e) =>
        {
            FreeBuffer();
            e.Cancel = false; // Allow the program to terminate.
        };
    }

    private void FreeBuffer()
    {
        if (buffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(buffer);
            Debug.WriteLine("Memory freed successfully.");
        }
    }

    public void AddModification<T>(T item) where T : struct
    {
        // 1. Make sure the structure size fits the buffer slot size.
        if (Marshal.SizeOf(item) >= slotSize)
        {
            return;
        }
        
        // 2. Update indexes.
        int indexForNewModification = GetNextIndex(CurrentPosition);
        
        CurrentPosition = indexForNewModification;
        NewestModifcation = indexForNewModification;

        if (indexForNewModification == OldestModification)
        {
            // Move the oldest modification index to the next slot.
            OldestModification = GetNextIndex(OldestModification);
        }

        // 3. Write the data.

        // 🔹Calculate the offset position in the buffer.
        IntPtr slotPtr = IntPtr.Add(buffer, CurrentPosition * slotSize);

        // 🔹Clear all bytes of the slot.
        unsafe
        {
            NativeMemory.Clear(slotPtr.ToPointer(), (uint)slotSize);
        }

        // 🔹Write the data in the slot.
        Marshal.StructureToPtr(item, slotPtr, false);
    }

    public object Undo()
    {
        // Case 1: The oldest modification is already undone, cannot undo more stuff.
        if (CurrentPosition == OldestModification)
        {
            return null;
        }

        // Case 2: Can undo whatever happened at 'CurrentPosition' modification.

        // 🔹Get data at 'CurrentPosition' which is what we use to perform the undo.
        object data = GetDataAtIndex(CurrentPosition);

        // 🔹Move 'CurrentPosition' index to an older slot.
        CurrentPosition = GetPreviousIndex(CurrentPosition);

        return data;
    }

    public object Redo()
    {
        // Case 1: The newest change is already redone, cannot redo more stuff.
        if (CurrentPosition == NewestModifcation)
        {
            return null;
        }
        
        // Case 2: Can redo whatever happened after 'CurrentPosition'.

        // 🔹Move 'CurrentPosition' index to a newer slot.
        CurrentPosition = GetNextIndex(CurrentPosition);

        // 🔹Get data at 'CurrentPosition' which is what we need to run the redo.
        return GetDataAtIndex(CurrentPosition);
    }
    
    public object GetDataAtIndex(int index)
    {
        IntPtr slotPtr = IntPtr.Add(buffer, index * slotSize);

        // Read the type identifier.
        ModificationType enumValue = (ModificationType)Marshal.ReadInt32(slotPtr);

        // Retrieve the correct object based on the type.
        switch (enumValue)
        {
            case ModificationType.SetRandomEmoji:
                return Marshal.PtrToStructure<SetRandomEmojiData>(slotPtr);

            case ModificationType.ClearSlot:
                return Marshal.PtrToStructure<ClearSlotData>(slotPtr);

            case ModificationType.FlipSet:
                return Marshal.PtrToStructure<FlipSetData>(slotPtr);
            default:
                return null;
        }
    }

    private int GetNextIndex(int index)
    {
        return index == totalSlots - 1
            ? 0
            : index + 1;
    }
    
    private int GetPreviousIndex(int index)
    {
        return index == 0
            ? totalSlots - 1
            : index - 1;
    }

    public unsafe int GetUserSlotIndexForCurrentModification()
    {
        IntPtr slotPtr = IntPtr.Add(buffer, CurrentPosition * slotSize);

        // Read the type identifier.
        ModificationType enumValue = (ModificationType)Marshal.ReadInt32(slotPtr);

        if (enumValue == ModificationType.SetRandomEmoji || enumValue == ModificationType.ClearSlot)
        {
            return *(int*)((byte*)slotPtr + sizeof(ModificationType));
        }
        else 
        {
            return -1;
        }
    }
}