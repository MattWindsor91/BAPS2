using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Common.Model.MessageEvents
{
    #region Text

    /// <summary>
    ///     Event structure representing a change in a text setting.
    /// </summary>
    public class TextSettingArgs : MessageArgsBase
    {
        public TextSettingArgs(TextSetting setting, TextSettingDirection direction)
        {
            Setting = setting;
            Direction = direction;
        }

        /// <summary>
        ///     The setting being changed.
        /// </summary>
        public TextSetting Setting { get; }

        /// <summary>
        ///     Whether the setting is moving up or down.
        /// </summary>
        public TextSettingDirection Direction { get; }
    }

    /// <summary>
    ///     Enumeration of text settings that the BAPS protocol understands
    ///     as separate commands.
    /// </summary>
    public enum TextSetting
    {
        /// <summary>
        ///     Request for the font size to be increased or decreased.
        /// </summary>
        FontSize,

        /// <summary>
        ///     Request for the text window to be scrolled up or down.
        /// </summary>
        Scroll
    }

    /// <summary>
    ///     Enumeration of directions in text property changes.
    /// </summary>
    public enum TextSettingDirection : byte
    {
        Down = 0,
        Up = 1
    }

    /// <summary>
    ///     Extension methods for <see cref="TextSettingDirection"/>.
    /// </summary>
    public static class TextSettingDirectionExtensions
    {
        /// <summary>
        ///     Converts a <see cref="TextSettingDirection"/> to a delta by
        ///     adding a magnitude.
        /// </summary>
        /// <param name="direction">The direction being converted.</param>
        /// <param name="magnitude">The magnitude to which the direction is applied.</param>
        /// <returns>
        ///     If <paramref name="direction"/> is down, -1 times <paramref name="magnitude"/>;
        ///     else, <paramref name="magnitude"/>.
        /// </returns>
        public static int ToDelta(this TextSettingDirection direction, int magnitude)
        {
            return direction == TextSettingDirection.Down ? -magnitude : magnitude;
        }
    }

    #endregion Text

    public class ServerVersionArgs : MessageArgsBase
    {
        public ServerVersionArgs(ServerVersion version)
        {
            Version = version;
        }

        public ServerVersion Version { get; }
    }

    public class ServerQuitArgs : MessageArgsBase
    {
        public ServerQuitArgs(bool wasRequested)
        {
            WasRequested = wasRequested;
        }

        public bool WasRequested { get; }
    }
}