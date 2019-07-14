using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;

namespace URY.BAPS.Client.Protocol.V2.Controllers
{
    /// <summary>
    ///     Controller for directories.
    /// </summary>
    public class DirectoryController : BapsNetControllerBase, IDirectoryController
    {
        private readonly byte _directoryId;

        public DirectoryController(byte directoryId, IClientCore? core) : base(core)
        {
            _directoryId = directoryId;
        }

        /// <summary>
        ///     An event interface that broadcasts directory server updates.
        ///     <para>
        ///         Note that the updates may include other directories; anything subscribing to this interface
        ///         must check incoming events to see if they affect the right channel.
        ///     </para>
        /// </summary>
        public IDirectoryServerUpdater Updater => Core.Updater;

        public void Refresh()
        {
            var cmd = new SystemCommand(SystemOp.ListFiles, _directoryId);
            Send(new MessageBuilder(cmd));
        }
    }
}