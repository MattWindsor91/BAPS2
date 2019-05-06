using System;
using System.Reactive.Linq;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     A <see cref="IBaseServerUpdater"/> that works by filtering
    ///     the message stream of a <see cref="Receiver"/> for various types of
    ///     server update messages.
    /// </summary>
    public class FilteringServerUpdater : IServerUpdater
    {
        public FilteringServerUpdater(IObservable<ArgsBase> master)
        {
            ObserveMessages = master;
        }

        protected IObservable<ArgsBase> ObserveMessages { get; set; }
        
         /// <summary>
        ///     Convenience observable for observing all protocol messages
        ///     in <see cref="ObserveMessages"/> that concern playback state
        ///     changes.
        /// </summary>
        public IObservable<PlaybackStateChangeArgs> ObservePlayerState =>
            ObserveMessages.OfType<PlaybackStateChangeArgs>();
        
        /// <summary>
        ///     Convenience observable for observing all protocol messages
        ///     in <see cref="ObserveMessages"/> that concern marker changes.
        /// </summary>
        public IObservable<MarkerChangeArgs> ObserveMarker =>
            ObserveMessages.OfType<MarkerChangeArgs>();

        public IObservable<TrackLoadArgs> ObserveTrackLoad =>
            ObserveMessages.OfType<TrackLoadArgs>();

        public IObservable<CountArgs> ObserveIncomingCount =>
            ObserveMessages.OfType<CountArgs>();

        public IObservable<ConfigChoiceArgs> ObserveConfigChoice =>
            ObserveMessages.OfType<ConfigChoiceArgs>();

        public IObservable<ConfigOptionArgs> ObserveConfigOption =>
            ObserveMessages.OfType<ConfigOptionArgs>();

        public IObservable<ConfigSettingArgs> ObserveConfigSetting =>
            ObserveMessages.OfType<ConfigSettingArgs>();

        public IObservable<ConfigResultArgs> ObserveConfigResult =>
            ObserveMessages.OfType<ConfigResultArgs>();

        public IObservable<DirectoryFileAddArgs> ObserveDirectoryFileAdd =>
            ObserveMessages.OfType<DirectoryFileAddArgs>();

        public IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare =>
            ObserveMessages.OfType<DirectoryPrepareEventArgs>();

        public IObservable<TrackAddArgs> ObserveTrackAdd =>
            ObserveMessages.OfType<TrackAddArgs>();

        public IObservable<TrackDeleteArgs> ObserveTrackDelete =>
            ObserveMessages.OfType<TrackDeleteArgs>();

        public IObservable<TrackMoveArgs> ObserveTrackMove =>
            ObserveMessages.OfType<TrackMoveArgs>();

        public IObservable<PlaylistResetArgs> ObservePlaylistReset =>
            ObserveMessages.OfType<PlaylistResetArgs>();

        public IObservable<ErrorEventArgs> ObserveError =>
            ObserveMessages.OfType<ErrorEventArgs>();

        public IObservable<ServerQuitArgs> ObserveServerQuit =>
            ObserveMessages.OfType<ServerQuitArgs>();

        public IObservable<ServerVersionArgs> ObserveServerVersion =>
            ObserveMessages.OfType<ServerVersionArgs>();

        public IObservable<IpRestrictionArgs> ObserveIpRestriction =>
            ObserveMessages.OfType<IpRestrictionArgs>();

        public IObservable<LibraryResultArgs> ObserveLibraryResult =>
            ObserveMessages.OfType<LibraryResultArgs>();

        public IObservable<ListingResultArgs> ObserveListingResult =>
            ObserveMessages.OfType<ListingResultArgs>();

        public IObservable<PermissionArgs> ObservePermission =>
            ObserveMessages.OfType<PermissionArgs>();

        public IObservable<ShowResultArgs> ObserveShowResult =>
            ObserveMessages.OfType<ShowResultArgs>();

        public IObservable<TextSettingArgs> ObserveTextSetting =>
            ObserveMessages.OfType<TextSettingArgs>();

        public IObservable<UnknownCommandArgs> ObserveUnknownCommand =>
            ObserveMessages.OfType<UnknownCommandArgs>();

        public IObservable<UserArgs> ObserveUser =>
            ObserveMessages.OfType<UserArgs>();

        public IObservable<UserResultArgs> ObserveUserResult =>
            ObserveMessages.OfType<UserResultArgs>();
       
    }
}