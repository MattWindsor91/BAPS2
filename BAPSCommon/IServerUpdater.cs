using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Event interface common to all server update classes.
    /// </summary>
    public interface IBaseServerUpdater
    {
        /// <summary>
        ///     Event raised to notify subscribers that a particular number of items is en route.
        /// </summary>
        event EventHandler<Updates.CountEventArgs> IncomingCount;
    }
    
    /// <summary>
    ///     Event interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackServerUpdater : IBaseServerUpdater
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
    public interface IDirectoryServerUpdater : IBaseServerUpdater
    {
        event EventHandler<Updates.DirectoryFileAddArgs> DirectoryFileAdd;
        event EventHandler<Updates.DirectoryPrepareArgs> DirectoryPrepare;
    }

    /// <summary>
    ///     Event interface for classes that send BapsNet server config updates.
    /// </summary>
    public interface IConfigServerUpdater : IBaseServerUpdater
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
    public interface IPlaylistServerUpdater : IBaseServerUpdater
    {
        event EventHandler<Updates.TrackAddEventArgs> ItemAdd;
        event EventHandler<Updates.TrackDeleteEventArgs> ItemDelete;
        event EventHandler<Updates.TrackMoveEventArgs> ItemMove;
        
        event EventHandler<Updates.ChannelResetEventArgs> ResetPlaylist;
    }

    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        event EventHandler<Updates.ErrorEventArgs> Error;
        event EventHandler<bool> ServerQuit;
        event EventHandler<Receiver.VersionInfo> Version;
    }
    
    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater
        : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater, IPlaylistServerUpdater,
            ISystemServerUpdater
    {
        event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IpRestriction;

        event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        event EventHandler<(uint permissionCode, string description)> Permission;
        event EventHandler<(uint showID, string description)> ShowResult;
        event EventHandler<Updates.UpDown> TextScroll;
        event EventHandler<Updates.UpDown> TextSizeChange;
        event EventHandler<(Command command, string description)> UnknownCommand;
        event EventHandler<(string username, uint permissions)> User;
        event EventHandler<(byte resultCode, string description)> UserResult;
    }
}