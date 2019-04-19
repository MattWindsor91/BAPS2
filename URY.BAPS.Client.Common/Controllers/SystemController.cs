using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Updaters;

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

        public ISystemServerUpdater Updater => Core;

        public void AutoUpdate()
        {
            // Add the auto-update message onto the queue (chat(2) and general(1))
            const CommandWord cmd = CommandWord.System | CommandWord.AutoUpdate | (CommandWord) 2 | (CommandWord) 1;
            SendAsync(new Message(cmd));
        }
    }
}