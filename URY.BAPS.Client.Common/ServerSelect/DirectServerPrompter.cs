using System.Collections.Generic;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.Common.ServerSelect
{
    /// <summary>
    ///     A <see cref="IServerPrompter"/> that wraps a custom <see cref="ServerRecord"/>, and always returns it
    ///     without prompting the user.
    /// </summary>
    public class DirectServerPrompter : IServerPrompter
    {
        public ServerRecord Selection { get; }

        public DirectServerPrompter(ServerRecord selection)
        {
            Selection = selection;
        }

        public bool GaveUp => false;
        
        public void Prompt()
        {
        }
    }
}