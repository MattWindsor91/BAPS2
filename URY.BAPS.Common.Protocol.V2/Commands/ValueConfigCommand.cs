using System;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class ValueConfigCommand : NormalCommandBase<ConfigOp>
    {
        public ValueConfigCommand(ConfigOp op, byte value, bool modeFlag) : base(op, value, modeFlag)
        {
            CheckValueFitsInIndexSpace(value);
        }

        private static void CheckValueFitsInIndexSpace(byte value)
        {
            if (64 <= value)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Value config commands can only contain a value between 0 and 63.");
        }

        protected override CommandWord OpAsCommandWord(ConfigOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}