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
        private IObservable<Updates.ConfigChoiceEventArgs> _observeConfigChoice;
        private IObservable<Updates.ConfigOptionEventArgs> _observeConfigOption;
        private IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> _observeConfigResult;
        private IObservable<Updates.ConfigSettingEventArgs> _observeConfigSetting;
        private IObservable<Updates.DirectoryFileAddEventArgs> _observeDirectoryFileAdd;
        private IObservable<Updates.DirectoryPrepareEventArgs> _observeDirectoryPrepare;
        private IObservable<Updates.ErrorEventArgs> _observeError;
        private IObservable<Updates.CountEventArgs> _observeIncomingCount;
        private IObservable<(Command cmdReceived, string ipAddress, uint mask)> _observeIpRestriction;
        private IObservable<(uint resultID, byte dirtyStatus, string description)> _observeLibraryResult;
        private IObservable<(uint listingID, uint channelID, string description)> _observeListingResult;
        private IObservable<Updates.MarkerEventArgs> _observeMarker;
        private IObservable<(uint permissionCode, string description)> _observePermission;

        private IObservable<Updates.PlayerStateEventArgs> _observePlayerState;
        private IObservable<Updates.PlaylistResetEventArgs> _observePlaylistReset;
        private IObservable<bool> _observeServerQuit;
        private IObservable<(uint showID, string description)> _observeShowResult;
        private IObservable<Updates.TextSettingEventArgs> _observeTextSetting;
        private IObservable<Updates.TrackAddEventArgs> _observeTrackAdd;
        private IObservable<Updates.TrackDeleteEventArgs> _observeTrackDelete;
        private IObservable<Updates.TrackLoadEventArgs> _observeTrackLoad;
        private IObservable<Updates.TrackMoveEventArgs> _observeTrackMove;
        private IObservable<(Command command, string description)> _observeUnknownCommand;
        private IObservable<(string username, uint permissions)> _observeUser;
        private IObservable<(byte resultCode, string description)> _observeUserResult;
        private IObservable<VersionInfo> _observeVersion;

        public Receiver(ClientSocket cs, CancellationToken token)
        {
            _cs = cs;
            _token = token;
        }

        public IObservable<Updates.PlayerStateEventArgs> ObservePlayerState =>
            _observePlayerState ??
            (_observePlayerState = Observable.FromEventPattern<Updates.PlayerStateEventArgs>(
                ev => PlayerState += ev,
                ev => PlayerState -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.MarkerEventArgs> ObserveMarker =>
            _observeMarker ??
            (_observeMarker = Observable.FromEventPattern<Updates.MarkerEventArgs>(
                ev => Marker += ev,
                ev => Marker -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.TrackLoadEventArgs> ObserveTrackLoad =>
            _observeTrackLoad ??
            (_observeTrackLoad = Observable.FromEventPattern<Updates.TrackLoadEventArgs>(
                ev => TrackLoad += ev,
                ev => TrackLoad -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.CountEventArgs> ObserveIncomingCount =>
            _observeIncomingCount ??
            (_observeIncomingCount = Observable.FromEventPattern<Updates.CountEventArgs>(
                ev => IncomingCount += ev,
                ev => IncomingCount -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.ConfigChoiceEventArgs> ObserveConfigChoice =>
            _observeConfigChoice ??
            (_observeConfigChoice = Observable.FromEventPattern<Updates.ConfigChoiceEventArgs>(
                ev => ConfigChoice += ev,
                ev => ConfigChoice -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.ConfigOptionEventArgs> ObserveConfigOption =>
            _observeConfigOption ??
            (_observeConfigOption = Observable.FromEventPattern<Updates.ConfigOptionEventArgs>(
                ev => ConfigOption += ev,
                ev => ConfigOption -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.ConfigSettingEventArgs> ObserveConfigSetting =>
            _observeConfigSetting ??
            (_observeConfigSetting = Observable.FromEventPattern<Updates.ConfigSettingEventArgs>(
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

        public IObservable<Updates.DirectoryFileAddEventArgs> ObserveDirectoryFileAdd =>
            _observeDirectoryFileAdd ??
            (_observeDirectoryFileAdd = Observable.FromEventPattern<Updates.DirectoryFileAddEventArgs>(
                ev => DirectoryFileAdd += ev,
                ev => DirectoryFileAdd -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.DirectoryPrepareEventArgs> ObserveDirectoryPrepare =>
            _observeDirectoryPrepare ??
            (_observeDirectoryPrepare = Observable.FromEventPattern<Updates.DirectoryPrepareEventArgs>(
                ev => DirectoryPrepare += ev,
                ev => DirectoryPrepare -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.TrackAddEventArgs> ObserveTrackAdd =>
            _observeTrackAdd ??
            (_observeTrackAdd = Observable.FromEventPattern<Updates.TrackAddEventArgs>(
                ev => TrackAdd += ev,
                ev => TrackAdd -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.TrackDeleteEventArgs> ObserveTrackDelete =>
            _observeTrackDelete ??
            (_observeTrackDelete = Observable.FromEventPattern<Updates.TrackDeleteEventArgs>(
                ev => TrackDelete += ev,
                ev => TrackDelete -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.TrackMoveEventArgs> ObserveTrackMove =>
            _observeTrackMove ??
            (_observeTrackMove = Observable.FromEventPattern<Updates.TrackMoveEventArgs>(
                ev => TrackMove += ev,
                ev => TrackMove -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.PlaylistResetEventArgs> ObservePlaylistReset =>
            _observePlaylistReset ??
            (_observePlaylistReset = Observable.FromEventPattern<Updates.PlaylistResetEventArgs>(
                ev => PlaylistReset += ev,
                ev => PlaylistReset -= ev
            ).Select(x => x.EventArgs));

        public IObservable<Updates.ErrorEventArgs> ObserveError =>
            _observeError ??
            (_observeError = Observable.FromEventPattern<Updates.ErrorEventArgs>(
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

        public IObservable<Updates.TextSettingEventArgs> ObserveTextSetting =>
            _observeTextSetting ??
            (_observeTextSetting = Observable.FromEventPattern<Updates.TextSettingEventArgs>(
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

        public event EventHandler<Updates.PlayerStateEventArgs> PlayerState;

        private void OnChannelOperation(Updates.PlayerStateEventArgs e)
        {
            PlayerState?.Invoke(this, e);
        }

        public event EventHandler<Updates.TrackLoadEventArgs> TrackLoad;

        private void OnLoadedItem(Updates.TrackLoadEventArgs args)
        {
            TrackLoad?.Invoke(this, args);
        }

        public event EventHandler<Updates.MarkerEventArgs> Marker;

        private void OnMarker(Updates.MarkerEventArgs e)
        {
            Marker?.Invoke(this, e);
        }

        #endregion Playback events

        #region Playlist events

        public event EventHandler<Updates.TrackAddEventArgs> TrackAdd;

        private void OnItemAdd(Updates.TrackAddEventArgs e)
        {
            TrackAdd?.Invoke(this, e);
        }

        public event EventHandler<Updates.TrackMoveEventArgs> TrackMove;

        private void OnItemMove(Updates.TrackMoveEventArgs e)
        {
            TrackMove?.Invoke(this, e);
        }

        public event EventHandler<Updates.TrackDeleteEventArgs> TrackDelete;

        private void OnItemDelete(Updates.TrackDeleteEventArgs e)
        {
            TrackDelete?.Invoke(this, e);
        }

        public event EventHandler<Updates.PlaylistResetEventArgs> PlaylistReset;

        private void OnResetPlaylist(Updates.PlaylistResetEventArgs e)
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

        public event EventHandler<Updates.ConfigOptionEventArgs> ConfigOption;

        private void OnConfigOption(Updates.ConfigOptionEventArgs args)
        {
            ConfigOption?.Invoke(this, args);
        }

        public event EventHandler<Updates.ConfigChoiceEventArgs> ConfigChoice;

        private void OnConfigChoice(Updates.ConfigChoiceEventArgs args)
        {
            ConfigChoice?.Invoke(this, args);
        }

        public event EventHandler<Updates.ConfigSettingEventArgs> ConfigSetting;

        private void OnConfigSetting(Updates.ConfigSettingEventArgs args)
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

        public event EventHandler<Updates.DirectoryFileAddEventArgs> DirectoryFileAdd;

        private void OnDirectoryFileAdd(Updates.DirectoryFileAddEventArgs args)
        {
            DirectoryFileAdd?.Invoke(this, args);
        }

        public event EventHandler<Updates.DirectoryPrepareEventArgs> DirectoryPrepare;

        private void OnDirectoryPrepare(Updates.DirectoryPrepareEventArgs args)
        {
            DirectoryPrepare?.Invoke(this, args);
        }

        public event EventHandler<VersionInfo> Version;

        private void OnVersion(VersionInfo v)
        {
            Version?.Invoke(this, v);
        }

        public event EventHandler<Updates.TextSettingEventArgs> TextSetting;

        private void OnTextSetting(Updates.TextSettingEventArgs args)
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

        public event EventHandler<Updates.CountEventArgs> IncomingCount;

        private void OnIncomingCount(Updates.CountEventArgs e)
        {
            IncomingCount?.Invoke(this, e);
        }

        public event EventHandler<Updates.ErrorEventArgs> Error;

        private void OnError(Updates.ErrorEventArgs e)
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
                    OnChannelOperation(new Updates.PlayerStateEventArgs(cmdReceived.Channel(),
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
                    OnMarker(new Updates.MarkerEventArgs(cmdReceived.Channel(), op.AsMarkerType(), position));
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

            OnMarker(new Updates.MarkerEventArgs(channelId, MarkerType.Position, 0U));
            OnLoadedItem(new Updates.TrackLoadEventArgs(channelId, index, track));
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
                        OnItemAdd(new Updates.TrackAddEventArgs(channelId, index, entry));
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
                    OnItemMove(new Updates.TrackMoveEventArgs(channelId, indexFrom, indexTo));
                }
                    break;
                case Command.DeleteItem:
                {
                    var channelId = cmdReceived.Channel();
                    var index = _cs.ReceiveI();
                    OnItemDelete(new Updates.TrackDeleteEventArgs(channelId, index));
                }
                    break;
                case Command.ResetPlaylist:
                {
                    var channelId = cmdReceived.Channel();
                    OnResetPlaylist(new Updates.PlaylistResetEventArgs(channelId));
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
                        DecodeCount(Updates.CountType.LibraryItem);
                    }
                }
                    break;
                case Command.LibraryError:
                {
                    var errorCode = cmdReceived.DatabaseValue();
                    DecodeError(Updates.ErrorType.Library, errorCode);
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
                        DecodeCount(Updates.CountType.Show);
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
                        DecodeCount(Updates.CountType.Listing);
                    }

                    break;
                case Command.BapsDbError:
                    DecodeError(Updates.ErrorType.BapsDb, cmdReceived.DatabaseValue());
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
                        OnConfigOption(new Updates.ConfigOptionEventArgs(optionId, (ConfigType) type, description,
                            hasIndex,
                            index));
                    }
                    else
                    {
                        DecodeCount(Updates.CountType.ConfigOption);
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
                        OnConfigChoice(new Updates.ConfigChoiceEventArgs(optionId, choiceIndex, choiceDescription));
                    }
                    else
                    {
                        DecodeCount(Updates.CountType.ConfigChoice, optionId);
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
                    DecodeError(Updates.ErrorType.Config, cmdReceived.ConfigValue());
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
                        DecodeCount(Updates.CountType.User);
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
                        DecodeCount(Updates.CountType.Permission);
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
                        DecodeCount(Updates.CountType.IpRestriction);
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

            OnConfigSetting(new Updates.ConfigSettingEventArgs(optionId, type, value, index));
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
                        OnDirectoryFileAdd(new Updates.DirectoryFileAddEventArgs(directoryIndex, index, description));
                    }
                    else
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        _ = _cs.ReceiveI();
                        var niceDirectoryName = _cs.ReceiveS();
                        OnDirectoryPrepare(new Updates.DirectoryPrepareEventArgs(directoryIndex, niceDirectoryName));
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
                    var upDown = cmdReceived.SystemValue() == 0 ? Updates.UpDown.Down : Updates.UpDown.Up;
                    OnTextSetting(new Updates.TextSettingEventArgs(Updates.TextSetting.Scroll, upDown));
                }
                    break;
                case Command.TextSize:
                {
                    var upDown = cmdReceived.SystemValue() == 0 ? Updates.UpDown.Down : Updates.UpDown.Up;
                    OnTextSetting(new Updates.TextSettingEventArgs(Updates.TextSetting.FontSize, upDown));
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

        private void DecodeCount(Updates.CountType type, uint extra = 0)
        {
            var count = _cs.ReceiveI();
            OnIncomingCount(new Updates.CountEventArgs {Count = count, Type = type, Extra = extra});
        }

        private void DecodeError(Updates.ErrorType type, byte errorCode)
        {
            var description = _cs.ReceiveS();
            OnError(new Updates.ErrorEventArgs {Type = type, Code = errorCode, Description = description});
        }

        #endregion Command decoding
    }
}