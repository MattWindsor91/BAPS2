using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common
{
    public partial class ClientCore
    {
        [NotNull] private readonly Subject<CountEventArgs> _incomingCount =
            new Subject<CountEventArgs>();

        [NotNull] private readonly Subject<ConfigChoiceEventArgs> _observeConfigChoice =
            new Subject<ConfigChoiceEventArgs>();

        [NotNull] private readonly Subject<ConfigOptionEventArgs> _observeConfigOption =
            new Subject<ConfigOptionEventArgs>();

        [NotNull]
        private readonly Subject<(CommandWord cmdReceived, uint optionID, ConfigResult result)> _observeConfigResult =
            new Subject<(CommandWord cmdReceived, uint optionID, ConfigResult result)>();

        [NotNull] private readonly Subject<ConfigSettingEventArgs> _observeConfigSetting =
            new Subject<ConfigSettingEventArgs>();

        [NotNull] private readonly Subject<DirectoryFileAddEventArgs> _observeDirectoryFileAdd =
            new Subject<DirectoryFileAddEventArgs>();

        [NotNull] private readonly Subject<DirectoryPrepareEventArgs> _observeDirectoryPrepare =
            new Subject<DirectoryPrepareEventArgs>();

        [NotNull]
        private readonly Subject<ErrorEventArgs> _observeError = new Subject<ErrorEventArgs>();

        [NotNull] private readonly Subject<(CommandWord cmdReceived, string ipAddress, uint mask)> _observeIpRestriction =
            new Subject<(CommandWord cmdReceived, string ipAddress, uint mask)>();

        [NotNull] private readonly Subject<(uint resultID, byte dirtyStatus, string description)> _observeLibraryResult
            =
            new Subject<(uint resultID, byte dirtyStatus, string description)>();

        [NotNull] private readonly Subject<(uint listingID, uint channelID, string description)> _observeListingResult =
            new Subject<(uint listingID, uint channelID, string description)>();

        [NotNull] private readonly Subject<MarkerEventArgs> _observeMarker =
            new Subject<MarkerEventArgs>();

        [NotNull] private readonly Subject<(uint permissionCode, string description)> _observePermission =
            new Subject<(uint permissionCode, string description)>();

        [NotNull] private readonly Subject<PlayerStateEventArgs> _observePlayerState =
            new Subject<PlayerStateEventArgs>();

        [NotNull] private readonly Subject<PlaylistResetEventArgs> _observePlaylistReset =
            new Subject<PlaylistResetEventArgs>();

        [NotNull] private readonly Subject<bool> _observeServerQuit = new Subject<bool>();

        [NotNull] private readonly Subject<(uint showID, string description)> _observeShowResult =
            new Subject<(uint showID, string description)>();

        [NotNull] private readonly Subject<TextSettingEventArgs> _observeTextSetting =
            new Subject<TextSettingEventArgs>();

        [NotNull] private readonly Subject<TrackAddEventArgs> _observeTrackAdd =
            new Subject<TrackAddEventArgs>();

        [NotNull] private readonly Subject<TrackDeleteEventArgs> _observeTrackDelete =
            new Subject<TrackDeleteEventArgs>();

        [NotNull] private readonly Subject<TrackLoadEventArgs> _observeTrackLoad =
            new Subject<TrackLoadEventArgs>();

        [NotNull] private readonly Subject<TrackMoveEventArgs> _observeTrackMove =
            new Subject<TrackMoveEventArgs>();

        [NotNull] private readonly Subject<(CommandWord command, string description)> _observeUnknownCommand =
            new Subject<(CommandWord command, string description)>();

        [NotNull] private readonly Subject<(string username, uint permissions)> _observeUser =
            new Subject<(string username, uint permissions)>();

        [NotNull] private readonly Subject<(byte resultCode, string description)> _observeUserResult =
            new Subject<(byte resultCode, string description)>();

        [NotNull] private readonly Subject<Receiver.VersionInfo> _observeVersion = new Subject<Receiver.VersionInfo>();

        [NotNull] private readonly IList<IDisposable> _receiverSubscriptions = new List<IDisposable>();

        public IObservable<CountEventArgs> ObserveIncomingCount => _incomingCount;

        public IObservable<ConfigChoiceEventArgs> ObserveConfigChoice => _observeConfigChoice;

        public IObservable<ConfigOptionEventArgs> ObserveConfigOption => _observeConfigOption;

        public IObservable<ConfigSettingEventArgs> ObserveConfigSetting => _observeConfigSetting;

        public IObservable<(CommandWord cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult =>
            _observeConfigResult;

        public IObservable<DirectoryFileAddEventArgs> ObserveDirectoryFileAdd => _observeDirectoryFileAdd;

        public IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare => _observeDirectoryPrepare;

        public IObservable<PlayerStateEventArgs> ObservePlayerState => _observePlayerState;

        public IObservable<MarkerEventArgs> ObserveMarker => _observeMarker;

        public IObservable<TrackLoadEventArgs> ObserveTrackLoad => _observeTrackLoad;

        public IObservable<TrackAddEventArgs> ObserveTrackAdd => _observeTrackAdd;

        public IObservable<TrackDeleteEventArgs> ObserveTrackDelete => _observeTrackDelete;

        public IObservable<TrackMoveEventArgs> ObserveTrackMove => _observeTrackMove;

        public IObservable<PlaylistResetEventArgs> ObservePlaylistReset => _observePlaylistReset;

        public IObservable<ErrorEventArgs> ObserveError => _observeError;

        public IObservable<bool> ObserveServerQuit => _observeServerQuit;

        public IObservable<Receiver.VersionInfo> ObserveVersion => _observeVersion;

        public IObservable<(CommandWord cmdReceived, string ipAddress, uint mask)> ObserveIpRestriction =>
            _observeIpRestriction;

        public IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult =>
            _observeLibraryResult;

        public IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult =>
            _observeListingResult;

        public IObservable<(uint permissionCode, string description)> ObservePermission => _observePermission;

        public IObservable<(uint showID, string description)> ObserveShowResult => _observeShowResult;

        public IObservable<TextSettingEventArgs> ObserveTextSetting => _observeTextSetting;

        public IObservable<(CommandWord command, string description)> ObserveUnknownCommand => _observeUnknownCommand;

        public IObservable<(string username, uint permissions)> ObserveUser => _observeUser;

        public IObservable<(byte resultCode, string description)> ObserveUserResult => _observeUserResult;

        /// <summary>
        ///     Subscribes each forwarding <see cref="Subject{T}" /> on the client core to its corresponding
        ///     receiver observable.
        /// </summary>
        private void SubscribeToReceiver()
        {
            if (_receiver == null) return;
            _receiverSubscriptions.Add(_receiver.ObserveConfigChoice.Subscribe(_observeConfigChoice));
            _receiverSubscriptions.Add(_receiver.ObserveConfigOption.Subscribe(_observeConfigOption));
            _receiverSubscriptions.Add(_receiver.ObserveConfigSetting.Subscribe(_observeConfigSetting));
            _receiverSubscriptions.Add(_receiver.ObserveConfigResult.Subscribe(_observeConfigResult));
            _receiverSubscriptions.Add(_receiver.ObserveConfigOption.Subscribe(_observeConfigOption));
            _receiverSubscriptions.Add(_receiver.ObserveDirectoryPrepare.Subscribe(_observeDirectoryPrepare));
            _receiverSubscriptions.Add(_receiver.ObserveDirectoryFileAdd.Subscribe(_observeDirectoryFileAdd));
            _receiverSubscriptions.Add(_receiver.ObservePlayerState.Subscribe(_observePlayerState));
            _receiverSubscriptions.Add(_receiver.ObserveMarker.Subscribe(_observeMarker));
            _receiverSubscriptions.Add(_receiver.ObserveError.Subscribe(_observeError));
            _receiverSubscriptions.Add(_receiver.ObserveIncomingCount.Subscribe(_incomingCount));
            _receiverSubscriptions.Add(_receiver.ObserveIpRestriction.Subscribe(_observeIpRestriction));
            _receiverSubscriptions.Add(_receiver.ObserveTrackAdd.Subscribe(_observeTrackAdd));
            _receiverSubscriptions.Add(_receiver.ObserveTrackDelete.Subscribe(_observeTrackDelete));
            _receiverSubscriptions.Add(_receiver.ObserveTrackMove.Subscribe(_observeTrackMove));
            _receiverSubscriptions.Add(_receiver.ObserveLibraryResult.Subscribe(_observeLibraryResult));
            _receiverSubscriptions.Add(_receiver.ObserveListingResult.Subscribe(_observeListingResult));
            _receiverSubscriptions.Add(_receiver.ObserveTrackLoad.Subscribe(_observeTrackLoad));
            _receiverSubscriptions.Add(_receiver.ObservePermission.Subscribe(_observePermission));
            _receiverSubscriptions.Add(_receiver.ObservePlaylistReset.Subscribe(_observePlaylistReset));
            _receiverSubscriptions.Add(_receiver.ObserveServerQuit.Subscribe(_observeServerQuit));
            _receiverSubscriptions.Add(_receiver.ObserveShowResult.Subscribe(_observeShowResult));
            _receiverSubscriptions.Add(_receiver.ObserveTextSetting.Subscribe(_observeTextSetting));
            _receiverSubscriptions.Add(_receiver.ObserveUnknownCommand.Subscribe(_observeUnknownCommand));
            _receiverSubscriptions.Add(_receiver.ObserveUser.Subscribe(_observeUser));
            _receiverSubscriptions.Add(_receiver.ObserveUserResult.Subscribe(_observeUserResult));
            _receiverSubscriptions.Add(_receiver.ObserveVersion.Subscribe(_observeVersion));
        }

        /// <summary>
        ///     Disposes each subscription created by <see cref="SubscribeToReceiver" />.
        /// </summary>
        private void UnsubscribeFromReceiver()
        {
            foreach (var subscription in _receiverSubscriptions) subscription.Dispose();
        }
    }
}