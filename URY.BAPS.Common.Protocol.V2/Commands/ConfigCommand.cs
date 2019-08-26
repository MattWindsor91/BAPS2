using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for config commands.
    ///     <para>
    ///         Config commands can either hold an optional index
    ///         (<see cref="IndexableConfigCommand"/>) or use the
    ///         index bitfield to store a value
    ///         (<see cref="ValueConfigCommand"/>).
    ///     </para>
    /// </summary>
    public abstract class ConfigCommand : NonChannelCommand<ConfigOp>
    {
        protected ConfigCommand(ConfigOp op, bool modeFlag) : base(op, modeFlag)
        {
        }

        protected override CommandGroup Group => CommandGroup.Config;

        protected override byte OpByte => (byte) Op;
    }
}