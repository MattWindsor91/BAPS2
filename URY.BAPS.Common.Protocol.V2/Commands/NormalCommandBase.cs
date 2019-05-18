namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base for commands that have a 'normal' layout of
    ///     mode mask and value.
    /// </summary>
    /// <typeparam name="TOp">The enumeration of allowed operations.</typeparam>
    public abstract class NormalCommandBase<TOp> : CommandBase<TOp>
    {
        protected NormalCommandBase(TOp op, byte value, bool modeFlag) : base(op, modeFlag)
        {
            Value = value;
        }

        public byte Value { get; }

        public override CommandWord Packed => OpAsCommandWord(Op).WithModeFlag(ModeFlag).WithValue(Value);
    }
}