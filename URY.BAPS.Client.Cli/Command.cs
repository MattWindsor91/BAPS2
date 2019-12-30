using System;

namespace URY.BAPS.Client.Cli
{
    /// <summary>
    ///     A descriptor of a top-level command in the BAPS CLI.
    /// </summary>
    public class Command
    {
        /// <summary>
        ///     The description of the command, used in the help output.
        /// </summary>
        public string Description { get; }
        
        private readonly Action<CliClient> _payload;

        /// <summary>
        ///     Constructs a command descriptor.
        /// </summary>
        /// <param name="description">The description of the command.</param>
        /// <param name="payload">The action of the command on the client.</param>
        public Command(string description, Action<CliClient> payload)
        {
            Description = description;
            _payload = payload;
        }

        /// <summary>
        ///     Runs the action on a client.
        /// </summary>
        /// <param name="client">The client to which the command should apply.</param>
        public void Run(CliClient client)
        {
            _payload(client);
        }
    }
}