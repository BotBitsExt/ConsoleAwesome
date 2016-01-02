using System;

namespace BotConsole
{
    public class ConsoleMessage
    {
        private readonly string text;
        private readonly ConsoleColor textColor;
        private readonly DateTime time;
        private readonly string title;
        private readonly ConsoleColor titleColor;

        public ConsoleMessage(string text, string title = "", ConsoleColor textColor = ConsoleColor.Gray,
            ConsoleColor titleColor = ConsoleColor.White)
        {
            this.text = text;
            this.title = title;
            this.textColor = textColor;
            this.titleColor = titleColor;

            time = DateTime.Now;
        }

        /// <summary>
        ///     Writes this message to the console.
        /// </summary>
        public void Write()
        {
            ConsoleAwesome.WriteTime(time);

            if (title != "")
            {
                Console.ForegroundColor = titleColor;
                Console.Write(title + " ");
            }
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}