using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class SystemCommand : NormalCommandBase<SystemOp>
    {
        public SystemCommand(SystemOp op, byte value = 0, bool modeFlag = false) : base(op, value, modeFlag)
        {
        }

        protected override CommandWord OpAsCommandWord(SystemOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }
}