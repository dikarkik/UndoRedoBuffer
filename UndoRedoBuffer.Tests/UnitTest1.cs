namespace UndoRedoBuffer.Tests;

public class UnitTest1
{
    [Fact]
    public unsafe void Test_SetRandomEmoji()
    {
        int maxUnicodes = ModificationsFactory.MAX_UNICODES;
        ushort[,] userEmojis = new ushort[ModificationsFactory.TOTAL_SET_EMOJIS, maxUnicodes];

        // ===================================================================
        // 1. Test Modifications Factory
        // ===================================================================

        int slotIndex = 3;
        SetRandomEmojiData setEmojiData = ModificationsFactory.GetData_SetRandomEmoji(userEmojis, slotIndex);

        // Verify type.
        Assert.Equal(ModificationType.SetRandomEmoji, setEmojiData.Type);

        // Verify index value.
        Assert.Equal(slotIndex, setEmojiData.SlotIndex);

        // Verify unicodes.
        var newData = GetString(setEmojiData.New_unicodes).TrimEnd('\0');
        Assert.Contains(newData, ModificationsFactory.randomEmojis);

        var oldData = GetString(setEmojiData.Old_unicodes).TrimEnd('\0');
        Assert.Equal("", oldData);

        // ===================================================================
        // 2. Test Modifications Executor
        // ===================================================================

        ModificationsExecutor.Do(userEmojis, setEmojiData);

        for (int i = 0; i < maxUnicodes; i++)
        {
            Assert.Equal(setEmojiData.New_unicodes[i], userEmojis[slotIndex, i]);
        }

        // ===================================================================
        // 3. Test Modifications Buffer 
        // ===================================================================

        // ADD MODIFICATION
        ModificationsBuffer buffer = new();
        buffer.AddModification(setEmojiData);
        Assert.Equal(1, buffer.CurrentPosition);
        Assert.Equal(1, buffer.NewestModifcation);
        Assert.Equal(0, buffer.OldestModification);
        
        Assert.Equal(typeof(SetRandomEmojiData), buffer.GetDataAtIndex(1).GetType());

        var buffer_setEmojiData = (SetRandomEmojiData)buffer.GetDataAtIndex(1);
        Assert.Equal(setEmojiData.SlotIndex, buffer_setEmojiData.SlotIndex);

        for (int i = 0; i < maxUnicodes; i++)
        {
            Assert.Equal(setEmojiData.New_unicodes[i], buffer_setEmojiData.New_unicodes[i]);
        }
        
        for (int i = 0; i < maxUnicodes; i++)
        {
            Assert.Equal(setEmojiData.Old_unicodes[i], buffer_setEmojiData.Old_unicodes[i]);
        }
        
        // UNDO
        buffer.Undo();
        Assert.Equal(0, buffer.CurrentPosition);
        Assert.Equal(1, buffer.NewestModifcation);
        Assert.Equal(0, buffer.OldestModification);

        // REDO
        buffer.Redo();
        Assert.Equal(1, buffer.CurrentPosition);
        Assert.Equal(1, buffer.NewestModifcation);
        Assert.Equal(0, buffer.OldestModification);
    }

    [Fact]
    public unsafe void Test_ClearSlot()
    {
        int maxUnicodes = ModificationsFactory.MAX_UNICODES;
        ushort[,] userEmojis = new ushort[ModificationsFactory.TOTAL_SET_EMOJIS, maxUnicodes];
        int slotIndex = 3;
        SetRandomEmojiData setEmojiData = ModificationsFactory.GetData_SetRandomEmoji(userEmojis, slotIndex);
        ModificationsExecutor.Do(userEmojis, setEmojiData);
        
        // ===================================================================
        // 1. Test Modifications Factory
        // ===================================================================

        ClearSlotData clearSlotData = ModificationsFactory.GetData_ClearSlot(userEmojis, slotIndex);
        
        // 1. Verify type.
        Assert.Equal(ModificationType.ClearSlot, clearSlotData.Type);

        // 1. Verify index value.
        Assert.Equal(slotIndex, clearSlotData.SlotIndex);

        // 2. Verify unicodes.
        for (int i = 0; i < maxUnicodes; i++)
        {
            Assert.Equal(userEmojis[3, i], clearSlotData.Cleared_unicodes[i]);
        }
        
        // ===================================================================
        // 2. Test Modifications Executor
        // ===================================================================

        ModificationsExecutor.Do(userEmojis, clearSlotData);

        for (int i = 0; i < maxUnicodes; i++)
        {
            Assert.Equal('\0', userEmojis[slotIndex, i]);
        }
        
        // ===================================================================
        // 3. Test Modifications Buffer 
        // ===================================================================

        // ADD MODIFICATION
        ModificationsBuffer buffer = new();
        buffer.AddModification(setEmojiData);
        buffer.AddModification(clearSlotData);
        Assert.Equal(2, buffer.CurrentPosition);
        Assert.Equal(2, buffer.NewestModifcation);
        Assert.Equal(0, buffer.OldestModification);
        
        Assert.Equal(typeof(ClearSlotData), buffer.GetDataAtIndex(2).GetType());

        var buffer_clearSlotData = (ClearSlotData)buffer.GetDataAtIndex(2);
        Assert.Equal(slotIndex, buffer_clearSlotData.SlotIndex);

        for (int i = 0; i < maxUnicodes; i++)
        {
            Assert.Equal(clearSlotData.Cleared_unicodes[i], buffer_clearSlotData.Cleared_unicodes[i]);
        }

        // UNDO
        buffer.Undo();
        Assert.Equal(1, buffer.CurrentPosition);
        Assert.Equal(2, buffer.NewestModifcation);
        Assert.Equal(0, buffer.OldestModification);

        // REDO
        buffer.Redo();
        Assert.Equal(2, buffer.CurrentPosition);
        Assert.Equal(2, buffer.NewestModifcation);
        Assert.Equal(0, buffer.OldestModification);
    }

    private unsafe string GetString(ushort* unicodes)
    {
        var maxUnicodes = ModificationsFactory.MAX_UNICODES;
        char[] result = new char[maxUnicodes];
        
        for (int i = 0; i < maxUnicodes; i++)
        {
            result[i] = (char)unicodes[i];
        }
        return new string(result);
    }
}