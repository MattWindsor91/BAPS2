using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;
using JetBrains.Annotations;

namespace BAPSClientCommon
{
    public partial class ClientCore
    {
        [NotNull] private readonly Subject<Updates.CountEventArgs> _incomingCount =
            new Subject<Updates.CountEventArgs>();

        [NotNull] private readonly Subject<Updates.ConfigChoiceEventArgs> _observeConfigChoice =
            new Subject<Updates.ConfigChoiceEventArgs>();

        [NotNull] private readonly Subject<Updates.ConfigOptionEventArgs> _observeConfigOption =
            new Subject<Updates.ConfigOptionEventArgs>();

        [NotNull]
        private readonly Subject<(Command cmdReceived, uint optionID, ConfigResult result)> _observeConfigResult =
            new Subject<(Command cmdReceived, uint optionID, ConfigResult result)>();

        [NotNull] private readonly Subject<Updates.ConfigSettingEventArgs> _observeConfigSetting =
            new Subject<Updates.ConfigSettingEventArgs>();

        [NotNull] private readonly Subject<Updates.DirectoryFileAddEventArgs> _observeDirectoryFileAdd =
            new Subject<Updates.DirectoryFileAddEventArgs>();

        [NotNull] private readonly Subject<Updates.DirectoryPrepareEventArgs> _observeDirectoryPrepare =
            new Subject<Updates.DirectoryPrepareEventArgs>();

        [NotNull]
        private readonly Subject<Updates.ErrorEventArgs> _observeError = new Subject<Updates.ErrorEventArgs>();

        [NotNull] private readonly Subject<(Command cmdReceived, string ipAddress, uint mask)> _observeIpRestriction =
            new Subject<(Command cmdReceived, string ipAddress, uint mask)>();

        [NotNull]
        private readonly Subject<(uint resultID, byte dirtyStatus, string description)> _observeLibraryResult =
            new Subject<(uint resultID, byte dirtyStatus, string description)>();

        [NotNull] private readonly Subject<(uint listingID, uint channelID, string description)> _observeListingResult =
            new Subject<(uint listingID, uint channelID, string description)>();

        [NotNull] private readonly Subject<Updates.MarkerEventArgs> _observeMarker =
            new Subject<Updates.MarkerEventArgs>();

        [NotNull] private readonly Subject<(uint permissionCode, string description)> _observePermission =
            new Subject<(uint permissionCode, string description)>();

        [NotNull] private readonly Subject<Updates.PlayerStateEventArgs> _observePlayerState =
            new Subject<Updates.PlayerStateEventArgs>();

        [NotNull] private readonly Subject<Updates.PlaylistResetEventArgs> _observePlaylistReset =
            new Subject<Updates.PlaylistResetEventArgs>();

        [NotNull] private readonly Subject<bool> _observeServerQuit = new Subject<bool>();

        [NotNull] private readonly Subject<(uint showID, string description)> _observeShowResult =
            new Subject<(uint showID, string description)>();

        [NotNull] private readonly Subject<Updates.TextSettingEventArgs> _observeTextSetting = new Subject<Updates.TextSettingEventArgs>();

        [NotNull] private readonly Subject<Updates.TrackAddEventArgs> _observeTrackAdd =
            new Subject<Updates.TrackAddEventArgs>();

        [NotNull] private readonly Subject<Updates.TrackDeleteEventArgs> _observeTrackDelete =
            new Subject<Updates.TrackDeleteEventArgs>();

        [NotNull] private readonly Subject<Updates.TrackLoadEventArgs> _observeTrackLoad =
            new Subject<Updates.TrackLoadEventArgs>();

        [NotNull] private readonly Subject<Updates.TrackMoveEventArgs> _observeTrackMove =
            new Subject<Updates.TrackMoveEventArgs>();

        [NotNull] private readonly Subject<(Command command, string description)> _observeUnknownCommand =
            new Subject<(Command command, string description)>();

        [NotNull] private readonly Subject<(string username, uint permissions)> _observeUser =
            new Subject<(string username, uint permissions)>();

        [NotNull] private readonly Subject<(byte resultCode, string description)> _observeUserResult =
            new Subject<(byte resultCode, string description)>();

        [NotNull] private readonly Subject<Receiver.VersionInfo> _observeVersion = new Subject<Receiver.VersionInfo>();

        [NotNull] private readonly IList<IDisposable> _receiverSubscriptions = new List<IDisposable>();

        public IObservable<Updates.CountEventArgs> ObserveIncomingCount => _incomingCount;

        public IObservable<Updates.ConfigChoiceEventArgs> ObserveConfigChoice => _observeConfigChoice;

        public IObservable<Updates.ConfigOptionEventArgs> ObserveConfigOption => _observeConfigOption;

        public IObservable<Updates.ConfigSettingEventArgs> ObserveConfigSetting => _observeConfigSetting;

        public IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult =>
            _observeConfigResult;

        public IObservable<Updates.DirectoryFileAddEventArgs> ObserveDirectoryFileAdd => _observeDirectoryFileAdd;

        public IObservable<Updates.DirectoryPrepareEventArgs> ObserveDirectoryPrepare => _observeDirectoryPrepare;

        public IObservable<Updates.PlayerStateEventArgs> ObservePlayerState => _observePlayerState;

        public IObservable<Updates.MarkerEventArgs> ObserveMarker => _observeMarker;

        public IObservable<Updates.TrackLoadEventArgs> ObserveTrackLoad => _observeTrackLoad;

        public IObservable<Updates.TrackAddEventArgs> ObserveTrackAdd => _observeTrackAdd;

        public IObservable<Updates.TrackDeleteEventArgs> ObserveTrackDelete => _observeTrackDelete;

        public IObservable<Updates.TrackMoveEventArgs> ObserveTrackMove => _observeTrackMove;

        public IObservable<Updates.PlaylistResetEventArgs> ObservePlaylistReset => _observePlaylistReset;

        public IObservable<Updates.ErrorEventArgs> ObserveError => _observeError;

        public IObservable<bool> ObserveServerQuit => _observeServerQuit;

        public IObservable<Receiver.VersionInfo> ObserveVersion => _observeVersion;

        public IObservable<(Command cmdReceived, string ipAddress, uint mask)> ObserveIpRestriction =>
            _observeIpRestriction;

        public IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult =>
            _observeLibraryResult;

        public IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult =>
            _observeListingResult;

        public IObservable<(uint permissionCode, string description)> ObservePermission => _observePermission;

        public IObservable<(uint showID, string description)> ObserveShowResult => _observeShowResult;

        public IObservable<Updates.TextSettingEventArgs> ObserveTextSetting => _observeTextSetting;

        public IObservable<(Command command, string description)> ObserveUnknownCommand => _observeUnknownCommand;

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