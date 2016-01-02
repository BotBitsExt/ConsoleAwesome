using System;
using System.Collections.Generic;
using System.Threading;
using BotBits;
using BotBits.Commands;
using BotBits.Events;
using JetBrains.Annotations;

namespace BotConsole
{
    [UsedImplicitly]
    public class ConsoleAwesome
    {
        /// <summary>
        ///     The messages queue containing messages that are waiting to be displayed.
        /// </summary>
        private static readonly Queue<ConsoleMessage> Messages = new Queue<ConsoleMessage>();

        /// <summary>
        ///     The last time that current time was printed.
        /// </summary>
        private static DateTime lastTime;

        /// <summary>
        ///     Tells whether the messages printing is currently paused.
        /// </summary>
        private static bool paused;

        /// <summary>
        ///     Tells whether the <see cref="Exit" /> method was called.
        /// </summary>
        private static bool finished;

        /// <summary>
        ///     The client used to execute commands.
        /// </summary>
        private static BotBitsClient client;

        /// <summary>
        ///     The thread which writes time and delayed messages from queue to console.
        /// </summary>
        private static Thread writeMessagesThread;

        /// <summary>
        ///     The thread which read input from console.
        /// </summary>
        private static Thread readInputThread;

        /// <summary>
        ///     Sets the client and initializes <see cref="readInputThread" />.
        /// </summary>
        /// <param name="c">The client.</param>
        [UsedImplicitly]
        public static void BindClient(BotBitsClient c)
        {
            if (readInputThread != null)
                return;

            client = c;
            EventLoader.Of(c).LoadStatic<ConsoleAwesome>();

            readInputThread = new Thread(ReadInput);
            readInputThread.Start();
        }

        /// <summary>
        ///     Initializes <see cref="writeMessagesThread" />.
        /// </summary>
        /// <param name="title">The title of console.</param>
        [UsedImplicitly]
        public static void Initialize(string title)
        {
            if (writeMessagesThread != null)
                return;

            Console.Title = title;
            Console.CursorVisible = false;

            writeMessagesThread = new Thread(WriteMessages);
            writeMessagesThread.Start();
        }

        /// <summary>
        ///     Writes the time and delayed messages to the console.
        /// </summary>
        private static void WriteMessages()
        {
            while (!finished)
            {
                if (paused)
                {
                    Thread.Sleep(5);
                    continue;
                }

                if (Messages.Count > 0)
                {
                    while (Messages.Count > 0)
                    {
                        if (paused)
                            break;

                        Messages.Dequeue().Write();
                    }
                }
                else
                {
                    var time = DateTime.Now;
                    if (lastTime.ToString("HH:mm:ss") != time.ToString("HH:mm:ss"))
                        WriteTime(time);
                    lastTime = time;
                }

                Thread.Sleep(5);
            }
        }

        /// <summary>
        ///     Reads the input from the console.
        /// </summary>
        private static void ReadInput()
        {
            while (!finished)
            {
                if (paused)
                {
                    Thread.Sleep(5);
                    continue;
                }

                var key = Console.ReadKey(true);

                if (finished)
                    return;

                paused = true;
                var input = InputReader.ReadInput(key.KeyChar.ToString());
                Console.CursorVisible = false;
                HandleInput(input);
                paused = false;
            }
        }

        /// <summary>
        ///     Handles the input sending it to <see cref="client" />.
        /// </summary>
        /// <param name="input">The input.</param>
        private static void HandleInput(string input)
        {
            input = input.Trim().ToLower();

            switch (input)
            {
                case "":
                    return;
                case "clear":
                    Console.Clear();
                    return;
            }

            input = input.StartsWith("/") || input.StartsWith("!") ? input.Substring(1) : "say " + input;
            input = input.Trim();

            if (input != "")
            {
                new CommandEvent(new ConsoleInvokeSource(message => new ConsoleMessage(message, "CONSOLE").Write()),
                    new ParsedRequest(input))
                    .RaiseIn(client);
            }
        }

        #region Writing

        /// <summary>
        ///     Prints the time from specified <see cref="time" />.
        /// </summary>
        /// <param name="time">The time.</param>
        public static void WriteTime(DateTime time)
        {
            InputReader.ClearLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(time.ToString("[HH:mm:ss] "));
            Console.ResetColor();
        }

        [UsedImplicitly]
        public static void Write(ConsoleMessage consoleMessage)
        {
            File.AppendAllText("Log.txt", $"[{consoleMessage.Time}]: [{consoleMessage.Type.Humanize(LetterCasing.AllCaps)}]: {consoleMessage.Text}{Environment.NewLine}");

            if (paused) Messages.Enqueue(consoleMessage);
            else consoleMessage.Write();
        }

        /// <summary>
        ///     Writes the specified text to the console or adds it to the queue.
        /// </summary>
        [UsedImplicitly]
        public static void Write(string text, string title = "", ConsoleColor textColor = ConsoleColor.Gray,
            ConsoleColor titleColor = ConsoleColor.White)
        {
            Write(new ConsoleMessage(text, title, textColor, titleColor));
        }

        /// <summary>
        ///     Writes the error.
        /// </summary>
        /// <param name="error">The error.</param>
        [UsedImplicitly]
        public static void WriteError(string error)
        {
            Write(error, textColor: ConsoleColor.Red);
        }

        #endregion

        /// <summary>
        ///     Shuts down all threads.
        /// </summary>
        private static void Exit()
        {
            finished = true;
            paused = false;

            // Final read key to allow reading of messages
            new ConsoleMessage("Press any key to finish.").Write();
            Console.ReadKey();

            writeMessagesThread.Abort();
            readInputThread.Abort();
        }

        #region Event Listeners

        [EventListener]
        private static void On(JoinEvent e)
        {
            Write(e.Username, "[+]", titleColor: ConsoleColor.Green);
        }

        [EventListener]
        private static void On(LeaveEvent e)
        {
            Write(e.Player.Username, "[-]", titleColor: ConsoleColor.Red);
        }

        [EventListener]
        private static void On(ChatEvent e)
        {
            Write(e.Text, e.Player.ChatName + ":");
        }

        [EventListener]
        private static void On(WriteEvent e)
        {
            Write(e.Text, e.Title + ":");
        }

        [EventListener]
        private static void On(InfoEvent e)
        {
            Write(e.Text, e.Title + ":");
        }

        [EventListener]
        private static void On(Info2Event e)
        {
            Write(e.Text, e.Title + ":");
        }

        [EventListener]
        private static void On(DisposingEvent e)
        {
            finished = true;
        }

        [EventListener]
        private static void On(DisposedEvent e)
        {
            Exit();
        }

        #endregion
    }
}
