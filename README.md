# ConsoleAwesome ![Version](https://img.shields.io/nuget/v/ConsoleAwesome.svg?label=version) [![Build status](https://ci.appveyor.com/api/projects/status/1hxfce410715c2fl?svg=true)](https://ci.appveyor.com/project/Tunous/consoleawesome)

ConsoleAwesome provides easy access to execution of BotBits commands from the command line and displays various EverybodyEdits messages received through BotBits in an easy to read way.

Thanks to Koya and EEJesse who allowed me to modify and improve their implementation of this idea.

# Installation

To use this library you only have to call 2 methods and everything will be automatically handled for you.

First step is to call `ConsoleAwesome.Initialize("Name of your bot")` which initializes the message handler.
You should call it early in your program to be able to use write methods provided by ConsoleAwesome.

Second step is to bind console to BotBits client with `ConsoleAwesome.BindClient(BotBitsClient)`.
This will handle listening and displaying of chat/info/etc messages and will allow you to execute BotBits commands by simply typing them in the console.

# Commands

When you type something into the console and press enter it will be sent to the bot using say command which simply results in bot saying what you wrote.
If you want to execute other commands you should prefix them with '/' or '!'.
For example typing '/help' would execute the help command.

## Custom commands

If you didn't bind BotBitsClient to the ConsoleAwesome you can add custom commands with `ConsoleAwesome.AddCustomCommand()` method.
It accepts name of the command as first parameter and action to be executed as second.
These commands are then executed when console input starts with specified command name.
Whole input text is provided as an argument to the action so you can parse it to handle additional command arguments.

**NOTE:** You can't add multiple actions for single command name and command names shouldn't contain any spaces.

# Logging

You can enable logging of the messages by setting `ConsoleAwesome.LogFile` parameter.
Once it's set to something different than null or white space all output will be appended to the path specified by `LogFile`.
