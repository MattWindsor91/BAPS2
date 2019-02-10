using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Updaters;
using JetBrains.Annotations;

namespace BAPSClientCommon.Controllers
{
    /// <summary>
    ///     Controller for general BAPS system requests and updates.
    /// </summary>
    [UsedImplicitly]
    public class SystemController : BapsNetControllerBase
    {
        public ISystemServerUpdater Updater => Core;
        
        public SystemController([CanBeNull] IClientCore core) : base(core)
        {
        }

        public void AutoUpdate()
        {
            // Add the auto-update message onto the queue (chat(2) and general(1))
            const Command cmd = Command.System | Command.AutoUpdate | (Command) 2 | (Command) 1;
            SendAsync(new Message(cmd));
        }
    }
}