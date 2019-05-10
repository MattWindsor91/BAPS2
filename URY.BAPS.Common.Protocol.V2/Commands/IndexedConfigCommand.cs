namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Unpacked BapsNet config command containing an index.
    /// </summary>
    /// <seealso cref="NonIndexedConfigCommand"/>
    public class IndexedConfigCommand : IndexableConfigCommandBase
    {
        public override bool HasIndex => true;

        public override byte Index { get; }

        public IndexedConfigCommand(ConfigOp op, byte index, bool modeFlag) : base(op, modeFlag)
        {
            Index = index;
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
        
        public override CommandWord Packed =>
            OpAsCommandWord(Op).WithConfigIndexedFlag(true).WithModeFlag(ModeFlag).WithConfigIndex(Index);
    }
}