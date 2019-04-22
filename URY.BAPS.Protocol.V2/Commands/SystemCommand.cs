namespace URY.BAPS.Protocol.V2.Commands
{
    public class SystemCommand : NormalCommandBase<SystemOp>
    {
        protected override CommandWord OpAsCommandWord(SystemOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public SystemCommand(SystemOp op, byte value = 0, bool modeFlag = false) : base(op, value, modeFlag)
        {
        }
    }
}