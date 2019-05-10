using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Encode;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Controller for directories.
    /// </summary>
    public class DirectoryController : BapsNetControllerBase
    {
        private readonly byte _directoryId;

        public DirectoryController(byte directoryId, [CanBeNull] IClientCore core) : base(core)
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

        /// <summary>
        ///     Asks the server to refresh this directory's listing.
        /// </summary>
        public void Refresh()
        {
            var cmd = new SystemCommand(SystemOp.ListFiles, _directoryId);
            Send(new MessageBuilder(cmd));
        }
    }
}