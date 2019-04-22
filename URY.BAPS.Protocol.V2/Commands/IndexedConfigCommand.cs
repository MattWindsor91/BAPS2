namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Unpacked BapsNet config command containing an index.
    /// </summary>
    /// <seealso cref="ConfigCommand"/>
    public class IndexedConfigCommand : ConfigCommandBase
    {
        public byte Index { get; }

        public IndexedConfigCommand(ConfigOp op, byte index) : base(op)
        {
            Index = index;
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public override CommandWord Packed =>
            OpAsCommandWord(Op).WithConfigIndexedFlag(true).WithConfigIndex(Index);
    }
}