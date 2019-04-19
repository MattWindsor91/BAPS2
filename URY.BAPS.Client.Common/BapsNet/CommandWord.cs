using System;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Common.BapsNet
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form the BapsNet flags (see Command's definition).
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of command groups.
    /// </summary>
    public enum CommandGroup : byte
    {
        Playback = 0,
        Playlist = 1,
        Database = 3,
        Config = 5,
        System = 7
    }

    /// <summary>
    ///     Enumeration of playback operations.
    /// </summary>
    public enum PlaybackOp : byte
    {
        Play = 0,
        Stop = 1,
        Pause = 2,
        Position = 3,
        Volume = 4,
        Load = 5,
        CuePosition = 6,
        IntroPosition = 7
    }

    /// <summary>
    ///     Enumeration of playlist operations.
    /// </summary>
    public enum PlaylistOp : byte
    {
        AddItem = 0,
        DeleteItem = 1,
        MoveItemTo = 2,
        Item = 3,
        GetPlaylist = 4,
        ResetPlaylist = 5,
        CopyItem = 6,
    }

    /// <summary>
    ///     Enumeration of database operations.
    /// </summary>
    public enum DatabaseOp : byte
    {
        LibrarySearch = 0,
        LibraryOrdering = 1,
        LibraryResult = 2,
        LibraryError = 3,
        GetShows = 4,
        Show = 5,
        GetListings = 6,
        Listing = 7,
        AssignListing = 8,
        BapsDbError = 9
    }

    /// <summary>
    ///     Enumeration of config operations.
    /// </summary>
    public enum ConfigOp : byte
    {
        GetOptions = 0,
        GetOptionChoices = 1,
        GetConfigSettings = 2,
        GetConfigSetting = 3,
        GetOption = 4,
        SetConfigValue = 5,
        GetUsers = 6,
        GetPermissions = 7,
        GetUser = 8,
        AddUser = 9,
        RemoveUser = 10,
        SetPassword = 11,
        GrantPermission = 12,
        RevokePermission = 13,
        // 14 - 15: unused
        Option = 16,
        OptionChoice = 17,
        ConfigSetting = 18,
        User = 19,
        Permission = 20,
        UserResult = 21,
        ConfigResult = 22,
        ConfigError = 23,
        GetIpRestrictions = 24,
        IpRestriction = 25,
        AlterIpRestriction = 26,
    }

    /// <summary>
    ///     Enumeration of system operations.
    /// </summary>
    public enum SystemOp : byte
    {
        ListFiles = 0,
        Filename = 1,
        SendMessage = 2,
        AutoUpdate = 3,
        End = 4,
        SendLogMessage = 5,
        SetBinaryMode = 6,
        Seed = 7,
        Login = 8,
        LoginResult = 9,
        Version = 10,
        Feedback = 11,
        ClientChange = 12,
        ScrollText = 13,
        TextSize = 14,
        Quit = 15
    }

    public static class CommandShifts
    {
        /// <summary>
        ///     Amount to shift group codes.
        /// </summary>
        public const ushort Group = 13;

        /// <summary>
        ///     Amount to shift playback and playlist operation codes.
        /// </summary>
        public const ushort ChannelOp = 7;

        /// <summary>
        ///     Amount to shift database, config, and system operation codes.
        /// </summary>
        public const ushort Op = 8;
    }

    public static class CommandMasks
    {
        /*
         * There are three different shapes that a command word can take:
         *
         * |GROUP|OPERATION|M|VALUE--------| <-- 'normal'
         * |GROUP|OPERATION--|M|CHANNELID--| <-- 'channel'
         * |GROUP|OPERATION|M|X|INDEX------| <-- 'config'
         * [F|D|E|C|B|A|9|8|7|6|5|4|3|2|1|0]
         *
         * (M = mode flag; X = is-indexed flag).
         */

        /// <summary>
        ///     Mask used, alongside a shift of 13, to select the
        ///     <see cref="CommandGroup"/> part of a command word.
        /// </summary>
        public const ushort Group = 0b11100000_00000000;

        /// <summary>
        ///     Mask used, alongside a shift of 7, to select the
        ///     <see cref="PlaybackOp"/> or
        ///     <see cref="PlaylistOp"/> part of a command word.
        /// </summary>
        public const ushort ChannelOp = 0b00011111_10000000;

        /// <summary>
        ///     Mask used to select the mode flag of a channel command word.
        /// </summary>
        public const ushort ChannelModeFlag = 0b00000000_01000000;

        /// <summary>
        ///     Mask used to select the operation of a non-channel command word.
        /// </summary>
        public const ushort Op = 0b00011111_00000000;

        /// <summary>
        ///     Mask used to select the channel ID of a channel command word.
        /// </summary>
        public const ushort ChannelId = 0b00000000_00111111;

        /// <summary>
        ///     Mask used to select modes on database, config, and system command words.
        /// </summary>
        public const ushort ModeFlag = 0b00000000_10000000;

        /// <summary>
        ///     Mask used to select values on database and system command words.
        /// </summary>
        public const ushort Value = 0b00000000_01111111;

        /// <summary>
        ///     Mask used to select the 'has index' flag on config words.
        /// </summary>
        public const ushort ConfigIndexedFlag = 0b00000000_01000000;

        /// <summary>
        ///     Mask used to select indexes on config command words.
        /// </summary>
        public const ushort ConfigIndex = 0b00000000_00111111;
    }

    /// <summary>
    ///     A packed BapsNet command word.
    ///     <para>
    ///         The various methods and extensions defined on <see cref="CommandWord"/> are
    ///         thin, low-level layers over bit manipulation.  For a more user-friendly
    ///         abstraction, see <see cref="ICommand"/> and its implementations.
    ///     </para>
    /// </summary>
    [Flags]
    public enum CommandWord : ushort
    {
        #region Command groups
        /// <summary>
        ///     The BapsNet command-word flag for the Playback command group.
        /// </summary>
        Playback = CommandGroup.Playback << CommandShifts.Group,

        /// <summary>
        ///     The BapsNet command-word flag for the Playlist command group.
        /// </summary>
        Playlist = CommandGroup.Playlist << CommandShifts.Group,

        /// <summary>
        ///     The BapsNet command-word flag for the Database command group.
        /// </summary>
        Database = CommandGroup.Database << CommandShifts.Group,

        /// <summary>
        ///     The BapsNet command-word flag for the Config command group.
        /// </summary>
        Config = CommandGroup.Config << CommandShifts.Group,

        /// <summary>
        ///     The BapsNet command-word flag for the System command group.
        /// </summary>
        System = CommandGroup.System << CommandShifts.Group,
        #endregion Command groups

        #region Playback commands
        Play = PlaybackOp.Play << CommandShifts.ChannelOp,
        Stop = PlaybackOp.Stop << CommandShifts.ChannelOp,
        Pause = PlaybackOp.Pause << CommandShifts.ChannelOp,
        Position = PlaybackOp.Position << CommandShifts.ChannelOp,
        Volume = PlaybackOp.Volume << CommandShifts.ChannelOp,
        Load = PlaybackOp.Load << CommandShifts.ChannelOp,
        CuePosition = PlaybackOp.CuePosition << CommandShifts.ChannelOp,
        IntroPosition = PlaybackOp.IntroPosition << CommandShifts.ChannelOp,
        #endregion Playback commands

        #region Playlist commands
        AddItem = PlaylistOp.AddItem << CommandShifts.ChannelOp,
        DeleteItem = PlaylistOp.DeleteItem << CommandShifts.ChannelOp,
        MoveItemTo = PlaylistOp.MoveItemTo << CommandShifts.ChannelOp,
        Item = PlaylistOp.Item << CommandShifts.ChannelOp,
        GetPlaylist = PlaylistOp.GetPlaylist << CommandShifts.ChannelOp,
        ResetPlaylist = PlaylistOp.ResetPlaylist << CommandShifts.ChannelOp,
        CopyItem = PlaylistOp.CopyItem << CommandShifts.ChannelOp,
        #endregion Playlist commands

        #region Database commands
        LibrarySearch = DatabaseOp.LibrarySearch << CommandShifts.Op,
        LibraryOrdering = DatabaseOp.LibraryOrdering << CommandShifts.Op,
        LibraryResult = DatabaseOp.LibraryResult << CommandShifts.Op,
        LibraryError = DatabaseOp.LibraryError << CommandShifts.Op,
        GetShows = DatabaseOp.GetShows << CommandShifts.Op,
        Show = DatabaseOp.Show << CommandShifts.Op,
        GetListings = DatabaseOp.GetListings << CommandShifts.Op,
        Listing = DatabaseOp.Listing << CommandShifts.Op,
        AssignListing = DatabaseOp.AssignListing << CommandShifts.Op,
        BapsDbError = DatabaseOp.BapsDbError << CommandShifts.Op,
        #endregion Database commands

        /**
         * Library orderings
        **/
        OrderByArtist = 0,
        OrderByTitle = 1,
        OrderByDateAdded = 2,
        OrderByDateReleased = 3,

        OrderAscending = 0,
        OrderDescending = 1,

        LibraryMaybeDirty = 1,
        LibraryDirty = 2,

        #region Config commands
        GetOptions = ConfigOp.GetOptions << CommandShifts.Op,
        GetOptionChoices = ConfigOp.GetOptionChoices << CommandShifts.Op,
        GetConfigSettings = ConfigOp.GetConfigSettings << CommandShifts.Op,
        GetConfigSetting = ConfigOp.GetConfigSetting << CommandShifts.Op,
        GetOption = ConfigOp.GetOption << CommandShifts.Op,
        SetConfigValue = ConfigOp.SetConfigValue << CommandShifts.Op,
        GetUsers = ConfigOp.GetUsers << CommandShifts.Op,
        GetPermissions = ConfigOp.GetPermissions << CommandShifts.Op,
        GetUser = ConfigOp.GetUser << CommandShifts.Op,
        AddUser = ConfigOp.AddUser << CommandShifts.Op,
        RemoveUser = ConfigOp.RemoveUser << CommandShifts.Op,
        SetPassword = ConfigOp.SetPassword << CommandShifts.Op,
        GrantPermission = ConfigOp.GrantPermission << CommandShifts.Op,
        RevokePermission = ConfigOp.RevokePermission << CommandShifts.Op,
        Option = ConfigOp.Option << CommandShifts.Op,
        OptionChoice = ConfigOp.OptionChoice << CommandShifts.Op,
        ConfigSetting = ConfigOp.ConfigSetting << CommandShifts.Op,
        User = ConfigOp.User << CommandShifts.Op,
        Permission = ConfigOp.Permission << CommandShifts.Op,
        UserResult = ConfigOp.UserResult << CommandShifts.Op,
        ConfigResult = ConfigOp.ConfigResult << CommandShifts.Op,
        ConfigError = ConfigOp.ConfigError << CommandShifts.Op,
        GetIpRestrictions = ConfigOp.GetIpRestrictions << CommandShifts.Op,
        IpRestriction = ConfigOp.IpRestriction << CommandShifts.Op,
        AlterIpRestriction = ConfigOp.AlterIpRestriction << CommandShifts.Op,
        #endregion Config commands

        #region System commands
        ListFiles = SystemOp.ListFiles << CommandShifts.Op,
        Filename = SystemOp.Filename << CommandShifts.Op,
        SendMessage = SystemOp.SendMessage << CommandShifts.Op,
        AutoUpdate = SystemOp.AutoUpdate << CommandShifts.Op,
        End = SystemOp.End << CommandShifts.Op,
        SendLogMessage = SystemOp.SendLogMessage << CommandShifts.Op,
        SetBinaryMode = SystemOp.SetBinaryMode << CommandShifts.Op,
        Seed = SystemOp.Seed << CommandShifts.Op,
        Login = SystemOp.Login << CommandShifts.Op,
        LoginResult = SystemOp.LoginResult << CommandShifts.Op,
        Version = SystemOp.Version << CommandShifts.Op,
        Feedback = SystemOp.Feedback << CommandShifts.Op,
        ClientChange = SystemOp.ClientChange << CommandShifts.Op,
        ScrollText = SystemOp.ScrollText << CommandShifts.Op,
        TextSize = SystemOp.TextSize << CommandShifts.Op,
        Quit = SystemOp.Quit << CommandShifts.Op,
        #endregion System commands
    }

    public static class CommandExtensions
    {
        public static CommandGroup Group(this CommandWord cmd)
        {
            return (CommandGroup) (((ushort)cmd & CommandMasks.Group) >> CommandShifts.Group);
        }

        /// <summary>
        ///     Extracts the operation part of a channel command word.
        /// </summary>
        /// <param name="cmd">The command word to query.</param>
        /// <returns>
        ///     The raw operation code, masked and shifted out of <paramref name="cmd"/>
        ///     using the channel op masks and shifts.
        /// </returns>
        private static byte ChannelOp(CommandWord cmd)
        {
            return (byte)(((ushort)cmd & CommandMasks.ChannelOp) >> CommandShifts.ChannelOp);
        }

        private static CommandWord FromChannelOp(byte op)
        {
            return (CommandWord)((op << CommandShifts.ChannelOp) & CommandMasks.ChannelOp);
        }

        /// <summary>
        ///     Extracts the operation part of a database, config or system command word.
        /// </summary>
        /// <param name="cmd">The command word to query.</param>
        /// <returns>
        ///     The raw operation code, masked and shifted out of <paramref name="cmd"/>
        ///     using the normal masks and shifts.
        /// </returns>
        private static byte Op(CommandWord cmd)
        {
            return (byte)(((ushort)cmd & CommandMasks.Op) >> CommandShifts.Op);
        }

        private static CommandWord FromOp(byte op)
        {
            return (CommandWord) ((op << CommandShifts.Op) & CommandMasks.Op);
        }

        public static PlaybackOp PlaybackOp(this CommandWord cmd)
        {
            var op = ChannelOp(cmd);
            if (!Enum.IsDefined(typeof(PlaybackOp), op))
            {
                throw new ArgumentOutOfRangeException(nameof(cmd), op, "Not a valid playback operation");
            }
            return (PlaybackOp)op;
        }

        public static PlaylistOp PlaylistOp(this CommandWord cmd)
        {
            var op = ChannelOp(cmd);
            if (!Enum.IsDefined(typeof(PlaylistOp), op))
            {
                throw new ArgumentOutOfRangeException(nameof(cmd), op, "Not a valid playlist operation");
            }
            return (PlaylistOp)op;
        }

        public static DatabaseOp DatabaseOp(this CommandWord cmd)
        {
            var op = Op(cmd);
            if (!Enum.IsDefined(typeof(DatabaseOp), op))
            {
                throw new ArgumentOutOfRangeException(nameof(cmd), op, "Not a valid database operation");
            }
            return (DatabaseOp)op;
        }

        public static ConfigOp ConfigOp(this CommandWord cmd)
        {
            var op = Op(cmd);
            if (!Enum.IsDefined(typeof(ConfigOp), op))
            {
                throw new ArgumentOutOfRangeException(nameof(cmd), op, "Not a valid config operation");
            }
            return (ConfigOp)op;
        }

        public static SystemOp SystemOp(this CommandWord cmd)
        {
            var op = Op(cmd);
            if (!Enum.IsDefined(typeof(SystemOp), op))
            {
                throw new ArgumentOutOfRangeException(nameof(cmd), op, "Not a valid system operation");
            }
            return (SystemOp)op;
        }

        #region Building command word bases from operations

        public static CommandWord AsCommandWord(this PlaybackOp op)
        {
            return CommandWord.Playback | FromChannelOp((byte)op);
        }

        public static CommandWord AsCommandWord(this PlaylistOp op)
        {
            return CommandWord.Playlist | FromChannelOp((byte)op);
        }

        public static CommandWord AsCommandWord(this DatabaseOp op)
        {
            return CommandWord.Database | FromOp((byte)op);
        }

        public static CommandWord AsCommandWord(this ConfigOp op)
        {
            return CommandWord.Config | FromOp((byte)op);
        }

        public static CommandWord AsCommandWord(this SystemOp op)
        {
            return CommandWord.System | FromOp((byte)op);
        }

        #endregion Building command word bases from operations

        private static CommandWord SetBit(CommandWord cmd, ushort mask, bool flag)
        {
            return cmd | (CommandWord) (flag ? mask : 0);
        }

        #region Channel mode flags

        public static CommandWord WithChannelModeFlag(this CommandWord cmd, bool flag)
        {
            return SetBit(cmd, CommandMasks.ChannelModeFlag, flag);
        }

        public static bool HasChannelModeFlag(this CommandWord cmd)
        {
            return cmd.HasFlag((CommandWord)CommandMasks.ChannelModeFlag);
        }

        #endregion Channel mode flags

        #region Config is-indexed flags

        public static CommandWord WithConfigIndexedFlag(this CommandWord cmd, bool flag)
        {
            return SetBit(cmd, CommandMasks.ConfigIndexedFlag, flag);
        }

        public static bool HasConfigIndexedFlag(this CommandWord cmd)
        {
            return cmd.HasFlag((CommandWord)CommandMasks.ConfigIndexedFlag);
        }

        #endregion Config is-indexed flags

        #region Normal mode flags

        public static CommandWord WithModeFlag(this CommandWord cmd, bool flag)
        {
            return SetBit(cmd, CommandMasks.ModeFlag, flag);
        }

        /// <summary>
        ///     Tests whether this command word has the (database/config/system) mode flag set.
        /// </summary>
        /// <param name="cmd">The command word to test.</param>
        /// <returns>Whether <paramref name="cmd"/> has the mode flag.</returns>
        public static bool HasModeFlag(this CommandWord cmd)
        {
            return cmd.HasFlag((CommandWord)CommandMasks.ModeFlag);
        }

        #endregion Normal mode flags

        #region Channels
        /// <summary>
        ///     Returns the channel component of a BapsNet playback or playlist command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it actually
        ///         has a channel component---for command words without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to mask-off.</param>
        /// <returns>The channel component of <see cref="cmd"/>.</returns>
        public static byte Channel(this CommandWord cmd)
        {
            return (byte)((ushort)cmd & CommandMasks.ChannelId);
        }

        /// <summary>
        ///     Adds a channel to this command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it can have a channel component,
        ///         nor does it clear any existing channel component (it just does a bitwise OR of the new value).
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to modify.</param>
        /// <param name="channelId">The channel ID to add.</param>
        /// <returns>The command word corresponding to <see cref="cmd" /> with <see cref="channelId" /> marked as the channel by bitwise OR.</returns>
        public static CommandWord WithChannel(this CommandWord cmd, byte channelId)
        {
            return cmd | (CommandWord)channelId;
        }
        #endregion Channels

        #region Values
        /// <summary>
        ///     Returns the value component of a BapsNet database or system command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it actually
        ///         has a value---for command words without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to mask-off.</param>
        /// <returns>The channel component of <see cref="cmd"/>.</returns>
        public static byte Value(this CommandWord cmd)
        {
            return (byte)((ushort)cmd & CommandMasks.Value);
        }

        /// <summary>
        ///     Adds a database or system command value to this command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it can have such a value,
        ///         nor does it clear any existing value (it just does a bitwise OR of the new value).
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to modify.</param>
        /// <param name="value">The channel ID to add.</param>
        /// <returns>The command corresponding to <see cref="cmd" /> with <see cref="value" /> marked as the value by bitwise OR.</returns>
        public static CommandWord WithValue(this CommandWord cmd, byte value)
        {
            return cmd | (CommandWord)value;
        }
        #endregion Values

        #region Config indices
        /// <summary>
        ///     Returns the index component of a BapsNet config command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it actually
        ///         has a index---for command words without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to mask-off.</param>
        /// <returns>The index component of <see cref="cmd"/>.</returns>
        public static byte ConfigIndex(this CommandWord cmd)
        {
            return (byte)((ushort)cmd & CommandMasks.ConfigIndex);
        }

        /// <summary>
        ///     Adds a database or system command value to this command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it can have an index,
        ///         nor does it clear any existing index (it just does a bitwise OR of the new index).
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to modify.</param>
        /// <param name="index">The index to add.</param>
        /// <returns>The command corresponding to <see cref="cmd" /> with <see cref="index" /> marked as the index by bitwise OR.</returns>
        public static CommandWord WithConfigIndex(this CommandWord cmd, byte index)
        {
            return cmd | (CommandWord)index;
        }
        #endregion Config indices
    }
}