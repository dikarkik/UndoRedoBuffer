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
                {
                    FlipSet(userEmojis);
                    break;
                }
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
                {
                    FlipSet(userEmojis);
                    break;
                }
        }
    }

    private static void FlipSet(ushort[,] userEmojis)
    {
        int lastIndex = userEmojis.GetLength(0) - 1;
        int middle = lastIndex / 2;
        int emojiSize = ModificationsFactory.MAX_UNICODES;
        for (int a = 0, b = lastIndex; a < middle; a++, b--)
        {
            //
            for (int unicode = 0; unicode < emojiSize; unicode++)
            {
                // Backup A
                ushort backup = userEmojis[a, unicode];

                // Copy B to A
                userEmojis[a, unicode] = userEmojis[b, unicode];

                // Copy backup A to B
                userEmojis[b, unicode] = backup;
            }
        }
    }
}