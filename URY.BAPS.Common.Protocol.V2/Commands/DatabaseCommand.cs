using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class DatabaseCommand : NormalCommandBase<DatabaseOp>
    {
        protected override CommandWord OpAsCommandWord(DatabaseOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public DatabaseCommand(DatabaseOp op, byte value, bool modeFlag) : base(op, value, modeFlag)
        {
        }
    }
}