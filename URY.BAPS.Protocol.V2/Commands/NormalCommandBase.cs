namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base for commands that have a 'normal' layout of
    ///     mode mask and value.
    /// </summary>
    /// <typeparam name="TOp">The enumeration of allowed operations.</typeparam>
    public abstract class NormalCommandBase<TOp> : CommandBase<TOp>
    {
        public bool ModeFlag { get; }

        public byte Value { get; }

        public NormalCommandBase(TOp op, byte value, bool modeFlag) : base(op)
        {
            ModeFlag = modeFlag;
            Value = value;
        }

        public override CommandWord Packed => OpAsCommandWord(Op).WithModeFlag(ModeFlag).WithValue(Value);
    }
}