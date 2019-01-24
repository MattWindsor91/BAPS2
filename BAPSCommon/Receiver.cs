using System;
using System.Threading;
using static BAPSCommon.ServerUpdates;

namespace BAPSCommon
{
    public class Receiver : IServerUpdater
    {
        private ClientSocket _cs;
        private CancellationToken _token;

        public Receiver(ClientSocket cs, CancellationToken token)
        {
            _cs = cs;
            _token = token;
        }

        public void Run()
        {
            while (true)
            {
                _token.ThrowIfCancellationRequested();
                var cmdReceived = _cs.ReceiveC();
                DecodeCommand(cmdReceived);
            }
        }


        public struct VersionInfo { public string Version; public string Date; public string Time; public string Author; }

        #region Playback events

        public event ChannelStateEventHandler ChannelState;
        private void OnChannelOperation(ChannelStateEventArgs e) => ChannelState?.Invoke(this, e);

        public event EventHandler<(ushort channelID, PositionType type, uint position)> Position;
        private void OnPosition(ushort channelId, PositionType type, uint position) => Position?.Invoke(this, (channelID: channelId, type, position));

        public event EventHandler<(ushort channelID, uint duration)> Duration;
        private void OnDuration(ushort channelId, uint duration) => Duration?.Invoke(this, (channelID: channelId, duration));

        public event EventHandler<(ushort channelID, uint index, TracklistItem entry)> LoadedItem;
        private void OnLoadedItem(ushort channelId, uint index, TracklistItem entry) => LoadedItem?.Invoke(this, (channelID: channelId, index, entry));

        public event EventHandler<(ushort ChannelID, uint index, TextTracklistItem entry)> TextItem;
        private void OnTextItem(ushort channelId, uint index, TextTracklistItem entry) => TextItem?.Invoke(this, (channelId, index, entry));

        #endregion Playback events

        #region Playlist events

        public event ItemAddEventHandler ItemAdd;
        private void OnItemAdd(ItemAddEventArgs e) => ItemAdd?.Invoke(this, e);

        public event ItemMoveEventHandler ItemMove;
        private void OnItemMove(ItemMoveEventArgs e) => ItemMove?.Invoke(this, e);

        public event ItemDeleteEventHandler ItemDelete;
        private void OnItemDelete(ItemDeleteEventArgs e) => ItemDelete?.Invoke(this, e);

        public event ChannelResetEventHandler ResetPlaylist;
        private void OnResetPlaylist(ChannelResetEventArgs e) => ResetPlaylist?.Invoke(this, e);

        #endregion Playlist events

        #region Database events

        public event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        private void OnLibraryResult(uint resultId, byte dirtyStatus, string description) => LibraryResult?.Invoke(this, (resultID: resultId, dirtyStatus, description));

        public event EventHandler<(uint showID, string description)> ShowResult;
        private void OnShowResult(uint showId, string description) => ShowResult?.Invoke(this, (showID: showId, description));

        public event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        private void OnListingResult(uint listingId, uint channelId, string description) => ListingResult?.Invoke(this, (listingID: listingId, channelID: channelId, description));

        #endregion Database events

        #region Config events


        public event ConfigOptionHandler ConfigOption;
        private void OnConfigOption(ConfigOptionArgs args) => ConfigOption?.Invoke(this, args);

        public event ConfigChoiceHandler ConfigChoice;
        private void OnConfigChoice(ConfigChoiceArgs args) => ConfigChoice?.Invoke(this, args);

        public event ConfigSettingHandler ConfigSetting;
        private void OnConfigSetting(ConfigSettingArgs args) => ConfigSetting?.Invoke(this, args);

        public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;
        private void OnConfigResult(Command cmdReceived, uint optionId, ConfigResult result) => ConfigResult?.Invoke(this, (cmdReceived, optionID: optionId, result));

        public event EventHandler<(string username, uint permissions)> User;
        private void OnUser(string username, uint permissions) => User?.Invoke(this, (username, permissions));

        public event EventHandler<(uint permissionCode, string description)> Permission;
        private void OnPermission(uint permissionCode, string description) => Permission?.Invoke(this, (permissionCode, description));

        public event EventHandler<(byte resultCode, string description)> UserResult;
        private void OnUserResult(byte resultCode, string description) => UserResult?.Invoke(this, (resultCode, description));

        public event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IpRestriction;
        private void OnIpRestriction(Command cmdReceived, string ipAddress, uint mask) => IpRestriction?.Invoke(this, (cmdReceived, ipAddress, mask));

        #endregion Config events

        #region System events

        public event DirectoryFileAddHandler DirectoryFileAdd;
        public void OnDirectoryFileAdd(DirectoryFileAddArgs args) => DirectoryFileAdd?.Invoke(this, args);

        public event DirectoryPrepareHandler DirectoryPrepare;
        public void OnDirectoryPrepare(DirectoryPrepareArgs args) => DirectoryPrepare?.Invoke(this, args);

        public event EventHandler<VersionInfo> Version;
        public void OnVersion(VersionInfo v) => Version?.Invoke(this, v);

        public event EventHandler<UpDown> TextScroll;
        public void OnTextScroll(UpDown updown) => TextScroll?.Invoke(this, updown);

        public event EventHandler<UpDown> TextSizeChange;
        public void OnTextSizeChange(UpDown updown) => TextSizeChange?.Invoke(this, updown);

        public event EventHandler<bool> ServerQuit;
        public void OnServerQuit(bool expected) => ServerQuit?.Invoke(this, expected);

        #endregion System events

        #region General events

        public event CountEventHandler IncomingCount;
        private void OnIncomingCount(CountEventArgs e) => IncomingCount?.Invoke(this, e);

        public event ErrorEventHandler Error;
        private void OnError(ErrorEventArgs e) => Error?.Invoke(this, e);

        public event EventHandler<(Command command, string description)> UnknownCommand;
        private void OnUnknownCommand(Command command, string description) => UnknownCommand?.Invoke(this, (command, description));

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
                        OnChannelOperation(new ChannelStateEventArgs(cmdReceived.Channel(), cmdReceived.AsChannelState()));
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
                        var channelId = cmdReceived.Channel();
                        var index = _cs.ReceiveI();
                        var type = (Command)_cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        switch (type)
                        {
                            case Command.FileItem:
                            case Command.LibraryItem:
                                {
                                    var duration = _cs.ReceiveI();
                                    OnDuration(channelId, duration);
                                    OnPosition(channelId, PositionType.Position, 0U);
                                }
                                goto case Command.VoidItem;
                            case Command.VoidItem:
                                {
                                    var entry = TracklistItemFactory.Create(type, description);
                                    OnLoadedItem(channelId, index, entry);
                                }
                                break;
                            case Command.TextItem:
                                {
                                    var text = _cs.ReceiveS();
                                    var entry = new TextTracklistItem(description, text);
                                    OnTextItem(channelId, index, entry);
                                }
                                break;
                            default:
                                OnDuration(channelId, 0U);
                                OnPosition(channelId, PositionType.Position, 0U);
                                break;
                        }
                    }
                    break;
                case Command.Position:
                case Command.CuePosition:
                case Command.IntroPosition:
                    {
                        var position = _cs.ReceiveI();
                        OnPosition(cmdReceived.Channel(), op.AsPositionType(), position);
                    }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed PLAYBACK");
                    break;
            }
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
                        var type = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        var entry = TracklistItemFactory.Create((Command)type, description);
                        OnItemAdd(new ItemAddEventArgs(channelId, index, entry));
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
                        OnItemMove(new ItemMoveEventArgs(channelId, indexFrom, indexTo));
                    }
                    break;
                case Command.DeleteItem:
                    {
                        var channelId = cmdReceived.Channel();
                        var index = _cs.ReceiveI();
                        OnItemDelete(new ItemDeleteEventArgs(channelId, index));
                    }
                    break;
                case Command.ResetPlaylist:
                    {
                        var channelId = cmdReceived.Channel();
                        OnResetPlaylist(new ChannelResetEventArgs(channelId));
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
                        else DecodeCount(CountType.LibraryItem);
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
                    else DecodeCount(CountType.Show);
                    break;
                case Command.Listing:
                    if (cmdReceived.HasFlag(Command.DatabaseModeMask))
                    {
                        var listingId = _cs.ReceiveI();
                        var channelId = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnListingResult(listingId, channelId, description);
                    }
                    else DecodeCount(CountType.Listing);
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
                            OnConfigOption(new ConfigOptionArgs(optionId, (ConfigType)type, description, hasIndex, index));
                        }
                        else DecodeCount(CountType.ConfigOption);
                    }
                    break;
                case Command.OptionChoice:
                    {
                        var optionId = _cs.ReceiveI();
                        if (cmdReceived.HasFlag(Command.ConfigModeMask))
                        {
                            var choiceIndex = _cs.ReceiveI();
                            var choiceDescription = _cs.ReceiveS();
                            OnConfigChoice(new ConfigChoiceArgs(optionId, choiceIndex, choiceDescription));
                        }
                        else DecodeCount(CountType.ConfigChoice, extra: optionId);
                    }
                    break;
                case Command.ConfigSetting:
                    {
                        if (cmdReceived.HasFlag(Command.ConfigModeMask))
                        {
                            var optionId = _cs.ReceiveI();
                            var type = _cs.ReceiveI();
                            DecodeConfigSetting(cmdReceived, optionId, (ConfigType)type);
                        }
                        else
                        {
                            _ = _cs.ReceiveI();
                        }
                    }
                    break;
                case Command.ConfigResult:
                    {
                        var optionid = _cs.ReceiveI();
                        var result = _cs.ReceiveI();
                        OnConfigResult(cmdReceived, optionid, (ConfigResult)result);
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
                        else DecodeCount(CountType.User);
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
                        else DecodeCount(CountType.Permission);
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
                            var ipaddress = _cs.ReceiveS();
                            var mask = _cs.ReceiveI();
                            OnIpRestriction(cmdReceived, ipaddress, mask);
                        }
                        else DecodeCount(CountType.IpRestriction);
                    }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed CONFIG");
                    break;
            }
        }

        private void DecodeConfigSetting(Command cmdReceived, uint optionId, ConfigType type)
        {
            object value = null;
            /** Determine what the final argument is going to be and retrieve it **/
            switch (type)
            {
                case ConfigType.Int:
                case ConfigType.Choice:
                    value = (int)_cs.ReceiveI();
                    break;
                case ConfigType.Str:
                    value = _cs.ReceiveS();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, "Invalid type received");
            }

            /** Use index=-1 to represent a non indexed setting **/
            int index = -1;
            if (cmdReceived.HasFlag(Command.ConfigUseValueMask))
            {
                index = cmdReceived.ConfigValue();
            }

            OnConfigSetting(new ConfigSettingArgs(optionId, type, value, index));
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
                        OnDirectoryFileAdd(new DirectoryFileAddArgs(directoryIndex, index, description));
                    }
                    else
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        _ = _cs.ReceiveI();
                        var niceDirectoryName = _cs.ReceiveS();
                        OnDirectoryPrepare(new DirectoryPrepareArgs(directoryIndex, niceDirectoryName));
                    }
                    break;
                case Command.Version:
                    {
                        var version = _cs.ReceiveS();
                        var date = _cs.ReceiveS();
                        var time = _cs.ReceiveS();
                        var author = _cs.ReceiveS();
                        OnVersion(new VersionInfo { Version = version, Date = date, Time = time, Author = author });
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
                        var updown = cmdReceived.SystemValue() == 0 ? UpDown.Down : UpDown.Up;
                        OnTextScroll(updown);
                    }
                    break;
                case Command.TextSize:
                    {
                        var updown = cmdReceived.SystemValue() == 0 ? UpDown.Down : UpDown.Up;
                        OnTextSizeChange(updown);
                    }
                    break;
                case Command.Quit:
                    {
                        //The server should send an int representing if this is an expected quit (0) or an exception error (1)."
                        bool expected = _cs.ReceiveI() == 0;
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
            OnIncomingCount(new CountEventArgs { Count = count, Type = type, Extra = extra });
        }

        private void DecodeError(ErrorType type, byte errorCode)
        {
            var description = _cs.ReceiveS();
            OnError(new ErrorEventArgs { Type = type, Code = errorCode, Description = description });
        }

        #endregion Command decoding
    }
}
