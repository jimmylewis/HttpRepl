// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HttpRepl.Commands;
using Microsoft.HttpRepl.FileSystem;
using Microsoft.Repl;
using Microsoft.Repl.Commanding;
using Microsoft.Repl.ConsoleHandling;
using Microsoft.Repl.Parsing;

namespace Microsoft.HttpRepl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IFileSystem fileSystem = new RealFileSystem();
            HttpState state = new HttpState(fileSystem);

            if (Console.IsOutputRedirected)
            {
                Reporter.Error.WriteLine("Cannot start the REPL when output is being redirected".SetColor(state.ErrorColor));
                return;
            }

            var dispatcher = DefaultCommandDispatcher.Create(state.GetPrompt, state);
            dispatcher.AddCommand(new ChangeDirectoryCommand());
            dispatcher.AddCommand(new ClearCommand());
            //dispatcher.AddCommand(new ConfigCommand());
            dispatcher.AddCommand(new DeleteCommand(fileSystem));
            dispatcher.AddCommand(new EchoCommand());
            dispatcher.AddCommand(new ExitCommand());
            dispatcher.AddCommand(new HeadCommand(fileSystem));
            dispatcher.AddCommand(new HelpCommand());
            dispatcher.AddCommand(new GetCommand(fileSystem));
            dispatcher.AddCommand(new ListCommand());
            dispatcher.AddCommand(new OptionsCommand(fileSystem));
            dispatcher.AddCommand(new PatchCommand(fileSystem));
            dispatcher.AddCommand(new PrefCommand());
            dispatcher.AddCommand(new PostCommand(fileSystem));
            dispatcher.AddCommand(new PutCommand(fileSystem));
            dispatcher.AddCommand(new RunCommand(fileSystem));
            dispatcher.AddCommand(new SetBaseCommand());
            dispatcher.AddCommand(new SetDiagCommand());
            dispatcher.AddCommand(new SetHeaderCommand());
            dispatcher.AddCommand(new SetSwaggerCommand());
            dispatcher.AddCommand(new UICommand());

            CancellationTokenSource source = new CancellationTokenSource();
            Shell shell = new Shell(dispatcher);
            shell.ShellState.ConsoleManager.AddBreakHandler(() => source.Cancel());
            if (args.Length > 0)
            {
                if (string.Equals(args[0], "--help", StringComparison.OrdinalIgnoreCase) || string.Equals(args[0], "-h", StringComparison.OrdinalIgnoreCase))
                {
                    shell.ShellState.ConsoleManager.WriteLine("Usage: dotnet httprepl [<BASE_ADDRESS>] [options]");
                    shell.ShellState.ConsoleManager.WriteLine();
                    shell.ShellState.ConsoleManager.WriteLine("Arguments:");
                    shell.ShellState.ConsoleManager.WriteLine("  <BASE_ADDRESS> - The initial base address for the REPL.");
                    shell.ShellState.ConsoleManager.WriteLine();
                    shell.ShellState.ConsoleManager.WriteLine("Options:");
                    shell.ShellState.ConsoleManager.WriteLine("  --help - Show help information.");

                    shell.ShellState.ConsoleManager.WriteLine();
                    shell.ShellState.ConsoleManager.WriteLine("REPL Commands:");
                    new HelpCommand().CoreGetHelp(shell.ShellState, (ICommandDispatcher<HttpState, ICoreParseResult>)shell.ShellState.CommandDispatcher, state);
                    return;
                }

                shell.ShellState.CommandDispatcher.OnReady(shell.ShellState);
                shell.ShellState.InputManager.SetInput(shell.ShellState, $"set base \"{args[0]}\"");
                await shell.ShellState.CommandDispatcher.ExecuteCommandAsync(shell.ShellState, CancellationToken.None).ConfigureAwait(false);
            }
            Task result = shell.RunAsync(source.Token);
            await result.ConfigureAwait(false);
        }
    }
}
