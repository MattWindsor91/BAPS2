using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.ServerConfig;
using JetBrains.Annotations;

namespace BAPSClientCommon.Controllers
{
    /// <summary>
    ///     Controller for directories.
    /// </summary>
    public class DirectoryController : BapsNetControllerBase
    {
        private readonly ushort _directoryId;
        
        /// <summary>
        ///     An event interface that broadcasts directory server updates.
        ///     <para>
        ///         Note that the updates may include other directories; anything subscribing to this interface
        ///         must check incoming events to see if they affect the right channel.
        ///     </para>
        /// </summary>
        public IDirectoryServerUpdater Updater => Core;
        
        public DirectoryController(ushort directoryId, [CanBeNull] IClientCore core) : base(core)
        {
            _directoryId = directoryId;
        }

        /// <summary>
        ///     Asks the server to refresh this directory's listing.
        /// </summary>
        public void Refresh()
        {
            var cmd = Command.System | Command.ListFiles | (Command) _directoryId;
            SendAsync(new Message(cmd));
        }
    }
}