using System;
using System.Reactive.Linq;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     Listens on an <see cref="ISource" /> for incoming BapsNet commands, decodes them, and sends server update
    ///     events.
    /// </summary>
    public class Receiver : IServerUpdater
    {
        /// <summary>
        ///     The BapsNet connection on which we're receiving commands.
        /// </summary>
        [NotNull] private readonly ISource _bapsNet;

        private readonly CancellationToken _token;
        private IObservable<ConfigChoiceArgs>? _observeConfigChoice;
        private IObservable<ConfigOptionArgs>? _observeConfigOption;
        private IObservable<ConfigResultArgs>? _observeConfigResult;
        private IObservable<ConfigSettingArgs>? _observeConfigSetting;
        private IObservable<DirectoryFileAddArgs>? _observeDirectoryFileAdd;
        private IObservable<DirectoryPrepareEventArgs>? _observeDirectoryPrepare;
        private IObservable<ErrorEventArgs>? _observeError;
        private IObservable<CountEventArgs>? _observeIncomingCount;
        private IObservable<IpRestriction>? _observeIpRestriction;
        private IObservable<(uint resultID, byte dirtyStatus, string description)>? _observeLibraryResult;
        private IObservable<(uint listingID, uint channelID, string description)>? _observeListingResult;
        private IObservable<MarkerChangeArgs>? _observeMarker;
        private IObservable<(uint permissionCode, string description)>? _observePermission;

        private IObservable<PlaybackStateChangeArgs>? _observePlayerState;
        private IObservable<PlaylistResetEventArgs>? _observePlaylistReset;
        private IObservable<bool>? _observeServerQuit;
        private IObservable<(uint showID, string description)>? _observeShowResult;
        private IObservable<TextSettingEventArgs>? _observeTextSetting;
        private IObservable<TrackAddEventArgs>? _observeTrackAdd;
        private IObservable<TrackDeleteEventArgs>? _observeTrackDelete;
        private IObservable<TrackLoadArgs>? _observeTrackLoad;
        private IObservable<TrackMoveEventArgs>? _observeTrackMove;
        private IObservable<(CommandWord command, string description)>? _observeUnknownCommand;
        private IObservable<(string username, uint permissions)>? _observeUser;
        private IObservable<(byte resultCode, string description)>? _observeUserResult;
        private IObservable<ServerVersion>? _observeServerVersion;

        /// <summary>
        ///     The decoder used to receive and process the bodies of BapsNet messages.
        /// </summary>
        private readonly CommandDecoder _decoder;
        
        public Receiver(ISource? bapsNet, CancellationToken token)
        {
            _bapsNet = bapsNet ?? throw new ArgumentNullException(nameof(bapsNet));
            _token = token;
            // TODO(@MattWindsor91): inject this dependency.
            _decoder = new CommandDecoder(this, _bapsNet, _token);
        }

        public IObservable<PlaybackStateChangeArgs> ObservePlayerState =>
            _observePlayerState ??= Observable.FromEventPattern<PlaybackStateChangeArgs>(
                ev => PlayerState += ev,
                ev => PlayerState -= ev
            ).Select(x => x.EventArgs);

        public IObservable<MarkerChangeArgs> ObserveMarker =>
            _observeMarker ??= Observable.FromEventPattern<MarkerChangeArgs>(
                ev => Marker += ev,
                ev => Marker -= ev
            ).Select(x => x.EventArgs);

        public IObservable<TrackLoadArgs> ObserveTrackLoad =>
            _observeTrackLoad ??= Observable.FromEventPattern<TrackLoadArgs>(
                ev => TrackLoad += ev,
                ev => TrackLoad -= ev
            ).Select(x => x.EventArgs);

        public IObservable<CountEventArgs> ObserveIncomingCount =>
            _observeIncomingCount ??= Observable.FromEventPattern<CountEventArgs>(
                ev => IncomingCount += ev,
                ev => IncomingCount -= ev
            ).Select(x => x.EventArgs);

        public IObservable<ConfigChoiceArgs> ObserveConfigChoice =>
            _observeConfigChoice ??= Observable.FromEventPattern<ConfigChoiceArgs>(
                ev => ConfigChoice += ev,
                ev => ConfigChoice -= ev
            ).Select(x => x.EventArgs);

        public IObservable<ConfigOptionArgs> ObserveConfigOption =>
            _observeConfigOption ??= Observable.FromEventPattern<ConfigOptionArgs>(
                ev => ConfigOption += ev,
                ev => ConfigOption -= ev
            ).Select(x => x.EventArgs);

        public IObservable<ConfigSettingArgs> ObserveConfigSetting =>
            _observeConfigSetting ??= Observable.FromEventPattern<ConfigSettingArgs>(
                ev => ConfigSetting += ev,
                ev => ConfigSetting -= ev
            ).Select(x => x.EventArgs);

        public IObservable<ConfigResultArgs> ObserveConfigResult =>
            _observeConfigResult ??= Observable
                .FromEventPattern<ConfigResultArgs>(
                    ev => ConfigResult += ev,
                    ev => ConfigResult -= ev
                ).Select(x => x.EventArgs);

        public IObservable<DirectoryFileAddArgs> ObserveDirectoryFileAdd =>
            _observeDirectoryFileAdd ??= Observable.FromEventPattern<DirectoryFileAddArgs>(
                ev => DirectoryFileAdd += ev,
                ev => DirectoryFileAdd -= ev
            ).Select(x => x.EventArgs);

        public IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare =>
            _observeDirectoryPrepare ??= Observable.FromEventPattern<DirectoryPrepareEventArgs>(
                ev => DirectoryPrepare += ev,
                ev => DirectoryPrepare -= ev
            ).Select(x => x.EventArgs);

        public IObservable<TrackAddEventArgs> ObserveTrackAdd =>
            _observeTrackAdd ??= Observable.FromEventPattern<TrackAddEventArgs>(
                ev => TrackAdd += ev,
                ev => TrackAdd -= ev
            ).Select(x => x.EventArgs);

        public IObservable<TrackDeleteEventArgs> ObserveTrackDelete =>
            _observeTrackDelete ??= Observable.FromEventPattern<TrackDeleteEventArgs>(
                ev => TrackDelete += ev,
                ev => TrackDelete -= ev
            ).Select(x => x.EventArgs);

        public IObservable<TrackMoveEventArgs> ObserveTrackMove =>
            _observeTrackMove ??= Observable.FromEventPattern<TrackMoveEventArgs>(
                ev => TrackMove += ev,
                ev => TrackMove -= ev
            ).Select(x => x.EventArgs);

        public IObservable<PlaylistResetEventArgs> ObservePlaylistReset =>
            _observePlaylistReset ??= Observable.FromEventPattern<PlaylistResetEventArgs>(
                ev => PlaylistReset += ev,
                ev => PlaylistReset -= ev
            ).Select(x => x.EventArgs);

        public IObservable<ErrorEventArgs> ObserveError =>
            _observeError ??= Observable.FromEventPattern<ErrorEventArgs>(
                ev => Error += ev,
                ev => Error -= ev
            ).Select(x => x.EventArgs);

        public IObservable<bool> ObserveServerQuit =>
            _observeServerQuit ??= Observable.FromEventPattern<bool>(
                ev => ServerQuit += ev,
                ev => ServerQuit -= ev
            ).Select(x => x.EventArgs);

        public IObservable<ServerVersion> ObserveServerVersion =>
            _observeServerVersion ??= Observable.FromEventPattern<ServerVersion>(
                ev => Version += ev,
                ev => Version -= ev
            ).Select(x => x.EventArgs);

        public IObservable<IpRestriction> ObserveIpRestriction =>
            _observeIpRestriction ??= Observable.FromEventPattern<IpRestriction>(
                ev => IpRestriction += ev,
                ev => IpRestriction -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult =>
            _observeLibraryResult ??= Observable.FromEventPattern<(uint resultID, byte dirtyStatus, string description)>(
                ev => LibraryResult += ev,
                ev => LibraryResult -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult =>
            _observeListingResult ??= Observable.FromEventPattern<(uint listingID, uint channelID, string description)>(
                ev => ListingResult += ev,
                ev => ListingResult -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(uint permissionCode, string description)> ObservePermission =>
            _observePermission ??= Observable.FromEventPattern<(uint permissionCode, string description)>(
                ev => Permission += ev,
                ev => Permission -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(uint showID, string description)> ObserveShowResult =>
            _observeShowResult ??= Observable.FromEventPattern<(uint showID, string description)>(
                ev => ShowResult += ev,
                ev => ShowResult -= ev
            ).Select(x => x.EventArgs);

        public IObservable<TextSettingEventArgs> ObserveTextSetting =>
            _observeTextSetting ??= Observable.FromEventPattern<TextSettingEventArgs>(
                ev => TextSetting += ev,
                ev => TextSetting -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(CommandWord command, string description)> ObserveUnknownCommand =>
            _observeUnknownCommand ??= Observable.FromEventPattern<(CommandWord command, string description)>(
                ev => UnknownCommand += ev,
                ev => UnknownCommand -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(string username, uint permissions)> ObserveUser =>
            _observeUser ??= Observable.FromEventPattern<(string username, uint permissions)>(
                ev => User += ev,
                ev => User -= ev
            ).Select(x => x.EventArgs);

        public IObservable<(byte resultCode, string description)> ObserveUserResult =>
            _observeUserResult ??= Observable.FromEventPattern<(byte resultCode, string description)>(
                ev => UserResult += ev,
                ev => UserResult -= ev
            ).Select(x => x.EventArgs);

        public void Run()
        {
            while (!_token.IsCancellationRequested)
            {
                var cmdReceived = _bapsNet.ReceiveCommand();
                DecodeCommand(cmdReceived);
            }

            _token.ThrowIfCancellationRequested();
        }
        
        #region Playback events

        public event EventHandler<PlaybackStateChangeArgs> PlayerState;

        public void OnPlaybackStateChange(PlaybackStateChangeArgs e)
        {
            PlayerState?.Invoke(this, e);
        }

        public event EventHandler<TrackLoadArgs> TrackLoad;

        public void OnLoadedItem(TrackLoadArgs args)
        {
            TrackLoad?.Invoke(this, args);
        }

        public event EventHandler<MarkerChangeArgs> Marker;

        public void OnMarkerChange(MarkerChangeArgs e)
        {
            Marker?.Invoke(this, e);
        }

        #endregion Playback events

        #region Playlist events

        public event EventHandler<TrackAddEventArgs> TrackAdd;

        public void OnItemAdd(TrackAddEventArgs e)
        {
            TrackAdd?.Invoke(this, e);
        }

        public event EventHandler<TrackMoveEventArgs> TrackMove;

        public void OnItemMove(TrackMoveEventArgs e)
        {
            TrackMove?.Invoke(this, e);
        }

        public event EventHandler<TrackDeleteEventArgs> TrackDelete;

        public void OnItemDelete(TrackDeleteEventArgs e)
        {
            TrackDelete?.Invoke(this, e);
        }

        public event EventHandler<PlaylistResetEventArgs> PlaylistReset;

        public void OnResetPlaylist(PlaylistResetEventArgs e)
        {
            PlaylistReset?.Invoke(this, e);
        }

        #endregion Playlist events

        #region Database events

        public event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;

        public void OnLibraryResult(uint resultId, byte dirtyStatus, string description)
        {
            LibraryResult?.Invoke(this, (resultID: resultId, dirtyStatus, description));
        }

        public event EventHandler<(uint showID, string description)> ShowResult;

        public void OnShowResult(uint showId, string description)
        {
            ShowResult?.Invoke(this, (showID: showId, description));
        }

        public event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;

        public void OnListingResult(uint listingId, uint channelId, string description)
        {
            ListingResult?.Invoke(this, (listingID: listingId, channelID: channelId, description));
        }

        #endregion Database events

        #region Config events

        public event EventHandler<ConfigOptionArgs> ConfigOption;

        public void OnConfigOption(ConfigOptionArgs optionArgs)
        {
            ConfigOption?.Invoke(this, optionArgs);
        }

        public event EventHandler<ConfigChoiceArgs> ConfigChoice;

        public void OnConfigChoice(ConfigChoiceArgs args)
        {
            ConfigChoice?.Invoke(this, args);
        }

        public event EventHandler<ConfigSettingArgs> ConfigSetting;

        public void OnConfigSetting(ConfigSettingArgs args)
        {
            ConfigSetting?.Invoke(this, args);
        }

        public event EventHandler<ConfigResultArgs> ConfigResult;

        public void OnConfigResult(ConfigResultArgs args)
        {
            ConfigResult?.Invoke(this, args);
        }

        public event EventHandler<(string username, uint permissions)> User;

        public void OnUser(string username, uint permissions)
        {
            User?.Invoke(this, (username, permissions));
        }

        public event EventHandler<(uint permissionCode, string description)> Permission;

        public void OnPermission(uint permissionCode, string description)
        {
            Permission?.Invoke(this, (permissionCode, description));
        }

        public event EventHandler<(byte resultCode, string description)> UserResult;

        public void OnUserResult(byte resultCode, string description)
        {
            UserResult?.Invoke(this, (resultCode, description));
        }

        public event EventHandler<IpRestriction> IpRestriction;

        public void OnIpRestriction(IpRestriction r)
        {
            IpRestriction?.Invoke(this, r);
        }

        #endregion Config events

        #region System events

        public event EventHandler<DirectoryFileAddArgs> DirectoryFileAdd;

        public void OnDirectoryFileAdd(DirectoryFileAddArgs args)
        {
            DirectoryFileAdd?.Invoke(this, args);
        }

        public event EventHandler<DirectoryPrepareEventArgs> DirectoryPrepare;

        public void OnDirectoryPrepare(DirectoryPrepareEventArgs args)
        {
            DirectoryPrepare?.Invoke(this, args);
        }

        public event EventHandler<ServerVersion> Version;

        public void OnVersion(ServerVersion v)
        {
            Version?.Invoke(this, v);
        }

        public event EventHandler<TextSettingEventArgs> TextSetting;

        public void OnTextSetting(TextSettingEventArgs args)
        {
            TextSetting?.Invoke(this, args);
        }


        public event EventHandler<bool> ServerQuit;

        public void OnServerQuit(bool expected)
        {
            ServerQuit?.Invoke(this, expected);
        }

        #endregion System events

        #region General events

        public event EventHandler<CountEventArgs> IncomingCount;

        public void OnIncomingCount(CountEventArgs e)
        {
            IncomingCount?.Invoke(this, e);
        }

        public event EventHandler<ErrorEventArgs> Error;

        public void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event EventHandler<(CommandWord command, string description)> UnknownCommand;

        public void OnUnknownCommand(CommandWord commandWord, string description)
        {
            UnknownCommand?.Invoke(this, (commandWord, description));
        }

        #endregion General events

        private void DecodeCommand(CommandWord word)
        {
            _ /* length */ = _bapsNet.ReceiveUint();
            var cmd = word.Unpack();
            cmd.Accept(_decoder);
        }
    }
}