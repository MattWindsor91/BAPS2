namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for unpacked commands.
    /// </summary>
    /// <typeparam name="TOp">The op enum (eg <see cref="SystemOp"/>) that this command type uses.</typeparam>
    public abstract class CommandBase<TOp> : ICommand
    {
        public TOp Op { get; }

        protected CommandBase(TOp op)
        {
            Op = op;
        }

        protected abstract CommandWord OpAsCommandWord(TOp op);

        public abstract void Accept(ICommandVisitor? visitor);

        public abstract CommandWord Packed { get; }
    }
}