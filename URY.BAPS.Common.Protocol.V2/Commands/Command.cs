using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for BapsNet commands.
    /// </summary>
    /// <typeparam name="TOp">The op enum (eg <see cref="SystemOp" />) that this command type uses.</typeparam>
    public abstract class Command<TOp> : ICommand
    {
        protected Command(TOp op, bool modeFlag)
        {
            Op = op;
            ModeFlag = modeFlag;
        }

        public TOp Op { get; }
        public bool ModeFlag { get; }

        public abstract void Accept(ICommandVisitor? visitor);

        /// <summary>
        ///     Retrieves the 'packed' 16-bit network representation of this command.
        /// </summary>
        public ushort Packed => (ushort) (Group.ToWordBits() | CommandWordOp | CommandWordFlags);

        protected abstract CommandGroup Group { get; }

        protected abstract ushort CommandWordOp { get; }
        protected abstract ushort CommandWordFlags { get; }
    }
}