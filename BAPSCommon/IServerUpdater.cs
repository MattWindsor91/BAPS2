using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Model;

namespace BAPSClientCommon
{
    /// <summary>
    /// Event interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackServerUpdater
    {
        /// <summary>
        ///     Event raised when the server reports a change in channel state.
        /// </summary>
        event ServerUpdates.ChannelStateEventHandler ChannelState;

        /// <summary>
        ///     Event raised when the server reports a change in channel marker.
        /// </summary>
        event ServerUpdates.ChannelMarkerEventHandler Marker;

    }

    /// <summary>
    /// Event interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater
    {
        event ServerUpdates.DirectoryFileAddHandler DirectoryFileAdd;
        event ServerUpdates.DirectoryPrepareHandler DirectoryPrepare;
    }

    /// <summary>
    /// Event interface for classes that send BapsNet server config updates.
    /// </summary>
    public interface IConfigServerUpdater
    {
        /// <summary>
        /// Event raised when the server declares a config choice.
        /// </summary>
        event ServerUpdates.ConfigChoiceHandler ConfigChoice;

        /// <summary>
        /// Event raised when the server declares a config option.
        /// </summary>
        event ServerUpdates.ConfigOptionHandler ConfigOption;

        /// <summary>
        /// Event raised when the server declares that a setting on a
        /// config option has changed.
        /// </summary>
        event ServerUpdates.ConfigSettingHandler ConfigSetting;

        event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;
    }

    /// <summary>
    /// Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater
    {
        event EventHandler<(ushort channelID, uint duration)> Duration;
        event ServerUpdates.ErrorEventHandler Error;
        event ServerUpdates.CountEventHandler IncomingCount;
        event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IpRestriction;
        event ServerUpdates.ItemAddEventHandler ItemAdd;
        event ServerUpdates.ItemDeleteEventHandler ItemDelete;
        event ServerUpdates.ItemMoveEventHandler ItemMove;
        event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        event EventHandler<(ushort channelID, uint index, Track entry)> LoadedItem;
        event EventHandler<(uint permissionCode, string description)> Permission;
        event ServerUpdates.ChannelResetEventHandler ResetPlaylist;
        event EventHandler<bool> ServerQuit;
        event EventHandler<(uint showID, string description)> ShowResult;
        event EventHandler<(ushort ChannelID, uint index, TextTrack entry)> TextItem;
        event EventHandler<ServerUpdates.UpDown> TextScroll;
        event EventHandler<ServerUpdates.UpDown> TextSizeChange;
        event EventHandler<(Command command, string description)> UnknownCommand;
        event EventHandler<(string username, uint permissions)> User;
        event EventHandler<(byte resultCode, string description)> UserResult;
        event EventHandler<Receiver.VersionInfo> Version;
    }
}