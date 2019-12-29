using System.Collections.Generic;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.Common.ServerSelect
{
    /// <summary>
    ///     Interface of objects that represent the selection of a server in the
    ///     configured server pool.
    /// </summary>
    public interface IServerPrompter
    {
        /// <summary>
        ///     The last selected server record.
        /// </summary>
        ServerRecord Selection { get; }
    
        /// <summary>
        ///     Whether the last prompt resulted in the user trying to quit.
        /// </summary>
        bool GaveUp { get; }
        
        /// <summary>
        ///     Prompts for a server record.  If successful, <see cref="Selection"/> contains the selected record;
        ///     otherwise, it is set to an invalid record.
        /// </summary>
        void Prompt();
    }
}