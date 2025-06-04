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

        // 1. Verify type.
        Assert.Equal(ModificationType.SetRandomEmoji, setEmojiData.Type);

        // 1. Verify index value.
        Assert.Equal(slotIndex, setEmojiData.SlotIndex);

        // 2. Verify unicodes.
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