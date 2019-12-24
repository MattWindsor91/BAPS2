using System;
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
        public CliClient(Protocol.V2.Core.Client client)
        {
            _client = client;
        }
        
        private readonly Protocol.V2.Core.Client _client;
        
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
 
             while (true)
             {
                 var key = System.Console.ReadKey(true);
                 if (key.Key == ConsoleKey.Q) break;
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
 
         private static void ProcessMessage(MessageArgsBase message)
         {
             System.Console.WriteLine(message.ToString());
         }
    }
}