using System;
using System.ComponentModel;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.ServerConfig
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

	/// <summary>
	/// 	Extension methods for <see cref="OptionKey"/>s.
	/// </summary>
	public static class OptionKeyExtensions
	{
		/// <summary>
		/// 	Converts a <see cref="ChannelFlag"/> to an <see cref="OptionKey"/>.
		/// </summary>
		/// <param name="flag">The channel flag to convert.</param>
		/// <returns>The corresponding option key.</returns>
		/// <exception cref="InvalidEnumArgumentException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static OptionKey ToOptionKey(this ChannelFlag flag)
		{
			if (!Enum.IsDefined(typeof(ChannelFlag), flag))
				throw new InvalidEnumArgumentException(nameof(flag), (int) flag, typeof(ChannelFlag));
            return flag switch {
				ChannelFlag.AutoAdvance => OptionKey.AutoAdvance,
				ChannelFlag.PlayOnLoad => OptionKey.AutoPlay,
                _ => OptionKey.Invalid
			};
        }

		/// <summary>
		/// 	Converts a <see cref="ChannelFlag"/> to an <see cref="OptionKey"/>.
		/// </summary>
		/// <param name="key">The option key to convert.</param>
		/// <returns>The corresponding channel flag.</returns>
		/// <exception cref="InvalidEnumArgumentException">
		/// 	<paramref name="key"/> isn't a valid <see cref="OptionKey"/>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The option key doesn't correspond to a valid channel flag.
		/// </exception>
		public static ChannelFlag ToChannelFlag(this OptionKey key)
		{
			if (!Enum.IsDefined(typeof(OptionKey), key))
				throw new InvalidEnumArgumentException(nameof(key), (int) key, typeof(OptionKey));
			switch (key)
			{
				case OptionKey.AutoAdvance:
					return ChannelFlag.AutoAdvance;
				case OptionKey.AutoPlay:
					return ChannelFlag.PlayOnLoad;
				default:
					throw new ArgumentOutOfRangeException(nameof(key), key, "Not a channel flag");
			}
		}	
	}
}