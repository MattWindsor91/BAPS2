using System;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Client.Protocol.V2.Login
{
    /// <summary>
    ///     Extension methods useful for creating the various bits of the V2 login process.
    /// </summary>
    public static class LoginExtensions
    {
        public static (bool matched, ushort command, string? payload) ReceiveSystemStringCommand(
            this IPrimitiveSource src, SystemOp expectedOp)
        {
            // ArgumentNullException need a parameter name, but `src` isn't technically a parameter, and code analysis
            // fails if we try to pass `nameof(src)` in.  Hmm.
            if (src is null) throw new ArgumentException("Source on which this method was invoked is null.");
            
            var cmd = src.ReceiveCommand();
            _ = src.ReceiveUint(); // Discard length
            var isRightGroup = CommandUnpacking.Group(cmd) == CommandGroup.System;
            var isRightOp = CommandUnpacking.SystemOp(cmd) == expectedOp;
            var isRightCommand = isRightGroup && isRightOp;
            return isRightCommand ? (true, cmd, src.ReceiveString()) : (false, default, null);
        }
    }
}