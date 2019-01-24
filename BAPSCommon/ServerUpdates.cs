namespace BAPSCommon
{
    /// <summary>
    /// Container for event structures and delegates that represent updates
    /// from the BAPS server.
    /// </summary>
    public static class ServerUpdates
    {

        public enum UpDown : byte
        {
            Down = 0,
            Up = 1
        }

        public enum CountType
        {
            LibraryItem,
            Show,
            Listing,
            ConfigOption,
            ConfigChoice,
            User,
            Permission,
            IpRestriction
        }

        public struct CountEventArgs { public CountType Type; public uint Count; public uint Extra; }

        public delegate void CountEventHandler(object sender, CountEventArgs e);

        /// <summary>
        /// Abstract base class of event payloads over config options.
        /// </summary>
        public abstract class ConfigArgs
        {
            /// <summary>The ID of the option to update.</summary>
            public uint OptionId { get; }

            public ConfigArgs(uint optionId)
            {
                OptionId = optionId;
            }
        }

        /// <summary>
        /// Event payload sent when the server declares a config choice.
        /// </summary>
        public class ConfigChoiceArgs : ConfigArgs
        {
            /// <summary>
            /// The ID of the choice to add or update.
            /// </summary>
            public uint ChoiceId { get; }

            /// <summary>
            /// The new description of the choice.
            /// </summary>
            public string ChoiceDescription { get; }

            public ConfigChoiceArgs(uint optionId, uint choiceId, string description)
                : base(optionId)
            {
                ChoiceId = choiceId;
                ChoiceDescription = description;
            }
        }

        public delegate void ConfigChoiceHandler(object sender, ConfigChoiceArgs e);

        /// <summary>
        /// Abstract base class of event payloads over config options that
        /// contain a type and an index.
        /// </summary>
        public abstract class ConfigTypeIndexArgs : ConfigArgs
        {
            /// <summary>The BAPSNET type of the value.</summary>
            public ConfigType Type { get; }

            /// <summary>If present and non-negative, the index of the option to set.</summary>
            public int Index { get; }

            public ConfigTypeIndexArgs(uint optionId, ConfigType type, int index = -1) : base(optionId)
            {
                Type = type;
                Index = index;
            }
        }

        /// <summary>
        /// Event payload sent when the server declares a config setting.
        /// </summary>
        public class ConfigSettingArgs : ConfigTypeIndexArgs
        {
            /// <summary>The new value to apply.</summary>
            public object Value;

            public ConfigSettingArgs(uint optionId, ConfigType type, object value, int index = -1)
                : base(optionId, type, index)
            {
                Value = value;
            }
        }

        public delegate void ConfigSettingHandler(object sender, ConfigSettingArgs e);

        /// <summary>
        /// Event payload sent when the server declares a config option.
        /// </summary>
        public class ConfigOptionArgs : ConfigTypeIndexArgs
        {
            /// <summary>The string description of the option.</summary>
            public string Description { get; }

            /// <summary>Whether the option has an index.</summary>
            public bool HasIndex { get; }

            public ConfigOptionArgs(uint optionId, ConfigType type, string description, bool hasIndex, int index = -1)
                : base(optionId, type, index)
            {
                Description = description;
                HasIndex = hasIndex;
            }
        }

        public delegate void ConfigOptionHandler(object sender, ConfigOptionArgs e);

        /// <summary>
        /// Enumeration of different errors a BAPS server can send.
        /// </summary>
        public enum ErrorType
        {
            Library,
            BapsDb,
            Config
        }

        public struct ErrorEventArgs { public ErrorType Type; public byte Code; public string Description; }

        /// <summary>
        /// Delegate for handling server errors.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The argument struct, containing the error code and description.</param>
        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        #region Channel and playlist item

        public abstract class ChannelEventArgs
        {
            public ushort ChannelId { get; }
            public ChannelEventArgs(ushort channelId)
            {
                ChannelId = channelId;
            }
        }

        /// <summary>
        /// Payload for a channel state (play/pause/stop) server update.
        /// </summary>
        public class ChannelStateEventArgs : ChannelEventArgs
        {
            /// <summary>
            /// The new state of the channel.
            /// </summary>
            public ChannelState State { get; }

            public ChannelStateEventArgs(ushort channelId, ChannelState state) : base(channelId)
            {
                State = state;
            }
        }

        /// <summary>
        /// Event handler for channel state server updates.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The payload of this event.</param>
        public delegate void ChannelStateEventHandler(object sender, ChannelStateEventArgs e);

        /// <summary>
        /// Payload for a channel tracklist reset server update.
        /// </summary>
        public class ChannelResetEventArgs : ChannelEventArgs
        {
            public ChannelResetEventArgs(ushort channelId) : base(channelId) { }
        }
        public delegate void ChannelResetEventHandler(object sender, ChannelResetEventArgs e);

        public abstract class ItemEventArgs : ChannelEventArgs
        {
            public uint Index { get; }
            public ItemEventArgs(ushort channelId, uint index) : base(channelId)
            {
                Index = index;
            }
        }

        /// <summary>
        /// Payload for a tracklist item add server update.
        /// </summary>
        public class ItemAddEventArgs : ItemEventArgs
        {
            public TracklistItem Item { get; }
            public ItemAddEventArgs(ushort channelId, uint index, TracklistItem item)
                : base(channelId, index)
            {
                Item = item;
            }
        }
        public delegate void ItemAddEventHandler(object sender, ItemAddEventArgs e);

        /// <summary>
        /// Payload for a tracklist item move server update.
        /// </summary>
        public class ItemMoveEventArgs : ItemEventArgs
        {
            public uint NewIndex { get; }
            public ItemMoveEventArgs(ushort channelId, uint fromIndex, uint toIndex)
                : base(channelId, fromIndex)
            {
                NewIndex = toIndex;
            }
        }
        public delegate void ItemMoveEventHandler(object sender, ItemMoveEventArgs e);

        /// <summary>
        /// Payload for a tracklist item delete server update.
        /// </summary>
        public class ItemDeleteEventArgs : ItemEventArgs
        {
            public ItemDeleteEventArgs(ushort channelId, uint index) : base(channelId, index) { }
        }
        public delegate void ItemDeleteEventHandler(object sender, ItemDeleteEventArgs e);

        #endregion Channel and playlist item

        #region Directory

        /// <summary>
        /// Base class for all directory update event payloads.
        /// </summary>
        public abstract class DirectoryArgs
        {
            /// <summary>
            /// The ID of the directory this update concerns.
            /// </summary>
            public ushort DirectoryId { get; }

            public DirectoryArgs(ushort directoryId)
            {
                DirectoryId = directoryId;
            }
        }

        /// <summary>
        /// Event payload for when the server adds a file to a directory.
        /// </summary>
        public class DirectoryFileAddArgs : DirectoryArgs
        {
            /// <summary>
            /// The target position of the file in the directory.
            /// </summary>
            public uint Index { get; }

            /// <summary>
            /// The description of the file.
            /// </summary>
            public string Description { get; }

            public DirectoryFileAddArgs(ushort directoryId, uint index, string description)
                : base(directoryId)
            {
                Index = index;
                Description = description;
            }
        }

        public delegate void DirectoryFileAddHandler(object sender, DirectoryFileAddArgs e);

        /// <summary>
        /// Event payload for when the server clears out and renames a directory,
        /// or creates one if it doesn't exist.
        /// </summary>
        public class DirectoryPrepareArgs : DirectoryArgs
        {
            /// <summary>
            /// The new name of the directory.
            /// </summary>
            public string Name { get; }

            public DirectoryPrepareArgs(ushort directoryId, string name)
                : base(directoryId)
            {
                Name = name;
            }
        }

        public delegate void DirectoryPrepareHandler(object sender, DirectoryPrepareArgs e);

        #endregion Directory
    }
}