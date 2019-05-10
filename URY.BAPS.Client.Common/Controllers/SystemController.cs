using JetBrains.Annotations;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Controller for general BAPS system requests and updates.
    /// </summary>
    [UsedImplicitly]
    public class SystemController : BapsNetControllerBase
    {
        public SystemController([CanBeNull] IClientCore core) : base(core)
        {
        }

        public ISystemServerUpdater Updater => Core.Updater;

        public void AutoUpdate()
        {
            // Add the auto-update message onto the queue (chat(2) and general(1))
            const byte autoUpdateType = 2 | 1;
            Send(new SystemCommand(SystemOp.AutoUpdate, autoUpdateType));
        }
    }
}