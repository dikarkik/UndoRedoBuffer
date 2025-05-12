using UndoRedoBuffer.View;

namespace UndoRedoBuffer;

internal class Program
{
    static void Main(string[] args)
    {
        //////////////////////////////////////////////////////////////////////////////////
        /// userEmojis: it's the app data
        //////////////////////////////////////////////////////////////////////////////////
        int maxUserEmojis = 5;
        ushort[,] userEmojis = new ushort[maxUserEmojis, ModificationsFactory.MAX_UNICODES];

        //////////////////////////////////////////////////////////////////////////////////
        /// buffer: stores data to make undo/redo possible.
        //////////////////////////////////////////////////////////////////////////////////
        ModificationsBuffer buffer = new();

        while (true)
        {
            Screenview.PrintScreen
            (
                userEmojis: userEmojis,
                buffer: buffer
            );

            // Wait for user input.
            ConsoleKeyInfo input = Console.ReadKey(intercept: true);

            switch (input.KeyChar)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                {
                    // User selected an slot.
                    int selectedSlot = (int)char.GetNumericValue(input.KeyChar) - 1;

                    // Case 1: slot is empty, set a random emoji.
                    if (userEmojis[selectedSlot, 0] == '\0')
                    {
                        SetRandomEmojiData data = ModificationsFactory.GetData_SetRandomEmoji(userEmojis, selectedSlot);
                        ModificationsExecutor.Do(userEmojis, data);
                        buffer.AddModification(data);
                    }
                    // Case 2: slot has emoji, clear it.
                    else
                    {
                        ClearSlotData data = ModificationsFactory.GetData_ClearSlot(userEmojis, selectedSlot);
                        ModificationsExecutor.Do(userEmojis, data);
                        buffer.AddModification(data);
                    }
                    continue;
                }
                case 'f':
                case 'F':
                {              
                    FlipSetData data = ModificationsFactory.GetData_FlipSet();
                    ModificationsExecutor.Do(userEmojis, data);
                    buffer.AddModification(data);
                    continue;
                }
                case 'U':
                case 'u':
                    var undoData = buffer.Undo();
                    ModificationsExecutor.Undo(userEmojis, undoData);
                    continue;
                case 'R':
                case 'r':
                    var redoData = buffer.Redo();
                    ModificationsExecutor.Do(userEmojis, redoData);
                    continue;
                case 'x': // Exit program.
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    continue;
            }
        }
    }
}