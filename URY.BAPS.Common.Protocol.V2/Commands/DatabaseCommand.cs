using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class DatabaseCommand : NormalCommandBase<DatabaseOp>
    {
        public DatabaseCommand(DatabaseOp op, byte value, bool modeFlag) : base(op, value, modeFlag)
        {
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        protected override CommandGroup Group => CommandGroup.Database;
        protected override byte OpByte => (byte) Op;
    }
}