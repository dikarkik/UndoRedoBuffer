using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UndoRedoBuffer;

public enum ModificationType : byte
{
    None = 0,
    SetRandomEmoji = 1,
    ClearSlot = 2,
    FlipSet = 3,
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SetRandomEmojiData
{
    public ModificationType Type;
    public int SlotIndex;
    public fixed ushort New_unicodes[ModificationsFactory.MAX_UNICODES];
    public fixed ushort Old_unicodes[ModificationsFactory.MAX_UNICODES];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ClearSlotData
{
    public ModificationType Type;
    public int SlotIndex;
    public fixed ushort Cleared_unicodes[ModificationsFactory.MAX_UNICODES];
}

public unsafe struct FlipSetData
{
    public ModificationType Type;
}

public static class ModificationsFactory
{
    public const int MAX_UNICODES = 10;
    
    // Carefully selected emojis which does not add or remove spaces when getting printed in console.
    public static readonly List<string> randomEmojis = new()
    {
        "😚", "🤑", "😴", "😇", "👀", "🍟", "✅", "🚧", "👽", "🎉", "🍋", "🍨"
    };

    private static readonly Random random = new();

    public static unsafe SetRandomEmojiData GetData_SetRandomEmoji(ushort[,] userEmojis, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > userEmojis.GetLength(1))
        {
            Debug.WriteLine("Error: index out of range.");
            slotIndex = userEmojis.GetLength(1) - 1;
        }
        
        SetRandomEmojiData data = new SetRandomEmojiData()
        {
            Type = ModificationType.SetRandomEmoji,
            SlotIndex = slotIndex,
        };
        
        int maxUnicodes = ModificationsFactory.MAX_UNICODES;
        for (int c = 0; c < maxUnicodes; c++)
        {
            data.Old_unicodes[c] = userEmojis[slotIndex, c];
        }
        
        int randomEmojiIndex = random.Next(0, (randomEmojis.Count - 1));
        string randomEmoji = randomEmojis[randomEmojiIndex];
        int totalUnicodes = randomEmoji.Length;
        if (totalUnicodes > maxUnicodes)
        {
            Debug.WriteLine($"Error: random emoji at index {randomEmojiIndex} occupy more than {maxUnicodes} unicode.");
            randomEmoji = "-";
        }
        for (int c = 0; c < maxUnicodes; c++)
        {
            if (c < totalUnicodes)
            { 
                data.New_unicodes[c] = (ushort)randomEmoji[c];
            }
            else
            {
                data.New_unicodes[c] = '\0';
            }
        }
        return data;
    }

    public static unsafe ClearSlotData GetData_ClearSlot(ushort[,] userEmojis, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > userEmojis.GetLength(1))
        {
            Debug.WriteLine("Error: index out of range.");
            slotIndex = userEmojis.GetLength(1) - 1;
        }

        ClearSlotData data = new ClearSlotData()
        {
            Type = ModificationType.ClearSlot,
            SlotIndex = slotIndex,
        };

        int maxUnicodes = ModificationsFactory.MAX_UNICODES;
        for (int c = 0; c < maxUnicodes; c++)
        {
            data.Cleared_unicodes[c] = userEmojis[slotIndex, c];
        }

        return data;
    }

    public static unsafe FlipSetData GetData_FlipSet()
    {
        throw new NotImplementedException();
    }
}