using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Common.Model.MessageEvents
{
    #region No-argument requests

    /// <summary>
    ///     Enumeration of system requests wrapped within <see cref="SystemRequestArgs"/>.
    /// </summary>
    public enum SystemRequest
    {
        GetVersion,
        Quit
    }
    
    /// <summary>
    ///     Represents a zero-argument system request from a client to a server.
    /// </summary>
    public class SystemRequestArgs : MessageArgsBase
    {
        /// <summary>
        ///     The type of request that this message describes.
        /// </summary>
        public SystemRequest Request { get; }

        /// <summary>
        ///     Constructs a system request.
        /// </summary>
        /// <param name="request">The type of request.</param>
        public SystemRequestArgs(SystemRequest request)
        {
            Request = request;
        }
    }
    
    #endregion No-argument requests
    
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

    /// <summary>
    ///     Event structure representing a server version response.
    /// </summary>
    public class ServerVersionArgs : MessageArgsBase
    {
        public ServerVersionArgs(ServerVersion version)
        {
            Version = version;
        }

        public ServerVersion Version { get; }
    }

    /// <summary>
    ///     Event structure representing the fact that the server has quit.
    /// </summary>
    public class ServerQuitArgs : MessageArgsBase
    {
        public ServerQuitArgs(bool wasRequested)
        {
            WasRequested = wasRequested;
        }

        public bool WasRequested { get; }
    }
}