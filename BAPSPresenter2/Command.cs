namespace BAPSPresenter2
{
    internal enum Command : ushort
    {
        /**
         * MASKS
         **/
        GROUPMASK = 0xE000,
        PLAYBACK_OPMASK = 0x1F80,
        PLAYBACK_MODEMASK = 0x40,
        PLAYBACK_CHANNELMASK = 0x3F,
        PLAYLIST_OPMASK = 0x1F80,
        PLAYLIST_MODEMASK = 0x40,
        PLAYLIST_CHANNELMASK = 0x3F,
        TEXT_OPMASK = 0x1C00,
        DATABASE_OPMASK = 0x1F00,
        DATABASE_MODEMASK = 0x80,
        DATABASE_VALUEMASK = 0x7F,
        CONFIG_OPMASK = 0x1F00,
        CONFIG_MODEMASK = 0x80,
        CONFIG_USEVALUEMASK = 0x40,
        CONFIG_VALUEMASK = 0x3F,
        SYSTEM_OPMASK = 0x1F00,
        SYSTEM_MODEMASK = 0x80,
        SYSTEM_VALUEMASK = 0x7F,

        /**
         * Operation categories
         **/
        PLAYBACK = (0 << 13),
        PLAYLIST = (1 << 13),
        BTEXT = (2 << 13),
        DATABASE = (3 << 13),
        CONFIG = (5 << 13),
        SYSTEM = (7 << 13),

        /**
         * Playback
         **/
        PLAY = (0 << 7),    //C-
        STOP = (1 << 7),    //C-
        PAUSE = (2 << 7),   //C-
        POSITION = (3 << 7),  //SC-[0](set) u32int timeposition

        //C -[1](get)
        VOLUME = (4 << 7),  //SC-[0](set) float level

        //C -[1](get)
        LOAD = (5 << 7),  //SC-[0](set) u32int playlistIndex

        //C -[1](get)
        CUEPOSITION = (6 << 7),  //SC-[0](set) u32int cueposition

        //C -[1](get)
        INTROPOSITION = (7 << 7),  //SC-[0](set) u32int introposition

        //C -[1](get)

        /**
         * Playlist
         **/
        ADDITEM = (0 << 7),  //C-u32int itemtype [VOID]

        //					[FILE]		u32int directory number, string filename
        //					[LIBRARY]	u32int searchItemIndex
        //					[TEXT]		string briefdescription, longstring details
        DELETEITEM = (1 << 7),  //SC-u32int index

        MOVEITEMTO = (2 << 7),  //C-u32int oldIndex, u32int newIndex
        ITEM = (3 << 7),    //S-[0](count)	u32int count

        //  [1](data)	u32int index, string name
        //C-[1](get)	u32int index
        GETPLAYLIST = (4 << 7), //C-none

        RESETPLAYLIST = (5 << 7),  //SC-none
        COPYITEM = (6 << 7),  //C-u32int fromindex, u32int tochannel

        /** PLAYLIST ITEMS **/
        VOIDITEM = 0,
        FILEITEM = 1,
        LIBRARYITEM = 2,
        TEXTITEM = 3,
        DIRECTLIBRARYITEM = 4,  // mattbw 2013-11-18; needed for direct library addition

        /**
         * Text
         **/
        AUTOTXTON = (0 << 10),  //UNUSED
        AUTOTXTOFF = (1 << 10), //UNUSED
        SENDTXTLIST = (2 << 10),    //UNSUED
        READ = (3 << 10),   //UNSUED
        UNREAD = (4 << 10), //UNUSED

        /**
         * Database
         **/
        LIBRARYSEARCH = (0 << 8),       //C  string artist, string title			MUSICLIBRESULT | LIBRARYERROR
        LIBRARYORDERING = (1 << 8),     //C  [VALUE-maybedirty] u32int orderingfield, u32int reverseorder?
        LIBRARYRESULT = (2 << 8),       //S  [0](count) u32int count

        //   [1](data)[VALUE-maybedirty] u32int index, string description
        LIBRARYERROR = (3 << 8),        //S  [-][VALUE-errorCode] string description

        GETSHOWS = (4 << 8),        //C  [-][VALUE-0]		{current user's shows)		SHOW

        //   [-][VALUE-1]		{system shows}				SHOW
        //   [-][VALUE-2]		string username				SHOW
        SHOW = (5 << 8),        //S  [0](count)	u32int count

        //   [1](data)	u32int showid, string description
        GETLISTINGS = (6 << 8),     //C				u32int showid

        LISTING = (7 << 8),     //S  [0](count) u32int count

        //   [1](data)  u32int listingid, u32int channel, string description
        ASSIGNLISTING = (8 << 8),       //C  [-][channel] u32int listingid

        BAPSDBERROR = (9 << 8),     //S  [-][VALUE-errorCode] string description

        /**
         * Library orderings
        **/
        ORDER_BYARTIST = 0,
        ORDER_BYTITLE = 1,
        ORDER_BYDATEADDED = 2,
        ORDER_BYDATERELEASED = 3,

        ORDER_ASCENDING = 0,
        ORDER_DESCENDING = 1,

        LIBRARY_MAYBEDIRTY = 1,
        LIBRARY_DIRTY = 2,

        /**
         * Config
         **/
        GETOPTIONS = (0 << 8),  //(no args)			OPTION (count-data)
        GETOPTIONCHOICES = (1 << 8),    //u32int optionid 	OPTIONCHOICE (count-data)
        GETCONFIGSETTINGS = (2 << 8),   //(no args)			CONFIGSETTING (count-data)
        GETCONFIGSETTING = (3 << 8),    //u32int optionid	CONFIGSETTING
        GETOPTION = (4 << 8),   //string optionName	OPTION CONFIGSETTING  (COUNT-DATA)
        SETCONFIGVALUE = (5 << 8),  //[x][0/1](uses index)[6bit index]	u32int optionid, u32int type, [string value | u32int value] CONFIGRESULT
        GETUSERS = (6 << 8),    //(no args)			USER (count-data)
        GETPERMISSIONS = (7 << 8),  //(no args)			PERMISSION (count-data)
        GETUSER = (8 << 8), //string username	USER

        ADDUSER = (9 << 8), //string username, string password		USERRESULT
        REMOVEUSER = (10 << 8), //string username						USERRESULT
        SETPASSWORD = (11 << 8),    //string username, string password		USERRESULT
        GRANTPERMISSION = (12 << 8),    //string username, u32int permission	USERRESULT
        REVOKEPERMISSION = (13 << 8),   //string username, u32int permission	USERRESULT

        OPTION = (16 << 8), // [0](count)		u32int count

        // [1](data)[0/1](uses index)[6bit index]	u32int optionid, string optionDesc, u32int type
        OPTIONCHOICE = (17 << 8),   // [0](count)		u32int count

        // [1](data)		u32int optionid, u32int optionValueid, string optionValueName
        CONFIGSETTING = (18 << 8),  // [0](count)		u32int count

        // [1](data)[0/1](uses index)[6bit index]	u32int optionid, u32int type, [u32int value | string value]
        USER = (19 << 8), // [0](count)		u32int count

        // [1](data)		string username, u32int permission
        PERMISSION = (20 << 8), // [0](count)		u32int count

        // [1](data)		u32int permission, string name
        USERRESULT = (21 << 8), // [RESULTVALUE] string resultText

        CONFIGRESULT = (22 << 8), // [x][0/1](uses index)[6bit index] u32int optionid, u32int result
        CONFIGERROR = (23 << 8), // [x][x][6bit errorcode]					string description
        GETIPRESTRICTIONS = (24 << 8), // (no args)		IPRESTRICTION count/data *2(allow deny lists)
        IPRESTRICTION = (25 << 8), // [0](count) [0/1](allow/deny) u32int count

        // [1](data)  [0/1](allow/deny)	string ipaddress, u32int mask
        ALTERIPRESTRICTION = (26 << 8), // [0/1](add/remove) [0/1](allow/deny) string ipaddress, u32int mask

        /**
         * System
         **/
        LISTFILES = (0 << 8),   //C-[x][VALUE-dirnumber]
        FILENAME = (1 << 8),    //S-[0](count)[VALUE-dirnumber] u32int count, string niceDirectoryName

        //	[1](data) [VALUE-dirnumber] u32int index, string filename
        SENDMESSAGE = (2 << 8), //C-	u32int clientid, string message

        AUTOUPDATE = (3 << 8),  //C-[VALUE-1](on)

        //  [VALUE-X](off)
        END = (4 << 8), //CS-	string reason

        SENDLOGMESSAGE = (5 << 8),  //S -	string description
        SETBINARYMODE = (6 << 8),   //C-
        SEED = (7 << 8),    //S- string encryptionSeed
        LOGIN = (8 << 8),   //C- string username, string encryptedpass
        LOGINRESULT = (9 << 8), //S- [VALUE-resultCode] string resultText
        VERSION = (10 << 8), //C-

        //S- string date, string time, string version, string author
        FEEDBACK = (11 << 8), //C- string message

        //S- [VALUE-0/1] success/fail
        CLIENTCHANGE = (12 << 8),   //S- [VALUE-0] string clientToRemove

        //   [VALUE-1] string clientToAdd
        SCROLLTEXT = (13 << 8), //S- [VALUE-0] scroll down

        //   [VALUE-1] scroll up
        TEXTSIZE = (14 << 8), //S- [VALUE-0] text smaller

        //	 [VALUE-1] text bigger
        QUIT = (15 << 8), //(no args)
    }

    internal static class CommandExtensions
    {
        internal static ushort Channel(this Command cmd) => (ushort)(cmd & Command.PLAYBACK_CHANNELMASK);
        internal static bool IsFlagSet(this Command cmd, Command flag) => (cmd & flag) == flag;
    }
}
