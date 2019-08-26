using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     BapsNet config command whose operation is indexable,
    ///     but doesn't contain an index.
    ///     <para>
    ///         <seealso cref="IndexedConfigCommand" />
    ///     </para>
    /// </summary>
    public class NonIndexedConfigCommand : IndexableConfigCommand
    {
        public NonIndexedConfigCommand(ConfigOp op, bool modeFlag) : base(op, modeFlag)
        {
        }

        protected override ushort CommandWordFlags => ModeFlag ? CommandMasks.ModeFlag : (ushort)0;

        public override bool HasIndex => false;
        public override byte Index => 0 /* Don't rely on this! */;

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }
}