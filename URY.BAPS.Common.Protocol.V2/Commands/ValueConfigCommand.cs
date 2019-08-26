using System;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     BapsNet config command that contains a value rather than an index.
    ///     <para>
    ///         As the value needs to fit within the same bit-pattern as an
    ///         index, the range of values that these commands can take is
    ///         very limited.
    ///     </para>
    /// </summary>
    public class ValueConfigCommand : ConfigCommand
    {
        public ValueConfigCommand(ConfigOp op, byte value, bool modeFlag) : base(op, modeFlag)
        {
            CheckValueFitsInIndexSpace(value);
            Value = value;
        }

        private static void CheckValueFitsInIndexSpace(byte value)
        {
            if (CommandMasks.ConfigIndex < value)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    $"Value config commands can only contain a value between 0 and {CommandMasks.ConfigIndex}.");
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public byte Value { get; }

        protected override CommandGroup Group => CommandGroup.Config;
        protected override ushort CommandWordFlags => CommandPacking.NormalCommandFlags(ModeFlag, Value);
        protected override byte OpByte => (byte) Op;
    }
}