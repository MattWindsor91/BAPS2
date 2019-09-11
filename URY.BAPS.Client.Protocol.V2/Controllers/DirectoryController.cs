using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Client.Protocol.V2.Controllers
{
    /// <summary>
    ///     Controller for directories.
    /// </summary>
    public class DirectoryController : BapsNetControllerBase, IDirectoryController
    {
        private readonly byte _directoryId;

        public DirectoryController(byte directoryId, ClientCore? core) : base(core)
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
        public IDirectoryEventFeed Updater => Core.EventFeed;

        public void Refresh()
        {
            var cmd = new SystemCommand(SystemOp.ListFiles, _directoryId);
            Send(new MessageBuilder(cmd));
        }
    }
}