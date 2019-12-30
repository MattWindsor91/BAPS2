using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Cli
{
    /// <summary>
    ///     Wraps a BAPS client, providing a thin command-line interface over it.
    /// </summary>
    public class CliClient
    {
        public CliClient(Protocol.V2.Core.V2Client client)
        {
            _client = client;
        }
        
        private readonly Protocol.V2.Core.V2Client _client;
        private bool _running;

        /// <summary>
        ///     From a <see cref="Command"/>, shows a listing of this client's commands.
        /// </summary>
        private static void ShowHelp()
        {
            Console.Out.WriteLine("Commands available:");
            Console.Out.WriteLine();
            
            foreach (var (verb, command) in _commands)
            {
                Console.Out.WriteLine($"{verb}: {command.Description}");
            }
        }

        /// <summary>
        ///     From a <see cref="Command"/>, stops the client's main loop.
        /// </summary>
        private void Stop()
        {
            _running = false;
        }

        public void Run()
        {
            _client.Start();
            MainLoop();
            _client.Stop();
        }
        
         private void MainLoop()
         {
             using var cts = new CancellationTokenSource();
 
             var receiveTask =
                 _client.EventFeed.ObserveMessages.ForEachAsync(ProcessMessage, cts.Token);

             _running = true;
             while (_running)
             {
                 var command = ReadLine.Read("BAPS> ");
                 ProcessCommand(command);
             }
 
             cts.Cancel();
             try
             {
                 receiveTask.Wait(CancellationToken.None);
             }
             catch (AggregateException)
             {
                 // TODO(@MattWindsor91): handle?
             }
         }

         private static Dictionary<string, Command> _commands = new Dictionary<string, Command>
         {
             {"help", new Command("Displays this help listing.", c => ShowHelp())},
             {"quit", new Command("Quits the server.", c => c.Stop())}
         };

         private void ProcessCommand(string commandString)
         {
             if (!_commands.TryGetValue(commandString, out var command))
             {
                 Console.Error.WriteLine("Bad command or file name.");
                 return;
             }
             command.Run(this);
         }
 
         private static void ProcessMessage(MessageArgsBase message)
         {
             System.Console.WriteLine(message.ToString());
         }
    }
}