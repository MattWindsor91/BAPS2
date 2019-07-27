using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for unpacked commands.
    /// </summary>
    /// <typeparam name="TOp">The op enum (eg <see cref="SystemOp" />) that this command type uses.</typeparam>
    public abstract class CommandBase<TOp> : ICommand
    {
        protected CommandBase(TOp op, bool modeFlag)
        {
            Op = op;
            ModeFlag = modeFlag;
        }

        public TOp Op { get; }
        public bool ModeFlag { get; }

        public abstract void Accept(ICommandVisitor? visitor);

        public abstract CommandWord Packed { get; }

        protected abstract CommandWord OpAsCommandWord(TOp op);
    }
}