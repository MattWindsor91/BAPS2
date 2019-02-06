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
        event Updates.ChannelStateEventHandler ChannelState;

        /// <summary>
        ///     Event raised when the server reports a change in channel marker.
        /// </summary>
        event Updates.MarkerEventHandler ChannelMarker;
        
        event Updates.TrackLoadEventHandler TrackLoad;
    }

    /// <summary>
    ///     Event interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater
    {
        event Updates.DirectoryFileAddHandler DirectoryFileAdd;
        event Updates.DirectoryPrepareHandler DirectoryPrepare;
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
        event Updates.ItemAddEventHandler ItemAdd;
        event Updates.ItemDeleteEventHandler ItemDelete;
        event Updates.ItemMoveEventHandler ItemMove;
        
        event Updates.ChannelResetEventHandler ResetPlaylist;
    }
    
    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater, IPlaylistServerUpdater
    {
        event Updates.ErrorEventHandler Error;
        event Updates.CountEventHandler IncomingCount;
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