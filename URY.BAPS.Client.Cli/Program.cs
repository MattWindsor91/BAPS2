using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Autofac;
using Autofac.Core;
using URY.BAPS.Client.Autofac;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Cli
{
    /// <summary>
    ///     A command-line BAPS console.
    /// </summary>
    public static class Program
    {
        public static void Main(string[] args)
        {
            args ??= Array.Empty<string>();
            var clientModule = GetClientModule(args);
            if (clientModule == null) return;
            
            var container = BuildDependencyInjectionContainer(clientModule);
            using var scope = container.BeginLifetimeScope();
            TryGetConsole(scope)?.Run();
        }

        private static ClientModule? GetClientModule(IReadOnlyList<string> args)
        {
            var module = new ClientModule();

            switch (args.Count)
            {
                case 0:
                    break;
                case 1:
                    module.ConfigPath = args[0];
                    break;
                default:
                    System.Console.Error.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} [INI_PATH]");
                    return null;
            }

            return module;
        }

        private static CliClient? TryGetConsole(IComponentContext scope)
        {
            CliClient? console = null;
            try
            {
                console = scope.Resolve<CliClient>();
            }
            catch (DependencyResolutionException exc)
            {
                System.Console.Error.WriteLine("Couldn't set up the components required to start the BAPS client.");
                ShowNestedErrorMessage(exc);
            }

            return console;
        }

        private static void ShowNestedErrorMessage(Exception exc)
        {
            var inner = exc.InnerException;
            while (inner != null)
            {
                 switch (inner)
                 {
                     case ClientConfigException e:
                         System.Console.Error.WriteLine("This is because of a problem getting the BAPS client's configuration.");
                         System.Console.Error.WriteLine($"Reason: {e.Message}");
                         return;
                 }               
                (inner, exc) = (exc.InnerException, inner);
            }
            
            System.Console.Error.WriteLine($"Reason: {exc.Message}");
        }


        private static IContainer BuildDependencyInjectionContainer(ClientModule module)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);
            builder.RegisterType<CliLoginPrompter>().As<ILoginPrompter>().InstancePerLifetimeScope();
            builder.RegisterType<CliLoginErrorHandler>().As<ILoginErrorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CliClient>().AsSelf().InstancePerLifetimeScope();
            var container = builder.Build();
            return container;
        }
    }
}
