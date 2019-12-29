using System;
using System.Linq.Expressions;
using System.Net.Sockets;

namespace URY.BAPS.Client.Common.ServerSelect
{
    /// <summary>
    ///     An object that coordinates the whole process of selecting a server to connect to.
    /// </summary>
    public class ServerSelector
    {
        private readonly IServerErrorHandler _errorHandler;
        private readonly IServerPrompter _prompter;
        private TcpClient? _server;

        /// <summary>
        ///     Constructs a <see cref="ServerSelector"/>.
        /// </summary>
        /// <param name="errorHandler">A handler for server selection errors.</param>
        /// <param name="prompter">A handler for server selection prompts.</param>
        public ServerSelector(IServerErrorHandler errorHandler, IServerPrompter prompter)
        {
            _errorHandler = errorHandler;
            _prompter = prompter;
        }

        public TcpClient Connection =>
            _server ?? throw new InvalidOperationException(
                "Tried to get a handle to the server before one has been selected.");
        
        public bool HasConnection => _server != null;
        
        public void Run()
        {
            if (HasConnection) return;
            PromptLoop();
            try
            {
                _server = _prompter.Selection.Dial();
            }
            catch (SocketException exc)
            {
                _errorHandler.HandleSocketException(exc);
                _server = null;
            }
        }

        private void PromptLoop()
        {
            do
            {
                _prompter.Prompt();
                if (!_prompter.Selection.IsValid) _errorHandler.HandleInvalidServer(_prompter.Selection);
                if (_prompter.GaveUp) _errorHandler.HandleGivingUp();
            } while (!(_prompter.GaveUp || _prompter.Selection.IsValid));
        }
    }
}