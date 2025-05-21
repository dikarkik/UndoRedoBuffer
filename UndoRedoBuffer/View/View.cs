using System.Text;

namespace UndoRedoBuffer.View;

public static class Screenview
{
	private struct IndexData()
	{
		public int Index;
		public ConsoleColor color;
		public char[] text;
	}

	public static unsafe void PrintScreen(ushort[,] userEmojis, ModificationsBuffer buffer)
	{
        Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		////////////////////////////////////////////////////////////////////
		// 1. Print buffer
		////////////////////////////////////////////////////////////////////
		
		Console.WriteLine("BUFFER");
	
		// Print first line: The 'current position' index.
		IndexData[] indexesData = new IndexData[]
		{
			new IndexData()
			{
				Index = buffer.CurrentPosition,
				color = ConsoleColor.Yellow,
				text = "┌→ Current Position".ToCharArray(),
			},
		};
		PrintIndexNameAndStickOfOtherIndexes(indexesData, indexesData[0].color);
		
		// Print second line: The 'Newest' index.
		indexesData = new IndexData[]
		{
			new IndexData()
			{
				Index = buffer.CurrentPosition,
				color = ConsoleColor.Yellow,
				text = ['│'],
			},
			new IndexData()
			{
				Index = buffer.NewestModifcation,
				color = ConsoleColor.Green,
				text = "┌→ Newest".ToCharArray(),
			},
		};
		PrintIndexNameAndStickOfOtherIndexes(indexesData, indexesData[1].color);
		
		// Print third line: The 'Oldest' index.
		indexesData = new IndexData[]
		{
			new IndexData()
			{
				Index = buffer.CurrentPosition,
				color = ConsoleColor.Yellow,
				text = ['│'],
			},
			new IndexData()
			{
				Index = buffer.NewestModifcation,
				color = ConsoleColor.Green,
				text = ['│'],
			},
			new IndexData()
			{
				Index = buffer.OldestModification,
				color = ConsoleColor.DarkMagenta,
				text = "┌→ Oldest".ToCharArray(),
			},
		};
		PrintIndexNameAndStickOfOtherIndexes(indexesData, indexesData[2].color);
		
		// Print fourth line: buffer data.
		var totalSlots = buffer.totalSlots;
		for (int i = 0; i < totalSlots; i++)
		{
			var data = buffer.GetDataAtIndex(i);
			switch (data)
			{
				case SetRandomEmojiData:
				{
					var setRandom = (SetRandomEmojiData)data;
					Console.Write($"[ {setRandom.SlotIndex:D2}->");

					int maxCharacters = ModificationsFactory.MAX_UNICODES;
					for (int character = 0; character < maxCharacters; character++)
					{
						if (setRandom.New_unicodes[character] != '\0')
						{
							Console.Write($"{(char)setRandom.New_unicodes[character]}");
						}
					}

					Console.Write(" ]");
					break;
				}
				case ClearSlotData:
				{
					var clearSlot = (ClearSlotData)data;
					Console.Write($"[ {clearSlot.SlotIndex:D2}->✖️  ]");
					break;
				}
				default:
				{
					Console.Write($"[ ------ ]");
					break;
				}
			}
		}
		Console.WriteLine();
		Console.WriteLine();

		// Print user emojis.
		Console.WriteLine("==========================================");
		Console.WriteLine("YOUR EMOJI SET");
		Console.WriteLine("==========================================");

		int indexOfLatestModifiedSlot = buffer.GetUserSlotIndexForCurrentModification();
		
		int rows = userEmojis.GetLength(0);
		int columns = userEmojis.GetLength(1);

		// line 1
		for (int row = 0; row < rows; row++)
		{
			if (row == indexOfLatestModifiedSlot)
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write("+----+   ");
				Console.ResetColor();
			}
			else
			{ 
				Console.Write("+----+   ");
			}
		}
		Console.WriteLine();

		// line 2
		for (int row = 0; row < rows; row++)
		{
			if (row == indexOfLatestModifiedSlot)
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write("¦ ");
				Console.ResetColor();
			}
			else
			{
				Console.Write("¦ ");
			}


			if (userEmojis[row, 0] != '\0')
			{
				for (int column = 0; column < columns; column++)
				{
					if (userEmojis[row, column] != '\0')
					{
						Console.Write($"{(char)userEmojis[row, column]}");
					}
				}
			}
			else
			{
				Console.Write("  ");
			}

			if (row == indexOfLatestModifiedSlot)
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write(" ¦   ");
				Console.ResetColor();
			}
			else
			{
				Console.Write(" ¦   ");
			}
		}
		Console.WriteLine();

		// line 3
		for (int row = 0; row < rows; row++)
		{
			if (row == indexOfLatestModifiedSlot)
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write("+----+   ");
				Console.ResetColor();
			}
			else
			{ 
				Console.Write("+----+   ");
			}
		}
		Console.WriteLine();

		// line 4
		for (int row = 0; row < rows; row++)
		{
			if (row == indexOfLatestModifiedSlot)
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write($"  {row + 1}      ");
				Console.ResetColor();
			}
			else
			{ 
				Console.Write($"  {row + 1}      ");
			}
		}
		Console.WriteLine();
	}
	
	private static void PrintIndexNameAndStickOfOtherIndexes(IndexData[] indexesData, ConsoleColor desiredIndexColor)
	{
		int indexToPrint = indexesData.First(data => data.color == desiredIndexColor).Index;
		var orderedData = indexesData.OrderBy(data => data.Index);
		int totalPrintedChars = 0;

		foreach (var data in orderedData)
		{
			int targetCharPosition = 10 * data.Index;
			int paddingSpaces = (targetCharPosition - totalPrintedChars) + 5;
			
			if (data.color == desiredIndexColor)
			{
				Console.Write(new string(' ', paddingSpaces));
				totalPrintedChars += paddingSpaces;
				Console.ForegroundColor = data.color;
				Console.Write(data.text);
				totalPrintedChars += data.text.Length;
			}
			else if (paddingSpaces > 0 && data.Index != indexToPrint)
			{
				Console.Write(new string(' ', paddingSpaces));
				totalPrintedChars += paddingSpaces;
				Console.ForegroundColor = data.color;
				Console.Write(data.text);
				totalPrintedChars += data.text.Length;
			}
		}
		Console.WriteLine();
		Console.ResetColor();
	}
}