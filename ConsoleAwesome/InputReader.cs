using System;

namespace BotConsole
{
    internal static class InputReader
    {
        private const string InputStart = " > ";

        /// <summary>
        ///     Reads the input from the user.
        /// </summary>
        /// <param name="input">The initial input.</param>
        /// <returns></returns>
        public static string ReadInput(string input)
        {
            WriteInput(input);

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return input;

                    case ConsoleKey.Delete:
                        if (Console.CursorLeft == InputStart.Length + input.Length)
                            break;

                        // If input would become empty we exit
                        if (input.Length <= 1)
                        {
                            ClearLine();
                            return "";
                        }

                        input = input.Remove(Console.CursorLeft - InputStart.Length, 1);
                        WriteInput(input, true);
                        break;

                    case ConsoleKey.Backspace:
                        if (Console.CursorLeft == InputStart.Length)
                            break;

                        // If input would become empty we exit
                        if (input.Length <= 1)
                        {
                            ClearLine();
                            return "";
                        }

                        input = input.Remove(Console.CursorLeft - InputStart.Length - 1, 1);
                        WriteInput(input, true);
                        Console.CursorLeft -= 1;
                        break;

                    case ConsoleKey.Escape:
                        ClearLine();
                        return "";

                    case ConsoleKey.LeftArrow:
                        if (Console.CursorLeft > InputStart.Length)
                            Console.CursorLeft -= 1;

                        break;

                    case ConsoleKey.RightArrow:
                        if (Console.CursorLeft < InputStart.Length + input.Length)
                            Console.CursorLeft += 1;

                        break;

                    case ConsoleKey.Home:
                        Console.CursorLeft = InputStart.Length;
                        break;

                    case ConsoleKey.End:
                        Console.CursorLeft = InputStart.Length + input.Length;
                        break;

                    default:
                        if (char.IsControl(key.KeyChar))
                            break;

                        // Fix possible issues when cursor is moved outside of bounds
                        if (Console.CursorLeft < InputStart.Length)
                            Console.CursorLeft = InputStart.Length;
                        if (Console.CursorLeft > InputStart.Length + input.Length)
                            Console.CursorLeft = InputStart.Length + input.Length;

                        if (Console.CursorLeft < InputStart.Length + input.Length)
                        {
                            input = input.Insert(Console.CursorLeft - InputStart.Length, key.KeyChar.ToString());
                            WriteInput(input, true);
                            Console.CursorLeft += 1;
                        }
                        else
                        {
                            input += key.KeyChar;
                            Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///     Clears the line.
        /// </summary>
        public static void ClearLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.BufferWidth - 1));
            Console.CursorLeft = 0;
        }

        /// <summary>
        ///     Writes the currently received input to console.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveCursorPosition">if set to <c>true</c> preserve cursor position.</param>
        private static void WriteInput(string input, bool preserveCursorPosition = false)
        {
            var initialCursorPosition = Console.CursorLeft;

            ClearLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(InputStart);
            Console.ResetColor();

            Console.Write(input);

            if (preserveCursorPosition)
                Console.CursorLeft = initialCursorPosition;
        }
    }
}