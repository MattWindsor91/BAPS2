namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Unpacked BapsNet config command whose operation is indexable,
    ///     but doesn't contain an index.
    ///     <para>
    ///         <seealso cref="IndexedConfigCommand" />
    ///     </para>
    /// </summary>
    public class NonIndexedConfigCommand : IndexableConfigCommandBase
    {
        public NonIndexedConfigCommand(ConfigOp op, bool modeFlag) : base(op, modeFlag)
        {
        }

        public override CommandWord Packed =>
            OpAsCommandWord(Op).WithModeFlag(ModeFlag);

        public override bool HasIndex => false;
        public override byte Index => 0 /* Don't rely on this! */;

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }
}