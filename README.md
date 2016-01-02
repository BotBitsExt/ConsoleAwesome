# ConsoleAwesome

ConsoleAwesome provieds easy access to execution of BotBits commands from the command line and displays various EverybodyEdits messages received through BotBits in an easy to read way.

Thanks to BBMP and EEJesse who allowed me to modify and improve their implementation of this idea.

# Installation

To use this library you only have to call 2 methods and everything will be automatically handled for you.

First step is to call `ConsoleAwesome.Initialize("Name of your bot")` which initializes the message handler.
You should call it early in your program to be able to use write methods provided by ConsoleAwesome.

Second step is to bind console to BotBits client with `ConsoleAwesome.BindClient(YourBotBitsClient)`.
This will handle listening and displaying of chat/info/etc messages and will allow you to execute commands by simply typing them in the console.

# Commands

When you type something into the console and press enter it will be sent to the bot using say command which simply results in bot saying what you wrote.
If you want to execute other commands you should prefix them with '/' or '!'.
For example typing '/help' would execute the help command.