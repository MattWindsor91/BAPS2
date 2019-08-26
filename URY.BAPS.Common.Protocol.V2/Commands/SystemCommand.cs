using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class SystemCommand : NormalCommandBase<SystemOp>
    {
        public SystemCommand(SystemOp op, byte value = 0, bool modeFlag = false) : base(op, value, modeFlag)
        {
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        protected override CommandGroup Group => CommandGroup.System;
        protected override byte OpByte => (byte) Op;
    }
}