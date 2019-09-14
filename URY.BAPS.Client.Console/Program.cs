using System;
using System.Reactive.Linq;
using System.Threading;
using Autofac;
using URY.BAPS.Client.Autofac;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Console
{
    /// <summary>
    ///     A command-line BAPS console.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = BuildDependencyInjectionContainer();

            using var scope = container.BeginLifetimeScope();

            var client = scope.Resolve<Protocol.V2.Core.Client>();
            client.Start();

            MainLoop(client);

            client.Stop();
        }

        private static void MainLoop(Protocol.V2.Core.Client client)
        {
            using var cts = new CancellationTokenSource();

            var receiveTask =
                client.EventFeed.ObserveMessages.ForEachAsync(ProcessMessage, cts.Token);

            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q) break;
            }

            cts.Cancel();
            try
            {
                receiveTask.Wait();
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

        private static IContainer BuildDependencyInjectionContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ClientModule>();
            builder.RegisterType<ConsoleLoginPrompter>().As<ILoginPrompter>().InstancePerLifetimeScope();
            builder.RegisterType<ConsoleLoginErrorHandler>().As<ILoginErrorHandler>().InstancePerLifetimeScope();
            var container = builder.Build();
            return container;
        }
    }
}
