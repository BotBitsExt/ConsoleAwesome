using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BotBits;
using BotBits.Commands;
using BotBits.Events;
using JetBrains.Annotations;

namespace BotConsole
{
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
        ///     The thread which reads input from console.
        /// </summary>
        private static Thread readInputThread;

        /// <summary>
        ///     The custom commands.
        /// </summary>
        private static readonly Dictionary<string, Action<string>> CustomCommands =
            new Dictionary<string, Action<string>>();

        /// <summary>
        ///     Gets or sets the log file.
        /// </summary>
        /// <value>Enables or disables log file.</value>
        public static bool Log { get; set; }

        /// <summary>
        ///     Sets the BotBits client.
        /// </summary>
        /// <param name="c">The client.</param>
        public static void BindClient(BotBitsClient c)
        {
            if (client != null) return;

            client = c;
            EventLoader.Of(c).LoadStatic<ConsoleAwesome>();
        }

        /// <summary>
        ///     Initializes message printing and input reading threads.
        /// </summary>
        /// <param name="title">The title of console.</param>
        public static void Initialize(string title)
        {
            if (writeMessagesThread != null)
                return;

            Console.Title = title;
            Console.CursorVisible = false;

            writeMessagesThread = new Thread(WriteMessages);
            writeMessagesThread.Start();

            readInputThread = new Thread(ReadInput);
            readInputThread.Start();
        }

        /// <summary>
        ///     Adds the custom command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="action">The action.</param>
        public static void AddCustomCommand(string command, Action<string> action)
        {
            command = command.ToLower();
            if (CustomCommands.ContainsKey(command) && !command.Contains(" ")) return;

            CustomCommands.Add(command, action);
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
                        if (paused) break;

                        Messages.Dequeue().Write();
                    }
                }
                else
                {
                    var time = DateTime.Now;
                    if (lastTime.ToString("HH:mm:ss") != time.ToString("HH:mm:ss")) WriteTime(time);

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

                if (finished) return;

                paused = true;
                Console.CursorVisible = true;

                var input = InputReader.ReadInput(key.KeyChar.ToString());
                HandleInput(input);

                Console.CursorVisible = false;
                paused = false;
            }
        }

        /// <summary>
        ///     Handles the input sending it to <see cref="client" />.
        /// </summary>
        /// <param name="input">The input.</param>
        private static void HandleInput(string input)
        {
            Action<string> action;
            if (CustomCommands.TryGetValue(input.Split(' ')[0].ToLower(), out action))
            {
                action(input);
                return;
            }
            if (client == null) return;

            input = input.Trim();
            input = input.StartsWith("/") || input.StartsWith("!") ? input.Substring(1) : "say " + input;

            if (input != "")
            {
                new CommandEvent(new ConsoleInvokeSource(message => new ConsoleMessage(message, "CONSOLE").Write()),
                    new ParsedRequest(input))
                    .RaiseIn(client);
            }
        }

        /// <summary>
        ///     Shuts down all threads.
        /// </summary>
        private static void Exit()
        {
            finished = true;
            paused = false;

            writeMessagesThread.Abort();
            readInputThread.Abort();
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
        
        public static void Write(ConsoleMessage consoleMessage)
        {
            if (Log) File.AppendAllText("LogFile.txt", $"[{consoleMessage.Time}]: {consoleMessage.Title} {consoleMessage.Text} {Environment.NewLine}");

            if (paused) Messages.Enqueue(consoleMessage);
            else consoleMessage.Write();
        }

        /// <summary>
        ///     Writes the specified text to the console or adds it to the queue.
        /// </summary>
        public static void Write(string text, string title = "", ConsoleColor textColor = ConsoleColor.Gray, ConsoleColor titleColor = ConsoleColor.White)
        {
            Write(new ConsoleMessage(text, title, textColor, titleColor));
        }

        /// <summary>
        ///     Writes the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static void WriteError(string error)
        {
            Write(error, textColor: ConsoleColor.Red);
        }

        /// <summary>
        ///     Writes the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void WriteNotification(string message)
        {
            Write(message, textColor: ConsoleColor.DarkCyan);
        }

        #endregion

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
            Write(e.Text, e.Player.ChatName + ":", titleColor: e.Player.Friend ? ConsoleColor.Green : ConsoleColor.White);
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