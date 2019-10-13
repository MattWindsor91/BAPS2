using System;
using System.Reactive.Linq;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     A <see cref="IFullEventFeed"/> that never sends any events.
    /// </summary>
    public class EmptyEventFeed : IFullEventFeed
    {
        public IObservable<CountArgs> ObserveIncomingCount => Observable.Empty<CountArgs>();
        public IObservable<UnknownCommandArgs> ObserveUnknownCommand => Observable.Empty<UnknownCommandArgs>();
        public IObservable<ConfigChoiceArgs> ObserveConfigChoice => Observable.Empty<ConfigChoiceArgs>();
        public IObservable<ConfigOptionArgs> ObserveConfigOption => Observable.Empty<ConfigOptionArgs>();
        public IObservable<ConfigSettingArgs> ObserveConfigSetting => Observable.Empty<ConfigSettingArgs>();
        public IObservable<ConfigResultArgs> ObserveConfigResult => Observable.Empty<ConfigResultArgs>();
        public IObservable<IpRestrictionArgs> ObserveIpRestriction => Observable.Empty<IpRestrictionArgs>();
        public IObservable<PermissionArgs> ObservePermission => Observable.Empty<PermissionArgs>();
        public IObservable<UserArgs> ObserveUser => Observable.Empty<UserArgs>();
        public IObservable<UserResultArgs> ObserveUserResult => Observable.Empty<UserResultArgs>();
        public IObservable<LibraryResultArgs> ObserveLibraryResult => Observable.Empty<LibraryResultArgs>();
        public IObservable<ListingResultArgs> ObserveListingResult => Observable.Empty<ListingResultArgs>();
        public IObservable<ShowResultArgs> ObserveShowResult => Observable.Empty<ShowResultArgs>();
        public IObservable<DirectoryFileAddArgs> ObserveDirectoryFileAdd => Observable.Empty<DirectoryFileAddArgs>();
        public IObservable<DirectoryPrepareArgs> ObserveDirectoryPrepare => Observable.Empty<DirectoryPrepareArgs>();
        public IObservable<PlaybackStateChangeArgs> ObservePlayerState => Observable.Empty<PlaybackStateChangeArgs>();
        public IObservable<MarkerChangeArgs> ObserveMarker => Observable.Empty<MarkerChangeArgs>();
        public IObservable<TrackLoadArgs> ObserveTrackLoad => Observable.Empty<TrackLoadArgs>();
        public IObservable<TrackAddArgs> ObserveTrackAdd => Observable.Empty<TrackAddArgs>();
        public IObservable<TrackDeleteArgs> ObserveTrackDelete => Observable.Empty<TrackDeleteArgs>();
        public IObservable<TrackMoveArgs> ObserveTrackMove => Observable.Empty<TrackMoveArgs>();
        public IObservable<PlaylistResetArgs> ObservePlaylistReset => Observable.Empty<PlaylistResetArgs>();
        public IObservable<ErrorEventArgs> ObserveError => Observable.Empty<ErrorEventArgs>();
        public IObservable<ServerQuitArgs> ObserveServerQuit => Observable.Empty<ServerQuitArgs>();
        public IObservable<ServerVersionArgs> ObserveServerVersion => Observable.Empty<ServerVersionArgs>();
        public IObservable<TextSettingArgs> ObserveTextSetting => Observable.Empty<TextSettingArgs>();
        public IObservable<MessageArgsBase> ObserveMessages => Observable.Empty<CountArgs>();
    }
}