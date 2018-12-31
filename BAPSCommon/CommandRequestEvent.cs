using System;

namespace BAPSCommon
{
    // NOTE(@MattWindsor91): Wherever this event appears, it is a stop-gap.
    // Ideally, dialogs etc. should NOT send raw BAPSNET commands.
    // This event mainly exists to translate what used to be a direct dependency
    // on a BAPSNET message queue.

    public class CommandRequestEventArgs : EventArgs
    {
        /// <summary>
        /// The BAPSNET message representing the command.
        /// </summary>
        public object message;
    }

    [Obsolete("Try to remove direct dependency on bapsnet", false)]
    public delegate void CommandRequestEventHandler(object sender, CommandRequestEventArgs e);
}
