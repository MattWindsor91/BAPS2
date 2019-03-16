using System;
using System.Reactive.Linq;
using System.Threading;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     Listens on a <see cref="ClientSocket" /> for incoming BapsNet commands, decodes them, and sends server update
    ///     events.
    /// </summary>
    public class Receiver : IServerUpdater
    {
        private readonly ClientSocket _cs;
        private readonly CancellationToken _token;
        private IObservable<ConfigChoiceEventArgs> _observeConfigChoice;
        private IObservable<ConfigOptionEventArgs> _observeConfigOption;
        private IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> _observeConfigResult;
        private IObservable<ConfigSettingEventArgs> _observeConfigSetting;
        private IObservable<DirectoryFileAddEventArgs> _observeDirectoryFileAdd;
        private IObservable<DirectoryPrepareEventArgs> _observeDirectoryPrepare;
        private IObservable<ErrorEventArgs> _observeError;
        private IObservable<CountEventArgs> _observeIncomingCount;
        private IObservable<(Command cmdReceived, string ipAddress, uint mask)> _observeIpRestriction;
        private IObservable<(uint resultID, byte dirtyStatus, string description)> _observeLibraryResult;
        private IObservable<(uint listingID, uint channelID, string description)> _observeListingResult;
        private IObservable<MarkerEventArgs> _observeMarker;
        private IObservable<(uint permissionCode, string description)> _observePermission;

        private IObservable<PlayerStateEventArgs> _observePlayerState;
        private IObservable<PlaylistResetEventArgs> _observePlaylistReset;
        private IObservable<bool> _observeServerQuit;
        private IObservable<(uint showID, string description)> _observeShowResult;
        private IObservable<TextSettingEventArgs> _observeTextSetting;
        private IObservable<TrackAddEventArgs> _observeTrackAdd;
        private IObservable<TrackDeleteEventArgs> _observeTrackDelete;
        private IObservable<TrackLoadEventArgs> _observeTrackLoad;
        private IObservable<TrackMoveEventArgs> _observeTrackMove;
        private IObservable<(Command command, string description)> _observeUnknownCommand;
        private IObservable<(string username, uint permissions)> _observeUser;
        private IObservable<(byte resultCode, string description)> _observeUserResult;
        private IObservable<VersionInfo> _observeVersion;

        public Receiver(ClientSocket cs, CancellationToken token)
        {
            _cs = cs;
            _token = token;
        }

        public IObservable<PlayerStateEventArgs> ObservePlayerState =>
            _observePlayerState ??
            (_observePlayerState = Observable.FromEventPattern<PlayerStateEventArgs>(
                ev => PlayerState += ev,
                ev => PlayerState -= ev
            ).Select(x => x.EventArgs));

        public IObservable<MarkerEventArgs> ObserveMarker =>
            _observeMarker ??
            (_observeMarker = Observable.FromEventPattern<MarkerEventArgs>(
                ev => Marker += ev,
                ev => Marker -= ev
            ).Select(x => x.EventArgs));

        public IObservable<TrackLoadEventArgs> ObserveTrackLoad =>
            _observeTrackLoad ??
            (_observeTrackLoad = Observable.FromEventPattern<TrackLoadEventArgs>(
                ev => TrackLoad += ev,
                ev => TrackLoad -= ev
            ).Select(x => x.EventArgs));

        public IObservable<CountEventArgs> ObserveIncomingCount =>
            _observeIncomingCount ??
            (_observeIncomingCount = Observable.FromEventPattern<CountEventArgs>(
                ev => IncomingCount += ev,
                ev => IncomingCount -= ev
            ).Select(x => x.EventArgs));

        public IObservable<ConfigChoiceEventArgs> ObserveConfigChoice =>
            _observeConfigChoice ??
            (_observeConfigChoice = Observable.FromEventPattern<ConfigChoiceEventArgs>(
                ev => ConfigChoice += ev,
                ev => ConfigChoice -= ev
            ).Select(x => x.EventArgs));

        public IObservable<ConfigOptionEventArgs> ObserveConfigOption =>
            _observeConfigOption ??
            (_observeConfigOption = Observable.FromEventPattern<ConfigOptionEventArgs>(
                ev => ConfigOption += ev,
                ev => ConfigOption -= ev
            ).Select(x => x.EventArgs));

        public IObservable<ConfigSettingEventArgs> ObserveConfigSetting =>
            _observeConfigSetting ??
            (_observeConfigSetting = Observable.FromEventPattern<ConfigSettingEventArgs>(
                ev => ConfigSetting += ev,
                ev => ConfigSetting -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult =>
            _observeConfigResult ??
            (_observeConfigResult = Observable
                .FromEventPattern<(Command cmdReceived, uint optionID, ConfigResult result)>(
                    ev => ConfigResult += ev,
                    ev => ConfigResult -= ev
                ).Select(x => x.EventArgs));

        public IObservable<DirectoryFileAddEventArgs> ObserveDirectoryFileAdd =>
            _observeDirectoryFileAdd ??
            (_observeDirectoryFileAdd = Observable.FromEventPattern<DirectoryFileAddEventArgs>(
                ev => DirectoryFileAdd += ev,
                ev => DirectoryFileAdd -= ev
            ).Select(x => x.EventArgs));

        public IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare =>
            _observeDirectoryPrepare ??
            (_observeDirectoryPrepare = Observable.FromEventPattern<DirectoryPrepareEventArgs>(
                ev => DirectoryPrepare += ev,
                ev => DirectoryPrepare -= ev
            ).Select(x => x.EventArgs));

        public IObservable<TrackAddEventArgs> ObserveTrackAdd =>
            _observeTrackAdd ??
            (_observeTrackAdd = Observable.FromEventPattern<TrackAddEventArgs>(
                ev => TrackAdd += ev,
                ev => TrackAdd -= ev
            ).Select(x => x.EventArgs));

        public IObservable<TrackDeleteEventArgs> ObserveTrackDelete =>
            _observeTrackDelete ??
            (_observeTrackDelete = Observable.FromEventPattern<TrackDeleteEventArgs>(
                ev => TrackDelete += ev,
                ev => TrackDelete -= ev
            ).Select(x => x.EventArgs));

        public IObservable<TrackMoveEventArgs> ObserveTrackMove =>
            _observeTrackMove ??
            (_observeTrackMove = Observable.FromEventPattern<TrackMoveEventArgs>(
                ev => TrackMove += ev,
                ev => TrackMove -= ev
            ).Select(x => x.EventArgs));

        public IObservable<PlaylistResetEventArgs> ObservePlaylistReset =>
            _observePlaylistReset ??
            (_observePlaylistReset = Observable.FromEventPattern<PlaylistResetEventArgs>(
                ev => PlaylistReset += ev,
                ev => PlaylistReset -= ev
            ).Select(x => x.EventArgs));

        public IObservable<ErrorEventArgs> ObserveError =>
            _observeError ??
            (_observeError = Observable.FromEventPattern<ErrorEventArgs>(
                ev => Error += ev,
                ev => Error -= ev
            ).Select(x => x.EventArgs));

        public IObservable<bool> ObserveServerQuit =>
            _observeServerQuit ??
            (_observeServerQuit = Observable.FromEventPattern<bool>(
                ev => ServerQuit += ev,
                ev => ServerQuit -= ev
            ).Select(x => x.EventArgs));

        public IObservable<VersionInfo> ObserveVersion =>
            _observeVersion ??
            (_observeVersion = Observable.FromEventPattern<VersionInfo>(
                ev => Version += ev,
                ev => Version -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(Command cmdReceived, string ipAddress, uint mask)> ObserveIpRestriction =>
            _observeIpRestriction ??
            (_observeIpRestriction = Observable.FromEventPattern<(Command cmdReceived, string ipAddress, uint mask)>(
                ev => IpRestriction += ev,
                ev => IpRestriction -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult =>
            _observeLibraryResult ??
            (_observeLibraryResult = Observable.FromEventPattern<(uint resultID, byte dirtyStatus, string description)>(
                ev => LibraryResult += ev,
                ev => LibraryResult -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult =>
            _observeListingResult ??
            (_observeListingResult = Observable.FromEventPattern<(uint listingID, uint channelID, string description)>(
                ev => ListingResult += ev,
                ev => ListingResult -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(uint permissionCode, string description)> ObservePermission =>
            _observePermission ??
            (_observePermission = Observable.FromEventPattern<(uint permissionCode, string description)>(
                ev => Permission += ev,
                ev => Permission -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(uint showID, string description)> ObserveShowResult =>
            _observeShowResult ??
            (_observeShowResult = Observable.FromEventPattern<(uint showID, string description)>(
                ev => ShowResult += ev,
                ev => ShowResult -= ev
            ).Select(x => x.EventArgs));

        public IObservable<TextSettingEventArgs> ObserveTextSetting =>
            _observeTextSetting ??
            (_observeTextSetting = Observable.FromEventPattern<TextSettingEventArgs>(
                ev => TextSetting += ev,
                ev => TextSetting -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(Command command, string description)> ObserveUnknownCommand =>
            _observeUnknownCommand ??
            (_observeUnknownCommand = Observable.FromEventPattern<(Command command, string description)>(
                ev => UnknownCommand += ev,
                ev => UnknownCommand -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(string username, uint permissions)> ObserveUser =>
            _observeUser ??
            (_observeUser = Observable.FromEventPattern<(string username, uint permissions)>(
                ev => User += ev,
                ev => User -= ev
            ).Select(x => x.EventArgs));

        public IObservable<(byte resultCode, string description)> ObserveUserResult =>
            _observeUserResult ??
            (_observeUserResult = Observable.FromEventPattern<(byte resultCode, string description)>(
                ev => UserResult += ev,
                ev => UserResult -= ev
            ).Select(x => x.EventArgs));

        public void Run()
        {
            while (!_token.IsCancellationRequested)
            {
                var cmdReceived = _cs.ReceiveC();
                DecodeCommand(cmdReceived);
            }

            _token.ThrowIfCancellationRequested();
        }


        public struct VersionInfo
        {
            public string Version;
            public string Date;
            public string Time;
            public string Author;
        }

        #region Playback events

        public event EventHandler<PlayerStateEventArgs> PlayerState;

        private void OnChannelOperation(PlayerStateEventArgs e)
        {
            PlayerState?.Invoke(this, e);
        }

        public event EventHandler<TrackLoadEventArgs> TrackLoad;

        private void OnLoadedItem(TrackLoadEventArgs args)
        {
            TrackLoad?.Invoke(this, args);
        }

        public event EventHandler<MarkerEventArgs> Marker;

        private void OnMarker(MarkerEventArgs e)
        {
            Marker?.Invoke(this, e);
        }

        #endregion Playback events

        #region Playlist events

        public event EventHandler<TrackAddEventArgs> TrackAdd;

        private void OnItemAdd(TrackAddEventArgs e)
        {
            TrackAdd?.Invoke(this, e);
        }

        public event EventHandler<TrackMoveEventArgs> TrackMove;

        private void OnItemMove(TrackMoveEventArgs e)
        {
            TrackMove?.Invoke(this, e);
        }

        public event EventHandler<TrackDeleteEventArgs> TrackDelete;

        private void OnItemDelete(TrackDeleteEventArgs e)
        {
            TrackDelete?.Invoke(this, e);
        }

        public event EventHandler<PlaylistResetEventArgs> PlaylistReset;

        private void OnResetPlaylist(PlaylistResetEventArgs e)
        {
            PlaylistReset?.Invoke(this, e);
        }

        #endregion Playlist events

        #region Database events

        public event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;

        private void OnLibraryResult(uint resultId, byte dirtyStatus, string description)
        {
            LibraryResult?.Invoke(this, (resultID: resultId, dirtyStatus, description));
        }

        public event EventHandler<(uint showID, string description)> ShowResult;

        private void OnShowResult(uint showId, string description)
        {
            ShowResult?.Invoke(this, (showID: showId, description));
        }

        public event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;

        private void OnListingResult(uint listingId, uint channelId, string description)
        {
            ListingResult?.Invoke(this, (listingID: listingId, channelID: channelId, description));
        }

        #endregion Database events

        #region Config events

        public event EventHandler<ConfigOptionEventArgs> ConfigOption;

        private void OnConfigOption(ConfigOptionEventArgs args)
        {
            ConfigOption?.Invoke(this, args);
        }

        public event EventHandler<ConfigChoiceEventArgs> ConfigChoice;

        private void OnConfigChoice(ConfigChoiceEventArgs args)
        {
            ConfigChoice?.Invoke(this, args);
        }

        public event EventHandler<ConfigSettingEventArgs> ConfigSetting;

        private void OnConfigSetting(ConfigSettingEventArgs args)
        {
            ConfigSetting?.Invoke(this, args);
        }

        public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;

        private void OnConfigResult(Command cmdReceived, uint optionId, ConfigResult result)
        {
            ConfigResult?.Invoke(this, (cmdReceived, optionID: optionId, result));
        }

        public event EventHandler<(string username, uint permissions)> User;

        private void OnUser(string username, uint permissions)
        {
            User?.Invoke(this, (username, permissions));
        }

        public event EventHandler<(uint permissionCode, string description)> Permission;

        private void OnPermission(uint permissionCode, string description)
        {
            Permission?.Invoke(this, (permissionCode, description));
        }

        public event EventHandler<(byte resultCode, string description)> UserResult;

        private void OnUserResult(byte resultCode, string description)
        {
            UserResult?.Invoke(this, (resultCode, description));
        }

        public event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IpRestriction;

        private void OnIpRestriction(Command cmdReceived, string ipAddress, uint mask)
        {
            IpRestriction?.Invoke(this, (cmdReceived, ipAddress, mask));
        }

        #endregion Config events

        #region System events

        public event EventHandler<DirectoryFileAddEventArgs> DirectoryFileAdd;

        private void OnDirectoryFileAdd(DirectoryFileAddEventArgs args)
        {
            DirectoryFileAdd?.Invoke(this, args);
        }

        public event EventHandler<DirectoryPrepareEventArgs> DirectoryPrepare;

        private void OnDirectoryPrepare(DirectoryPrepareEventArgs args)
        {
            DirectoryPrepare?.Invoke(this, args);
        }

        public event EventHandler<VersionInfo> Version;

        private void OnVersion(VersionInfo v)
        {
            Version?.Invoke(this, v);
        }

        public event EventHandler<TextSettingEventArgs> TextSetting;

        private void OnTextSetting(TextSettingEventArgs args)
        {
            TextSetting?.Invoke(this, args);
        }


        public event EventHandler<bool> ServerQuit;

        private void OnServerQuit(bool expected)
        {
            ServerQuit?.Invoke(this, expected);
        }

        #endregion System events

        #region General events

        public event EventHandler<CountEventArgs> IncomingCount;

        private void OnIncomingCount(CountEventArgs e)
        {
            IncomingCount?.Invoke(this, e);
        }

        public event EventHandler<ErrorEventArgs> Error;

        private void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event EventHandler<(Command command, string description)> UnknownCommand;

        private void OnUnknownCommand(Command command, string description)
        {
            UnknownCommand?.Invoke(this, (command, description));
        }

        #endregion General events

        #region Command decoding

        private void DecodeCommand(Command cmdReceived)
        {
            _ /* length */ = _cs.ReceiveI();
            switch (cmdReceived & Command.GroupMask)
            {
                case Command.Playback:
                    DecodePlaybackCommand(cmdReceived);
                    break;
                case Command.Playlist:
                    DecodePlaylistCommand(cmdReceived);
                    break;
                case Command.Database:
                    DecodeDatabaseCommand(cmdReceived);
                    break;
                case Command.Config:
                    DecodeConfigCommand(cmdReceived);
                    break;
                case Command.System:
                    DecodeSystemCommand(cmdReceived);
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "unknown group");
                    break;
            }
        }

        private void DecodePlaybackCommand(Command cmdReceived)
        {
            var op = cmdReceived & Command.PlaybackOpMask;
            switch (op)
            {
                case Command.Play:
                case Command.Pause:
                case Command.Stop:
                {
                    OnChannelOperation(new PlayerStateEventArgs(cmdReceived.Channel(),
                        cmdReceived.AsPlaybackState()));
                }
                    break;
                case Command.Volume:
                {
                    // Deliberately ignore
                    _ = _cs.ReceiveF();
                }
                    break;
                case Command.Load:
                {
                    DecodeLoad(cmdReceived.Channel());
                }
                    break;
                case Command.Position:
                case Command.CuePosition:
                case Command.IntroPosition:
                {
                    var position = _cs.ReceiveI();
                    OnMarker(new MarkerEventArgs(cmdReceived.Channel(), op.AsMarkerType(), position));
                }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed PLAYBACK");
                    break;
            }
        }

        private void DecodeLoad(ushort channelId)
        {
            var index = _cs.ReceiveI();
            var type = (TrackType) _cs.ReceiveI();
            var description = _cs.ReceiveS();

            var duration = 0U;
            if (type.HasAudio()) duration = _cs.ReceiveI();

            var text = "";
            if (type.HasText()) text = _cs.ReceiveS();

            var track = TrackFactory.Create(type, description, duration, text);

            OnMarker(new MarkerEventArgs(channelId, MarkerType.Position, 0U));
            OnLoadedItem(new TrackLoadEventArgs(channelId, index, track));
        }

        private void DecodePlaylistCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.PlaylistOpMask)
            {
                case Command.Item:
                    if (cmdReceived.HasFlag(Command.PlaylistModeMask))
                    {
                        var channelId = cmdReceived.Channel();
                        var index = _cs.ReceiveI();
                        var type = (TrackType) _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        var entry = TrackFactory.Create(type, description);
                        OnItemAdd(new TrackAddEventArgs(channelId, index, entry));
                    }
                    else
                    {
                        // Deliberately ignore?
                        _ = _cs.ReceiveI();
                    }

                    break;
                case Command.MoveItemTo:
                {
                    var channelId = cmdReceived.Channel();
                    var indexFrom = _cs.ReceiveI();
                    var indexTo = _cs.ReceiveI();
                    OnItemMove(new TrackMoveEventArgs(channelId, indexFrom, indexTo));
                }
                    break;
                case Command.DeleteItem:
                {
                    var channelId = cmdReceived.Channel();
                    var index = _cs.ReceiveI();
                    OnItemDelete(new TrackDeleteEventArgs(channelId, index));
                }
                    break;
                case Command.ResetPlaylist:
                {
                    var channelId = cmdReceived.Channel();
                    OnResetPlaylist(new PlaylistResetEventArgs(channelId));
                }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed PLAYLIST");
                    break;
            }
        }

        private void DecodeDatabaseCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.DatabaseOpMask)
            {
                case Command.LibraryResult:
                {
                    if (cmdReceived.HasFlag(Command.DatabaseModeMask))
                    {
                        var dirtyStatus = cmdReceived.DatabaseValue();
                        var resultId = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnLibraryResult(resultId, dirtyStatus, description);
                    }
                    else
                    {
                        DecodeCount(CountType.LibraryItem);
                    }
                }
                    break;
                case Command.LibraryError:
                {
                    var errorCode = cmdReceived.DatabaseValue();
                    DecodeError(ErrorType.Library, errorCode);
                }
                    break;
                case Command.Show:
                    if (cmdReceived.HasFlag(Command.DatabaseModeMask))
                    {
                        var showId = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnShowResult(showId, description);
                    }
                    else
                    {
                        DecodeCount(CountType.Show);
                    }

                    break;
                case Command.Listing:
                    if (cmdReceived.HasFlag(Command.DatabaseModeMask))
                    {
                        var listingId = _cs.ReceiveI();
                        var channelId = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnListingResult(listingId, channelId, description);
                    }
                    else
                    {
                        DecodeCount(CountType.Listing);
                    }

                    break;
                case Command.BapsDbError:
                    DecodeError(ErrorType.BapsDb, cmdReceived.DatabaseValue());
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed DATABASE");
                    break;
            }
        }

        private void DecodeConfigCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.ConfigOpMask)
            {
                case Command.Option:
                {
                    if (cmdReceived.HasFlag(Command.ConfigModeMask))
                    {
                        var hasIndex = cmdReceived.HasFlag(Command.ConfigUseValueMask);
                        var index = cmdReceived.ConfigValue();
                        var optionId = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        var type = _cs.ReceiveI();
                        OnConfigOption(new ConfigOptionEventArgs(optionId, (ConfigType) type, description,
                            hasIndex,
                            index));
                    }
                    else
                    {
                        DecodeCount(CountType.ConfigOption);
                    }
                }
                    break;
                case Command.OptionChoice:
                {
                    var optionId = _cs.ReceiveI();
                    if (cmdReceived.HasFlag(Command.ConfigModeMask))
                    {
                        var choiceIndex = _cs.ReceiveI();
                        var choiceDescription = _cs.ReceiveS();
                        OnConfigChoice(new ConfigChoiceEventArgs(optionId, choiceIndex, choiceDescription));
                    }
                    else
                    {
                        DecodeCount(CountType.ConfigChoice, optionId);
                    }
                }
                    break;
                case Command.ConfigSetting:
                {
                    if (cmdReceived.HasFlag(Command.ConfigModeMask))
                    {
                        var optionId = _cs.ReceiveI();
                        var type = _cs.ReceiveI();
                        DecodeConfigSetting(cmdReceived, optionId, (ConfigType) type);
                    }
                    else
                    {
                        _ = _cs.ReceiveI();
                    }
                }
                    break;
                case Command.ConfigResult:
                {
                    var optionId = _cs.ReceiveI();
                    var result = _cs.ReceiveI();
                    OnConfigResult(cmdReceived, optionId, (ConfigResult) result);
                }
                    break;
                case Command.ConfigError:
                    DecodeError(ErrorType.Config, cmdReceived.ConfigValue());
                    break;
                case Command.User:
                {
                    if (cmdReceived.HasFlag(Command.ConfigModeMask))
                    {
                        var username = _cs.ReceiveS();
                        var permissions = _cs.ReceiveI();
                        OnUser(username, permissions);
                    }
                    else
                    {
                        DecodeCount(CountType.User);
                    }
                }
                    break;
                case Command.Permission:
                {
                    if (cmdReceived.HasFlag(Command.ConfigModeMask))
                    {
                        var permissionCode = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnPermission(permissionCode, description);
                    }
                    else
                    {
                        DecodeCount(CountType.Permission);
                    }
                }
                    break;
                case Command.UserResult:
                {
                    var resultCode = cmdReceived.ConfigValue();
                    var description = _cs.ReceiveS();
                    OnUserResult(resultCode, description);
                }
                    break;
                case Command.IpRestriction:
                {
                    if (cmdReceived.HasFlag(Command.ConfigModeMask))
                    {
                        var ipAddress = _cs.ReceiveS();
                        var mask = _cs.ReceiveI();
                        OnIpRestriction(cmdReceived, ipAddress, mask);
                    }
                    else
                    {
                        DecodeCount(CountType.IpRestriction);
                    }
                }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed CONFIG");
                    break;
            }
        }

        private void DecodeConfigSetting(Command cmdReceived, uint optionId, ConfigType type)
        {
            object value;
            /** Determine what the final argument is going to be and retrieve it **/
            switch (type)
            {
                case ConfigType.Int:
                case ConfigType.Choice:
                    value = (int) _cs.ReceiveI();
                    break;
                case ConfigType.Str:
                    value = _cs.ReceiveS();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid type received");
            }

            /** Use index=-1 to represent a non indexed setting **/
            var index = -1;
            if (cmdReceived.HasFlag(Command.ConfigUseValueMask)) index = cmdReceived.ConfigValue();

            OnConfigSetting(new ConfigSettingEventArgs(optionId, type, value, index));
        }

        private void DecodeSystemCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.SystemOpMask)
            {
                case Command.SendLogMessage:
                    _cs.ReceiveS();
                    break;
                case Command.Filename:
                    if (cmdReceived.HasFlag(Command.SystemModeMask))
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        var index = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnDirectoryFileAdd(new DirectoryFileAddEventArgs(directoryIndex, index, description));
                    }
                    else
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        _ = _cs.ReceiveI();
                        var niceDirectoryName = _cs.ReceiveS();
                        OnDirectoryPrepare(new DirectoryPrepareEventArgs(directoryIndex, niceDirectoryName));
                    }

                    break;
                case Command.Version:
                {
                    var version = _cs.ReceiveS();
                    var date = _cs.ReceiveS();
                    var time = _cs.ReceiveS();
                    var author = _cs.ReceiveS();
                    OnVersion(new VersionInfo {Version = version, Date = date, Time = time, Author = author});
                }
                    break;
                case Command.Feedback:
                {
                    _ = _cs.ReceiveI();
                }
                    break;
                case Command.SendMessage:
                {
                    _ = _cs.ReceiveS();
                    _ = _cs.ReceiveS();
                    _ = _cs.ReceiveS();
                }
                    break;
                case Command.ClientChange:
                {
                    _ = _cs.ReceiveS();
                }
                    break;
                case Command.ScrollText:
                {
                    var upDown = cmdReceived.SystemValue() == 0 ? UpDown.Down : UpDown.Up;
                    OnTextSetting(new TextSettingEventArgs(Events.TextSetting.Scroll, upDown));
                }
                    break;
                case Command.TextSize:
                {
                    var upDown = cmdReceived.SystemValue() == 0 ? UpDown.Down : UpDown.Up;
                    OnTextSetting(new TextSettingEventArgs(Events.TextSetting.FontSize, upDown));
                }
                    break;
                case Command.Quit:
                {
                    //The server should send an int representing if this is an expected quit (0) or an exception error (1)."
                    var expected = _cs.ReceiveI() == 0;
                    OnServerQuit(expected);
                }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed SYSTEM");
                    break;
            }
        }

        private void DecodeCount(CountType type, uint extra = 0)
        {
            var count = _cs.ReceiveI();
            OnIncomingCount(new CountEventArgs {Count = count, Type = type, Extra = extra});
        }

        private void DecodeError(ErrorType type, byte errorCode)
        {
            var description = _cs.ReceiveS();
            OnError(new ErrorEventArgs {Type = type, Code = errorCode, Description = description});
        }

        #endregion Command decoding
    }
}