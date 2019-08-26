using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Unpacked BapsNet config command containing an index.
    /// </summary>
    /// <seealso cref="NonIndexedConfigCommand" />
    public class IndexedConfigCommand : IndexableConfigCommand
    {
        public IndexedConfigCommand(ConfigOp op, byte index, bool modeFlag) : base(op, modeFlag)
        {
            Index = index;
        }

        public override bool HasIndex => true;

        public override byte Index { get; }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        protected override ushort CommandWordFlags
        {
            get
            {
                var word = (ushort)(CommandMasks.ConfigIndexedFlag | CommandUnpacking.ConfigIndex(Index));
                if (ModeFlag) word |= CommandMasks.ModeFlag;
                return word;
            }
        }
    }
}