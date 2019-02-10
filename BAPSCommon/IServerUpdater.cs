using System;
using System.Reactive.Linq;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Observable interface common to all server update classes.
    /// </summary>
    public interface IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that notifies subscribers that a particular number of items is en route.
        /// </summary>
        IObservable<Updates.CountEventArgs> ObserveIncomingCount { get; }
    }

    /// <summary>
    ///     Observable interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackServerUpdater : IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that reports when the server reports a change in channel state.
        /// </summary>
        IObservable<Updates.PlayerStateEventArgs> ObservePlayerState { get; }

        /// <summary>
        ///     Observable that reports when the server reports a change in channel marker.
        /// </summary>
        IObservable<Updates.MarkerEventArgs> ObserveMarker { get; }

        /// <summary>
        ///     Observable that reports when the server reports that a new track has been loaded into the player.
        /// </summary>
        IObservable<Updates.TrackLoadEventArgs> ObserveTrackLoad { get; }
    }

    /// <summary>
    ///     Observable interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.DirectoryFileAddEventArgs> ObserveDirectoryFileAdd { get; }
        IObservable<Updates.DirectoryPrepareEventArgs> ObserveDirectoryPrepare { get; }
    }

    /// <summary>
    ///     Observable interface for classes that send BapsNet server config updates.
    /// </summary>
    public interface IConfigServerUpdater : IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that reports when the server declares a config choice.
        /// </summary>
        IObservable<Updates.ConfigChoiceEventArgs> ObserveConfigChoice { get; }

        /// <summary>
        ///     Observable that reports when the server declares a config option.
        /// </summary>
        IObservable<Updates.ConfigOptionEventArgs> ObserveConfigOption { get; }

        /// <summary>
        ///     Event raised when the server declares that a setting on a
        ///     config option has changed.
        /// </summary>
        IObservable<Updates.ConfigSettingEventArgs> ObserveConfigSetting { get; }

        IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult { get; }
    }

    /// <summary>
    ///     Event interface for classes that send BapsNet server playlist updates.
    /// </summary>
    public interface IPlaylistServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.TrackAddEventArgs> ObserveTrackAdd { get; }
        IObservable<Updates.TrackDeleteEventArgs> ObserveTrackDelete { get; }
        IObservable<Updates.TrackMoveEventArgs> ObserveTrackMove { get; }

        IObservable<Updates.PlaylistResetEventArgs> ObservePlaylistReset { get; }
    }

    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.ErrorEventArgs> ObserveError { get; }
        IObservable<bool> ObserveServerQuit { get; }
        IObservable<Receiver.VersionInfo> ObserveVersion { get; }
    }

    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater
        : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater, IPlaylistServerUpdater,
            ISystemServerUpdater
    {
        IObservable<(Command cmdReceived, string ipAddress, uint mask)> ObserveIpRestriction { get; }

        IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult { get; }
        IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult { get; }
        IObservable<(uint permissionCode, string description)> ObservePermission { get; }
        IObservable<(uint showID, string description)> ObserveShowResult { get; }
        IObservable<Updates.UpDown> ObserveTextScroll { get; }
        IObservable<Updates.UpDown> ObserveTextSizeChange { get; }
        IObservable<(Command command, string description)> ObserveUnknownCommand { get; }
        IObservable<(string username, uint permissions)> ObserveUser { get; }
        IObservable<(byte resultCode, string description)> ObserveUserResult { get; }
    }
}