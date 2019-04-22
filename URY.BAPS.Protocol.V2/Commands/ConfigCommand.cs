namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Unpacked BapsNet config command without an index.
    /// </summary>
    /// <seealso cref="IndexedConfigCommand"/>
    public class ConfigCommand : ConfigCommandBase
    {
        public ConfigCommand(ConfigOp op) : base(op)
        {
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public override CommandWord Packed =>
            OpAsCommandWord(Op);
    }
}