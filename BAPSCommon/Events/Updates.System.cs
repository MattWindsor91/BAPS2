namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        /// <summary>
        ///     Enumeration of directions in text property changes.
        /// </summary>
        public enum UpDown : byte
        {
            Down = 0,
            Up = 1
        }

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

        public class TextSettingEventArgs
        {
            /// <summary>
            ///     The setting being changed.
            /// </summary>
            public TextSetting Setting { get; }
            
            /// <summary>
            ///     Whether the setting is moving up or down.
            /// </summary>
            public UpDown Direction { get; }

            public TextSettingEventArgs(TextSetting setting, UpDown direction)
            {
                Setting = setting;
                Direction = direction;
            }
        }
    }
}