using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using URY.BAPS.Server.Managers;

namespace URY.BAPS.Server
{
    /// <summary>
    ///     The main server object.
    /// </summary>
    public class Server
    {
        private ILogger<Server> Logger { get; }

        private readonly ClientManager _clientManager;
        private readonly ConfigManager _configManager;
        private readonly UserManager _userManager;

        public Server(ILoggerFactory loggerFactory, ClientManager clientManager, ConfigManager configManager, UserManager userManager)
        {
            _clientManager = clientManager;
            _configManager = configManager;
            _userManager = userManager;

            Logger = loggerFactory.CreateLogger<Server>();
        }

        public Task Run()
        {
            Logger.LogInformation("Server starting.");

            var tf = new TaskFactory(_clientManager.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None, TaskScheduler.Default);
            var clientManagerTask = tf.StartNew(_clientManager.Run);

            return Task.WhenAll(clientManagerTask);
        }
    }
}
