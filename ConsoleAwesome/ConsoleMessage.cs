using System;

namespace BotConsole
{
    public class ConsoleMessage
    {
        public readonly DateTime Time;
        public readonly string Title;
        public readonly string Text;

        private readonly ConsoleColor titleColor;
        private readonly ConsoleColor textColor;

        public ConsoleMessage(string text, string title = "", ConsoleColor textColor = ConsoleColor.Gray, ConsoleColor titleColor = ConsoleColor.White)
        {
            Text = text;
            Title = title;
            this.textColor = textColor;
            this.titleColor = titleColor;

            Time = DateTime.Now;
        }

        /// <summary>
        ///     Writes this message to the console.
        /// </summary>
        public void Write()
        {
            ConsoleAwesome.WriteTime(Time);

            if (Title != "")
            {
                Console.ForegroundColor = titleColor;
                Console.Write(Title + " ");
            }

            Console.ForegroundColor = textColor;
            Console.WriteLine(Text);
            Console.ResetColor();
        }
    }
}