using JetBrains.Annotations;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Messages;

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