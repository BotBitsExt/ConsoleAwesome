using System;

namespace BotConsole
{
    public class ConsoleMessage
    {
        public readonly DateTime Time;
        public readonly string Text;

        private readonly ConsoleColor titleColor;
        private readonly ConsoleColor textColor;
        private readonly string title;
        
        public ConsoleMessage(string text, string title = "", ConsoleColor textColor = ConsoleColor.Gray,ConsoleColor titleColor = ConsoleColor.White)
        {
            this.Text = text;
            this.title = title;
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

            if (title != "")
            {
                Console.ForegroundColor = titleColor;
                Console.Write(title + " ");
            }
            Console.ForegroundColor = textColor;
            Console.WriteLine(Text);
            Console.ResetColor();
        }
    }
}