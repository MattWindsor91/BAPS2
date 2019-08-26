namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for commands that aren't channel commands.
    ///     <para>
    ///         This contains both config commands and normal commands.
    ///     </para>
    /// </summary>
    /// <typeparam name="TOp">Type of operation enum.</typeparam>
    public abstract class NonChannelCommand<TOp> : Command<TOp>
    {
        protected NonChannelCommand(TOp op, bool modeFlag) : base(op, modeFlag)
        {
        }

        /// <summary>
        ///     The result of casting <see cref="Command{TOp}.Op"/> to a byte.
        ///     <para>
        ///         Concrete classes must implement this themselves because the
        ///         safety of the cast depends on <typeparamref name="TOp"/>
        ///         being a byte-based enum.
        ///     </para>
        /// </summary>
        protected abstract byte OpByte { get; }

        protected override ushort CommandWordOp => CommandPacking.Op(OpByte);
    }
}