namespace BAPSClientCommon.ServerConfig
{
	/// <summary>
	///     Enumeration of known BapsNet configuration keys.
	///     <para>
	///         These are integer indices, rather than string descriptions;
	///         they should correspond to the on-the-wire BapsNet setting ID.
	///     </para>
	/// </summary>
	public enum OptionKey : uint
    {
        ChannelCount = 0,
        Device = 1,
        ChannelName = 2,
        AutoAdvance = 3,
        AutoPlay = 4,
        Repeat = 5,
        DirectoryCount = 6,
        DirectoryName = 7,
        DirectoryLocation = 8,
        ServerId = 9,
        Port = 10,
        MaxQueueConns = 11,
        ClientConnLimit = 12,
        DbServer = 13,
        DbPort = 14,
        LibraryDbName = 15,
        BapsDbName = 16,
        DbUsername = 17,
        DbPassword = 18,
        LibraryLocation = 19,
        CleanMusicOnly = 20,
        SaveIntroPositions = 21,
        StorePlaybackEvents = 22,
        LogName = 23,
        LogSource = 24,
        SupportAddress = 25,
        SmtpServer = 26,
        ControllerEnabled = 27,
        ControllerPort = 28,
        ControllerButtonCount = 29,
        ControllerButtonCode = 30,
        PaddleMode = 31,
        Controller2Enabled = 31,
        Controller2DeviceCount = 32,
        Controller2Serial = 33,
        Controller2Offset = 34,
        Controller2ButtonCount = 35,
        Controller2ButtonCode = 36,
        Invalid = uint.MaxValue
    }
}