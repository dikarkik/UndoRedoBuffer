namespace UndoRedoBuffer;

/// <summary>
/// This class assumes that data was created using 'ModificationFactory'
// by passing the same 'userEmojis' matrix to ensure valid modifications.
/// </summary>
public static class ModificationsExecutor
{
    public static unsafe void Do(ushort[,] userEmojis, object modificationData)
    {
        switch (modificationData)
        {
            case SetRandomEmojiData:
            {
                var data = (SetRandomEmojiData)modificationData;
                int slotIndex = data.SlotIndex;
                var totalUnicodes = ModificationsFactory.MAX_UNICODES;
                for (int i = 0; i < totalUnicodes; i++)
                {
                    userEmojis[slotIndex, i] = data.New_unicodes[i];
                }
                break;
            }
            case ClearSlotData:
            {
                var data = (ClearSlotData)modificationData;
                int slotIndex = data.SlotIndex;
                var totalUnicodes = ModificationsFactory.MAX_UNICODES;
                for (int i = 0; i < totalUnicodes; i++)
                {
                    userEmojis[slotIndex, i] = '\0';
                }
                break;
            }
            case FlipSetData:
                throw new NotImplementedException();
        }
    }
    
    public static unsafe void Undo(ushort[,] userEmojis, object modificationData)
    {
        switch (modificationData)
        {
            case SetRandomEmojiData:
            {
                var data = (SetRandomEmojiData)modificationData;
                int slotIndex = data.SlotIndex;
                var totalUnicodes = ModificationsFactory.MAX_UNICODES;
                for (int i = 0; i < totalUnicodes; i++)
                {
                    userEmojis[slotIndex, i] = data.Old_unicodes[i];
                }
                break;
            }
            case ClearSlotData:
            {
                var data = (ClearSlotData)modificationData;
                int slotIndex = data.SlotIndex;
                var totalUnicodes = ModificationsFactory.MAX_UNICODES;
                 for (int i = 0; i < totalUnicodes; i++)
                {
                    userEmojis[slotIndex, i] = data.Cleared_unicodes[i];
                }
                break;
            }
            case FlipSetData:
                throw new NotImplementedException();
        }
    }
}