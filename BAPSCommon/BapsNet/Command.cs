using System;

namespace BAPSClientCommon.BapsNet
{
    public enum CommandGroup : ushort
    {
        Playback = 0,
        Playlist = 1,
        Database = 3,
        Config = 5,
        System = 7
    }

    [Flags]
    public enum Command : ushort
    {
        /**
         * MASKS
         **/
        GroupMask = 0b11100000_00000000, // 14th bit onwards, hence shift is 13
        PlaybackOpMask = 0b00011111_10000000,
        PlaybackModeMask = 0b00000000_01000000,
        PlaybackChannelMask = 0b00000000_00111111,
        PlaylistOpMask = 0b00011111_10000000,
        PlaylistModeMask = 0b00000000_01000000,
        PlaylistChannelMask = 0b00000000_00111111,
        TextOpMask = 0b00011100_00000000,
        DatabaseOpMask = 0b00011111_00000000,
        DatabaseModeMask = 0b00000000_10000000,
        DatabaseValueMask = 0b00000000_01111111,
        ConfigOpMask = 0b00011111_00000000,
        ConfigModeMask = 0b00000000_10000000,
        ConfigUseValueMask = 0b00000000_01000000,
        ConfigValueMask = 0b00000000_00111111,
        SystemOpMask = 0b00011111_00000000,
        SystemModeMask = 0b00000000_10000000,
        SystemValueMask = 0b00000000_01111111,

        /**
         * Operation categories
         **/
        Playback = CommandGroup.Playback << 13,
        Playlist = CommandGroup.Playlist << 13,
        Database = CommandGroup.Database << 13,
        Config = CommandGroup.Config << 13,
        System = CommandGroup.System << 13,

        /**
         * Playback
         **/
        Play = 0 << 7, //C-
        Stop = 1 << 7, //C-
        Pause = 2 << 7, //C-
        Position = 3 << 7, //SC-[0](set) u32int timeposition

        //C -[1](get)
        Volume = 4 << 7, //SC-[0](set) float level

        //C -[1](get)
        Load = 5 << 7, //SC-[0](set) u32int playlistIndex

        //C -[1](get)
        CuePosition = 6 << 7, //SC-[0](set) u32int cueposition

        //C -[1](get)
        IntroPosition = 7 << 7, //SC-[0](set) u32int introposition

        //C -[1](get)

        /**
         * Playlist
         **/
        AddItem = 0 << 7, //C-u32int itemtype [VOID]

        //					[FILE]		u32int directory number, string filename
        //					[LIBRARY]	u32int searchItemIndex
        //					[TEXT]		string briefdescription, longstring details
        DeleteItem = 1 << 7, //SC-u32int index

        MoveItemTo = 2 << 7, //C-u32int oldIndex, u32int newIndex
        Item = 3 << 7, //S-[0](count)	u32int count

        //  [1](data)	u32int index, string name
        //C-[1](get)	u32int index
        GetPlaylist = 4 << 7, //C-none

        ResetPlaylist = 5 << 7, //SC-none
        CopyItem = 6 << 7, //C-u32int fromindex, u32int tochannel

        /** PLAYLIST ITEMS **/
        VoidItem = 0,
        FileItem = 1,
        LibraryItem = 2,
        TextItem = 3,
        DirectLibraryItem = 4, // mattbw 2013-11-18; needed for direct library addition

        /**
         * Database
         **/
        LibrarySearch = 0 << 8, //C  string artist, string title			MUSICLIBRESULT | LIBRARYERROR
        LibraryOrdering = 1 << 8, //C  [VALUE-maybedirty] u32int orderingfield, u32int reverseorder?
        LibraryResult = 2 << 8, //S  [0](count) u32int count

        //   [1](data)[VALUE-maybedirty] u32int index, string description
        LibraryError = 3 << 8, //S  [-][VALUE-errorCode] string description

        GetShows = 4 << 8, //C  [-][VALUE-0]		{current user's shows)		SHOW

        //   [-][VALUE-1]		{system shows}				SHOW
        //   [-][VALUE-2]		string username				SHOW
        Show = 5 << 8, //S  [0](count)	u32int count

        //   [1](data)	u32int showid, string description
        GetListings = 6 << 8, //C				u32int showid

        Listing = 7 << 8, //S  [0](count) u32int count

        //   [1](data)  u32int listingid, u32int channel, string description
        AssignListing = 8 << 8, //C  [-][channel] u32int listingid

        BapsDbError = 9 << 8, //S  [-][VALUE-errorCode] string description

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

        /**
         * Config
         **/
        GetOptions = 0 << 8, //(no args)			OPTION (count-data)
        GetOptionChoices = 1 << 8, //u32int optionid 	OPTIONCHOICE (count-data)
        GetConfigSettings = 2 << 8, //(no args)			CONFIGSETTING (count-data)
        GetConfigSetting = 3 << 8, //u32int optionid	CONFIGSETTING
        GetOption = 4 << 8, //string optionName	OPTION CONFIGSETTING  (COUNT-DATA)

        SetConfigValue =
            5 << 8, //[x][0/1](uses index)[6bit index]	u32int optionid, u32int type, [string value | u32int value] CONFIGRESULT
        GetUsers = 6 << 8, //(no args)			USER (count-data)
        GetPermissions = 7 << 8, //(no args)			PERMISSION (count-data)
        GetUser = 8 << 8, //string username	USER

        AddUser = 9 << 8, //string username, string password		USERRESULT
        RemoveUser = 10 << 8, //string username						USERRESULT
        SetPassword = 11 << 8, //string username, string password		USERRESULT
        GrantPermission = 12 << 8, //string username, u32int permission	USERRESULT
        RevokePermission = 13 << 8, //string username, u32int permission	USERRESULT

        Option = 16 << 8, // [0](count)		u32int count

        // [1](data)[0/1](uses index)[6bit index]	u32int optionid, string optionDesc, u32int type
        OptionChoice = 17 << 8, // [0](count)		u32int count

        // [1](data)		u32int optionid, u32int optionValueid, string optionValueName
        ConfigSetting = 18 << 8, // [0](count)		u32int count

        // [1](data)[0/1](uses index)[6bit index]	u32int optionid, u32int type, [u32int value | string value]
        User = 19 << 8, // [0](count)		u32int count

        // [1](data)		string username, u32int permission
        Permission = 20 << 8, // [0](count)		u32int count

        // [1](data)		u32int permission, string name
        UserResult = 21 << 8, // [RESULTVALUE] string resultText

        ConfigResult = 22 << 8, // [x][0/1](uses index)[6bit index] u32int optionid, u32int result
        ConfigError = 23 << 8, // [x][x][6bit errorcode]					string description
        GetIpRestrictions = 24 << 8, // (no args)		IPRESTRICTION count/data *2(allow deny lists)
        IpRestriction = 25 << 8, // [0](count) [0/1](allow/deny) u32int count

        // [1](data)  [0/1](allow/deny)	string ipaddress, u32int mask
        AlterIpRestriction = 26 << 8, // [0/1](add/remove) [0/1](allow/deny) string ipaddress, u32int mask

        /**
         * System
         **/
        ListFiles = 0 << 8, //C-[x][VALUE-dirnumber]
        Filename = 1 << 8, //S-[0](count)[VALUE-dirnumber] u32int count, string niceDirectoryName

        //	[1](data) [VALUE-dirnumber] u32int index, string filename
        SendMessage = 2 << 8, //C-	u32int clientid, string message

        AutoUpdate = 3 << 8, //C-[VALUE-1](on)

        //  [VALUE-X](off)
        End = 4 << 8, //CS-	string reason

        SendLogMessage = 5 << 8, //S -	string description
        SetBinaryMode = 6 << 8, //C-
        Seed = 7 << 8, //S- string encryptionSeed
        Login = 8 << 8, //C- string username, string encryptedpass
        LoginResult = 9 << 8, //S- [VALUE-resultCode] string resultText
        Version = 10 << 8, //C-

        //S- string date, string time, string version, string author
        Feedback = 11 << 8, //C- string message

        //S- [VALUE-0/1] success/fail
        ClientChange = 12 << 8, //S- [VALUE-0] string clientToRemove

        //   [VALUE-1] string clientToAdd
        ScrollText = 13 << 8, //S- [VALUE-0] scroll down

        //   [VALUE-1] scroll up
        TextSize = 14 << 8, //S- [VALUE-0] text smaller

        //	 [VALUE-1] text bigger
        Quit = 15 << 8 //(no args)
    }

    public static class CommandExtensions
    {
        public static CommandGroup Group(this Command cmd)
        {
            return (CommandGroup) ((uint) (cmd & Command.GroupMask) >> 13);
        }

        /// <summary>
        ///     Returns the channel component of a BAPSnet command.
        ///     <para>
        ///         This doesn't perform any validation on the command to see whether it actually
        ///         has a channel component---for commands without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command to mask-off.</param>
        /// <returns>The channel component</returns>
        public static ushort Channel(this Command cmd)
        {
            return (ushort) (cmd & Command.PlaybackChannelMask);
        }

        /// <summary>
        ///     Adds a channel to this command.
        ///     <para>
        ///         This doesn't perform any validation on the command to see whether it can have a channel component.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command to put a channel on.</param>
        /// <param name="channelId">The channel ID to add.</param>
        /// <returns>The command corresponding to <see cref="cmd" /> with <see cref="channelId" /> marked as the channel.</returns>
        public static Command WithChannel(this Command cmd, ushort channelId)
        {
            return cmd | (Command) channelId;
        }

        public static byte DatabaseValue(this Command cmd)
        {
            return (byte) (cmd & Command.DatabaseValueMask);
        }

        public static byte ConfigValue(this Command cmd)
        {
            return (byte) (cmd & Command.ConfigValueMask);
        }

        public static byte SystemValue(this Command cmd)
        {
            return (byte) (cmd & Command.SystemValueMask);
        }
    }
}