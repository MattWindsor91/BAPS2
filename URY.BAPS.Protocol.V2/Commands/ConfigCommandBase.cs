namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for indexed and non-indexed config commands.
    /// </summary>
    public abstract class ConfigCommandBase : CommandBase<ConfigOp>
    {
        protected ConfigCommandBase(ConfigOp op) : base(op)
        {
        }

        protected override CommandWord OpAsCommandWord(ConfigOp op)
        {
            return op.AsCommandWord();
        }
    }
}