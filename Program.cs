using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class CustomPromptEditor
{
	static void Main(string[] args)
	{
		Console.Clear();
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine("========================================");
		Console.WriteLine("    Ryugu Code editor (Beta)      ");
		Console.WriteLine("========================================");
		Console.ResetColor();

		Console.Write("Enter file path: ");
		string? filePath = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(filePath)) return;

		List<string> lines = new List<string>();
		if (File.Exists(filePath))
		{
			string[] fileLines = File.ReadAllLines(filePath);
			if (fileLines.Length > 0)
			{
				lines.AddRange(fileLines);
			}
			else
			{
				lines.Add("");
			}
		}
		else
		{
			lines.Add("");
		}

		RunEditor(filePath, lines);
	}

	static void RunEditor(string filePath, List<string> lines)
	{
		bool isRunning = true;
		int headerHeight = 2;
		int cursorX = 0;
		int cursorY = 0;
		int scrollOffset = 0;

		Console.CursorVisible = true;

		while (isRunning)
		{
			int windowHeight = Console.WindowHeight;
			int windowWidth = Console.WindowWidth;
			int maxDisplayRows = windowHeight - headerHeight - 2;

			if (cursorY < scrollOffset)
			{
				scrollOffset = cursorY;
			}
			else if (cursorY >= scrollOffset + maxDisplayRows)
			{
				scrollOffset = cursorY - maxDisplayRows + 1;
			}

			Console.SetCursorPosition(0, 0);
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write($"--- Editing: {filePath} ---".PadRight(windowWidth));
			Console.Write(new string('-', windowWidth));
			Console.ResetColor();

			for (int i = 0; i < maxDisplayRows; i++)
			{
				int lineIndex = scrollOffset + i;
				Console.SetCursorPosition(0, headerHeight + i);
				if (lineIndex < lines.Count)
				{
					string lineText = lines[lineIndex];
					if (lineText.Length >= windowWidth)
					{
						Console.Write(lineText.Substring(0, windowWidth - 1));
					}
					else
					{
						Console.Write(lineText.PadRight(windowWidth - 1));
					}
				}
				else
				{
					Console.Write(new string(' ', windowWidth - 1));
				}
			}

			int menuRow = windowHeight - 2;
			Console.SetCursorPosition(0, menuRow);
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Gray;
			string menuText = "Ctrl + X [EXIT]   Ctrl + O [SAVE]   Ctrl + K [Cut]   Ctrl + A -> Delete [All Delete]";
			Console.Write(menuText.PadRight(windowWidth));
			Console.ResetColor();

			string currentLineText = lines[cursorY];
			cursorX = Math.Max(0, Math.Min(cursorX, currentLineText.Length));

			int displayCursorX = cursorX;
			int displayCursorY = headerHeight + (cursorY - scrollOffset);
			Console.SetCursorPosition(displayCursorX, displayCursorY);

			ConsoleKeyInfo keyInfo = Console.ReadKey(true);

			if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.X)
			{
				isRunning = false;
				continue;
			}
			if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.O)
			{
				File.WriteAllLines(filePath, lines);
				continue;
			}
			if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.K)
			{
				lines.Clear();
				lines.Add("");
				cursorX = 0;
				cursorY = 0;
				scrollOffset = 0;
				continue;
			}
			if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.A)
			{
				ConsoleKeyInfo nextKey = Console.ReadKey(true);
				if (nextKey.Key == ConsoleKey.Delete)
				{
					lines.Clear();
					lines.Add("");
					cursorX = 0;
					cursorY = 0;
					scrollOffset = 0;
				}
				continue;
			}

			switch (keyInfo.Key)
			{
				case ConsoleKey.UpArrow:
					if (cursorY > 0)
					{
						cursorY--;
						cursorX = Math.Min(cursorX, lines[cursorY].Length);
					}
					break;

				case ConsoleKey.DownArrow:
					if (cursorY < lines.Count - 1)
					{
						cursorY++;
						cursorX = Math.Min(cursorX, lines[cursorY].Length);
					}
					break;

				case ConsoleKey.LeftArrow:
					if (cursorX > 0)
					{
						cursorX--;
					}
					else if (cursorY > 0)
					{
						cursorY--;
						cursorX = lines[cursorY].Length;
					}
					break;

				case ConsoleKey.RightArrow:
					if (cursorX < lines[cursorY].Length)
					{
						cursorX++;
					}
					else if (cursorY < lines.Count - 1)
					{
						cursorY++;
						cursorX = 0;
					}
					break;

				case ConsoleKey.Enter:
					string currentLine = lines[cursorY];
					string remainingText = currentLine.Substring(cursorX);
					lines[cursorY] = currentLine.Substring(0, cursorX);
					lines.Insert(cursorY + 1, remainingText);
					cursorY++;
					cursorX = 0;
					break;

				case ConsoleKey.Backspace:
					if (cursorX > 0)
					{
						string line = lines[cursorY];
						lines[cursorY] = line.Remove(cursorX - 1, 1);
						cursorX--;
					}
					else if (cursorY > 0)
					{
						string prevLine = lines[cursorY - 1];
						string thisLine = lines[cursorY];
						cursorX = prevLine.Length;
						lines[cursorY - 1] = prevLine + thisLine;
						lines.RemoveAt(cursorY);
						cursorY--;
					}
					break;

				case ConsoleKey.Delete:
					if (cursorX < lines[cursorY].Length)
					{
						string line = lines[cursorY];
						lines[cursorY] = line.Remove(cursorX, 1);
					}
					else if (cursorY < lines.Count - 1)
					{
						string thisLine = lines[cursorY];
						string nextLine = lines[cursorY + 1];
						lines[cursorY] = thisLine + nextLine;
						lines.RemoveAt(cursorY + 1);
					}
					break;

				default:
					if (keyInfo.KeyChar != '\0' && !char.IsControl(keyInfo.KeyChar))
					{
						string line = lines[cursorY];
						lines[cursorY] = line.Insert(cursorX, keyInfo.KeyChar.ToString());
						cursorX++;
					}
					break;
			}
		}

		Console.Clear();
		Console.WriteLine("Editor closed.");
	}
}
