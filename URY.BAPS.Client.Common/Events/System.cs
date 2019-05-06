using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Events
{
    #region Text

    /// <summary>
    ///     Event structure representing a change in a text setting.
    /// </summary>
    public class TextSettingArgs : ArgsBase
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
    
    #endregion Text

    public class ServerVersionArgs : ArgsBase
    {
        public ServerVersion Version { get; }

        public ServerVersionArgs(ServerVersion version)
        {
            Version = version;
        }
    }
    
    public class ServerQuitArgs : ArgsBase
    {
        public bool WasRequested { get; }

        public ServerQuitArgs(bool wasRequested)
        {
            WasRequested = wasRequested;
        }
    }
}