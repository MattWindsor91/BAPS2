using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Event interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackServerUpdater
    {
        /// <summary>
        ///     Event raised when the server reports a change in channel state.
        /// </summary>
        event EventHandler<Updates.PlayerStateEventArgs> ChannelState;

        /// <summary>
        ///     Event raised when the server reports a change in channel marker.
        /// </summary>
        event EventHandler<Updates.MarkerEventArgs> ChannelMarker;
        
        event EventHandler<Updates.TrackLoadEventArgs> TrackLoad;
    }

    /// <summary>
    ///     Event interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater
    {
        event EventHandler<Updates.DirectoryFileAddArgs> DirectoryFileAdd;
        event EventHandler<Updates.DirectoryPrepareArgs> DirectoryPrepare;
    }

    /// <summary>
    ///     Event interface for classes that send BapsNet server config updates.
    /// </summary>
    public interface IConfigServerUpdater
    {
        /// <summary>
        ///     Event raised when the server declares a config choice.
        /// </summary>
        event EventHandler<Updates.ConfigChoiceArgs> ConfigChoice;

        /// <summary>
        ///     Event raised when the server declares a config option.
        /// </summary>
        event EventHandler<Updates.ConfigOptionArgs> ConfigOption;

        /// <summary>
        ///     Event raised when the server declares that a setting on a
        ///     config option has changed.
        /// </summary>
        event EventHandler<Updates.ConfigSettingArgs> ConfigSetting;

        event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;
    }

    /// <summary>
    ///     Event interface for classes that send BapsNet server playlist updates.
    /// </summary>
    public interface IPlaylistServerUpdater
    {
        event EventHandler<Updates.TrackAddEventArgs> ItemAdd;
        event EventHandler<Updates.TrackDeleteEventArgs> ItemDelete;
        event EventHandler<Updates.TrackMoveEventArgs> ItemMove;
        
        event EventHandler<Updates.ChannelResetEventArgs> ResetPlaylist;
    }
    
    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater, IPlaylistServerUpdater
    {
        event EventHandler<Updates.ErrorEventArgs> Error;
        event EventHandler<Updates.CountEventArgs> IncomingCount;
        event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IpRestriction;

        event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        event EventHandler<(uint permissionCode, string description)> Permission;
        event EventHandler<bool> ServerQuit;
        event EventHandler<(uint showID, string description)> ShowResult;
        event EventHandler<Updates.UpDown> TextScroll;
        event EventHandler<Updates.UpDown> TextSizeChange;
        event EventHandler<(Command command, string description)> UnknownCommand;
        event EventHandler<(string username, uint permissions)> User;
        event EventHandler<(byte resultCode, string description)> UserResult;
        event EventHandler<Receiver.VersionInfo> Version;
    }
}