using System;
using System.Threading;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Listens on a <see cref="ClientSocket" /> for incoming BapsNet commands, decodes them, and sends server update
    ///     events.
    /// </summary>
    public class Receiver : IServerUpdater
    {
        private readonly ClientSocket _cs;
        private readonly CancellationToken _token;

        public Receiver(ClientSocket cs, CancellationToken token)
        {
            _cs = cs;
            _token = token;
        }

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

        public event EventHandler<Updates.PlayerStateEventArgs> ChannelState;

        private void OnChannelOperation(Updates.PlayerStateEventArgs e)
        {
            ChannelState?.Invoke(this, e);
        }

        public event EventHandler<Updates.TrackLoadEventArgs> TrackLoad;

        private void OnLoadedItem(Updates.TrackLoadEventArgs args)
        {
            TrackLoad?.Invoke(this, args);
        }

        public event EventHandler<Updates.MarkerEventArgs> ChannelMarker;

        private void OnMarker(Updates.MarkerEventArgs e)
        {
            ChannelMarker?.Invoke(this, e);
        }

        #endregion Playback events

        #region Playlist events

        public event EventHandler<Updates.TrackAddEventArgs> ItemAdd;

        private void OnItemAdd(Updates.TrackAddEventArgs e)
        {
            ItemAdd?.Invoke(this, e);
        }

        public event EventHandler<Updates.TrackMoveEventArgs> ItemMove;

        private void OnItemMove(Updates.TrackMoveEventArgs e)
        {
            ItemMove?.Invoke(this, e);
        }

        public event EventHandler<Updates.TrackDeleteEventArgs> ItemDelete;

        private void OnItemDelete(Updates.TrackDeleteEventArgs e)
        {
            ItemDelete?.Invoke(this, e);
        }

        public event EventHandler<Updates.ChannelResetEventArgs> ResetPlaylist;

        private void OnResetPlaylist(Updates.ChannelResetEventArgs e)
        {
            ResetPlaylist?.Invoke(this, e);
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

        public event EventHandler<Updates.ConfigOptionArgs> ConfigOption;

        private void OnConfigOption(Updates.ConfigOptionArgs args)
        {
            ConfigOption?.Invoke(this, args);
        }

        public event EventHandler<Updates.ConfigChoiceArgs> ConfigChoice;

        private void OnConfigChoice(Updates.ConfigChoiceArgs args)
        {
            ConfigChoice?.Invoke(this, args);
        }

        public event EventHandler<Updates.ConfigSettingArgs> ConfigSetting;

        private void OnConfigSetting(Updates.ConfigSettingArgs args)
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

        public event EventHandler<Updates.DirectoryFileAddArgs> DirectoryFileAdd;

        private void OnDirectoryFileAdd(Updates.DirectoryFileAddArgs args)
        {
            DirectoryFileAdd?.Invoke(this, args);
        }

        public event EventHandler<Updates.DirectoryPrepareArgs> DirectoryPrepare;

        private void OnDirectoryPrepare(Updates.DirectoryPrepareArgs args)
        {
            DirectoryPrepare?.Invoke(this, args);
        }

        public event EventHandler<VersionInfo> Version;

        private void OnVersion(VersionInfo v)
        {
            Version?.Invoke(this, v);
        }

        public event EventHandler<Updates.UpDown> TextScroll;

        private void OnTextScroll(Updates.UpDown upDown)
        {
            TextScroll?.Invoke(this, upDown);
        }

        public event EventHandler<Updates.UpDown> TextSizeChange;

        public void OnTextSizeChange(Updates.UpDown upDown)
        {
            TextSizeChange?.Invoke(this, upDown);
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
                    OnResetPlaylist(new Updates.ChannelResetEventArgs(channelId));
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
                        OnConfigOption(new Updates.ConfigOptionArgs(optionId, (ConfigType) type, description, hasIndex,
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
                        OnConfigChoice(new Updates.ConfigChoiceArgs(optionId, choiceIndex, choiceDescription));
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

            OnConfigSetting(new Updates.ConfigSettingArgs(optionId, type, value, index));
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
                        OnDirectoryFileAdd(new Updates.DirectoryFileAddArgs(directoryIndex, index, description));
                    }
                    else
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        _ = _cs.ReceiveI();
                        var niceDirectoryName = _cs.ReceiveS();
                        OnDirectoryPrepare(new Updates.DirectoryPrepareArgs(directoryIndex, niceDirectoryName));
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
                    OnTextScroll(upDown);
                }
                    break;
                case Command.TextSize:
                {
                    var upDown = cmdReceived.SystemValue() == 0 ? Updates.UpDown.Down : Updates.UpDown.Up;
                    OnTextSizeChange(upDown);
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