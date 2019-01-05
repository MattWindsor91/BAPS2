using System;
using System.Threading;

namespace BAPSCommon
{
    public class Receiver
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

        public enum CountType
        {
            LibraryItem,
            Show,
            Listing,
            ConfigOption,
            ConfigChoice,
            User,
            Permission,
            IPRestriction
        }

        public enum UpDown : byte
        {
            Down = 0,
            Up   = 1,
        }

        public struct CountEventArgs { public CountType Type; public uint Count; public uint Extra;  }

        public delegate void CountEventHandler(object sender, CountEventArgs e);

        public enum ErrorType
        {
            Library,
            BapsDB,
            Config
        }

        public struct ErrorEventArgs { public ErrorType Type; public byte Code; public string Description; }

        /// <summary>
        /// Delegate for handling server errors.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The argument struct, containing the error code and description.</param>
        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        public struct VersionInfo { public string Version; public string Date; public string Time; public string Author; }

        #region Playback events

        public event EventHandler<(ushort channelID, Command op)> ChannelOperation;
        private void OnChannelOperation(ushort channelID, Command op) => ChannelOperation?.Invoke(this, (channelID, op));

        public event EventHandler<(ushort channelID, PositionType type, uint position)> Position;
        private void OnPosition(ushort channelID, PositionType type, uint position) => Position?.Invoke(this, (channelID, type, position));

        public event EventHandler<(ushort channelID, uint duration)> Duration;
        private void OnDuration(ushort channelID, uint duration) => Duration?.Invoke(this, (channelID, duration));

        public event EventHandler<(ushort channelID, uint index, Command type, string description)> LoadedItem;
        private void OnLoadedItem(ushort channelID, uint index, Command type, string description) => LoadedItem?.Invoke(this, (channelID, index, type, description));

        public event EventHandler<(ushort ChannelID, uint index, string description, string text)> TextItem;
        private void OnTextItem(ushort channelID, uint index, string description, string text) => TextItem?.Invoke(this, (channelID, index, description, text));

        #endregion Playback events

        #region Playlist events

        public event EventHandler<(ushort channelID, uint index, uint type, string description)> ItemAdd;
        private void OnItemAdd(ushort channelID, uint index, uint type, string description) => ItemAdd?.Invoke(this, (channelID, index, type, description));

        public event EventHandler<(ushort channelID, uint indexFrom, uint indexTo)> ItemMove;
        private void OnItemMove(ushort channelID, uint indexFrom, uint indexTo) => ItemMove?.Invoke(this, (channelID, indexFrom, indexTo));

        public event EventHandler<(ushort channelID, uint index)> ItemDelete;
        private void OnItemDelete(ushort channelID, uint index) => ItemDelete?.Invoke(this, (channelID, index));

        public event EventHandler<ushort> ResetPlaylist;
        private void OnResetPlaylist(ushort channelID) => ResetPlaylist?.Invoke(this, channelID);

        #endregion Playlist events

        #region Database events

        public event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        private void OnLibraryResult(uint resultID, byte dirtyStatus, string description) => LibraryResult?.Invoke(this, (resultID, dirtyStatus, description));

        public event EventHandler<(uint showID, string description)> ShowResult;
        private void OnShowResult(uint showID, string description) => ShowResult?.Invoke(this, (showID, description));

        public event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        private void OnListingResult(uint listingID, uint channelID, string description) => ListingResult?.Invoke(this, (listingID, channelID, description));

        #endregion Database events

        #region Config events

        public event EventHandler<(Command cmdReceived, uint optionID, string description, uint type)> ConfigOption;
        private void OnConfigOption(Command cmdReceived, uint optionID, string description, uint type) => ConfigOption?.Invoke(this, (cmdReceived, optionID, description, type));

        public event EventHandler<(uint optionID, uint choiceIndex, string choiceDescription)> ConfigChoice;
        private void OnConfigChoice(uint optionID, uint choiceIndex, string choiceDescription) => ConfigChoice?.Invoke(this, (optionID, choiceIndex, choiceDescription));

        public event EventHandler<(Command cmdReceived, uint optionID, ConfigType type)> ConfigSetting;
        private void OnConfigSetting(Command cmdReceived, uint optionID, ConfigType type) => ConfigSetting?.Invoke(this, (cmdReceived, optionID, type));

        public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;
        private void OnConfigResult(Command cmdReceived, uint optionID, ConfigResult result) => ConfigResult?.Invoke(this, (cmdReceived, optionID, result));

        public event EventHandler<(string username, uint permissions)> User;
        private void OnUser(string username, uint permissions) => User?.Invoke(this, (username, permissions));

        public event EventHandler<(uint permissionCode, string description)> Permission;
        private void OnPermission(uint permissionCode, string description) => Permission?.Invoke(this, (permissionCode, description));

        public event EventHandler<(byte resultCode, string description)> UserResult;
        private void OnUserResult(byte resultCode, string description) => UserResult?.Invoke(this, (resultCode, description));

        public event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IPRestriction;
        private void OnIPRestriction(Command cmdReceived, string ipAddress, uint mask) => IPRestriction?.Invoke(this, (cmdReceived, ipAddress, mask));

        #endregion Config events

        #region System events

        public event EventHandler<(ushort directoryID, uint index, string description)> DirectoryFileAdd;
        public void OnDirectoryFileAdd(byte directoryID, uint index, string description) => DirectoryFileAdd?.Invoke(this, (directoryID, index, description));

        public event EventHandler<(ushort directoryID, string directoryName)> DirectoryPrepare;
        public void OnDirectoryPrepare(byte directoryID, string directoryName) => DirectoryPrepare?.Invoke(this, (directoryID, directoryName));

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
            switch (cmdReceived & Command.GROUPMASK)
            {
                case Command.PLAYBACK:
                    DecodePlaybackCommand(cmdReceived);
                    break;
                case Command.PLAYLIST:
                    DecodePlaylistCommand(cmdReceived);
                    break;
                case Command.DATABASE:
                    DecodeDatabaseCommand(cmdReceived);
                    break;
                case Command.CONFIG:
                    DecodeConfigCommand(cmdReceived);
                    break;
                case Command.SYSTEM:
                    DecodeSystemCommand(cmdReceived);
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "unknown group");
                    break;
            }
        }

        private void DecodePlaybackCommand(Command cmdReceived)
        {
            var op = cmdReceived & Command.PLAYBACK_OPMASK;
            switch (op)
            {
                case Command.PLAY:
                case Command.PAUSE:
                case Command.STOP:
                    {
                        OnChannelOperation(cmdReceived.Channel(), op);
                    }
                    break;
                case Command.VOLUME:
                    {
                        // Deliberately ignore
                        _ = _cs.ReceiveF();
                    }
                    break;
                case Command.LOAD:
                    {
                        var channelID = cmdReceived.Channel();
                        var index = _cs.ReceiveI();
                        var type = (Command)_cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        switch (type)
                        {
                            case Command.FILEITEM:
                            case Command.LIBRARYITEM:
                                {
                                    var duration = _cs.ReceiveI();
                                    OnDuration(channelID, duration);
                                    OnPosition(channelID, PositionType.Position, 0U);
                                }
                                goto case Command.VOIDITEM;
                            case Command.VOIDITEM:
                                {
                                    OnLoadedItem(channelID, index, type, description);
                                }
                                break;
                            case Command.TEXTITEM:
                                {
                                    var text = _cs.ReceiveS();
                                    OnTextItem(channelID, index, description, text);
                                }
                                break;
                            default:
                                OnDuration(channelID, 0U);
                                OnPosition(channelID, PositionType.Position, 0U);
                                break;
                        }
                    }
                    break;
                case Command.POSITION:
                case Command.CUEPOSITION:
                case Command.INTROPOSITION:
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
            switch (cmdReceived & Command.PLAYLIST_OPMASK)
            {
                case Command.ITEM:
                    if (cmdReceived.HasFlag(Command.PLAYLIST_MODEMASK))
                    {
                        var channelID = cmdReceived.Channel();
                        var index = _cs.ReceiveI();
                        var type = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnItemAdd(channelID, index, type, description);
                    }
                    else
                    {
                        // Deliberately ignore?
                        _ = _cs.ReceiveI();
                    }
                    break;
                case Command.MOVEITEMTO:
                    {
                        var channelID = cmdReceived.Channel();
                        var indexFrom = _cs.ReceiveI();
                        var indexTo = _cs.ReceiveI();
                        OnItemMove(channelID, indexFrom, indexTo);
                    }
                    break;
                case Command.DELETEITEM:
                    {
                        var channelID = cmdReceived.Channel();
                        var index = _cs.ReceiveI();
                        OnItemDelete(channelID, index);
                    }
                    break;
                case Command.RESETPLAYLIST:
                    {
                        var channelID = cmdReceived.Channel();
                        OnResetPlaylist(channelID);
                    }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed PLAYLIST");
                    break;
            }
        }

        private void DecodeDatabaseCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.DATABASE_OPMASK)
            {
                case Command.LIBRARYRESULT:
                    {
                        if (cmdReceived.HasFlag(Command.DATABASE_MODEMASK))
                        {
                            var dirtyStatus = cmdReceived.DatabaseValue();
                            var resultID = _cs.ReceiveI();
                            var description = _cs.ReceiveS();
                            OnLibraryResult(resultID, dirtyStatus, description);
                        }
                        else DecodeCount(CountType.LibraryItem);
                    }
                    break;
                case Command.LIBRARYERROR:
                    {
                        var errorCode = cmdReceived.DatabaseValue();
                        DecodeError(ErrorType.Library, errorCode);
                    }
                    break;
                case Command.SHOW:
                    if (cmdReceived.HasFlag(Command.DATABASE_MODEMASK))
                    {
                        var showID = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnShowResult(showID, description);
                    }
                    else DecodeCount(CountType.Show);
                    break;
                case Command.LISTING:
                    if (cmdReceived.HasFlag(Command.DATABASE_MODEMASK))
                    {
                        var listingID = _cs.ReceiveI();
                        var channelID = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnListingResult(listingID, channelID, description);
                    }
                    else DecodeCount(CountType.Listing);
                    break;
                case Command.BAPSDBERROR:
                    DecodeError(ErrorType.BapsDB, cmdReceived.DatabaseValue());
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed DATABASE");
                    break;
            }
        }

        private void DecodeConfigCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.CONFIG_OPMASK)
            {
                case Command.OPTION:
                    {
                        if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                        {
                            var optionID = _cs.ReceiveI();
                            var description = _cs.ReceiveS();
                            var type = _cs.ReceiveI();
                            OnConfigOption(cmdReceived, optionID, description, type);
                        }
                        else DecodeCount(CountType.ConfigOption);
                    }
                    break;
                case Command.OPTIONCHOICE:
                    {
                        var optionID = _cs.ReceiveI();
                        if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                        {
                            var choiceIndex = _cs.ReceiveI();
                            var choiceDescription = _cs.ReceiveS();
                            OnConfigChoice(optionID, choiceIndex, choiceDescription);
                        }
                        else DecodeCount(CountType.ConfigChoice, extra: optionID);
                    }
                    break;
                case Command.CONFIGSETTING:
                    {
                        if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                        {
                            var optionID = _cs.ReceiveI();
                            var type = _cs.ReceiveI();
                            OnConfigSetting(cmdReceived, optionID, (ConfigType)type);
                        }
                        else
                        {
                            _ = _cs.ReceiveI();
                        }
                    }
                    break;
                case Command.CONFIGRESULT:
                    {
                        var optionid = _cs.ReceiveI();
                        var result = _cs.ReceiveI();
                        OnConfigResult(cmdReceived, optionid, (ConfigResult)result);
                    }
                    break;
                case Command.CONFIGERROR:
                    DecodeError(ErrorType.Config, cmdReceived.ConfigValue());
                    break;
                case Command.USER:
                    {
                        if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                        {
                            var username = _cs.ReceiveS();
                            var permissions = _cs.ReceiveI();
                            OnUser(username, permissions);
                        }
                        else DecodeCount(CountType.User);
                    }
                    break;
                case Command.PERMISSION:
                    {
                        if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                        {
                            var permissionCode = _cs.ReceiveI();
                            var description = _cs.ReceiveS();
                            OnPermission(permissionCode, description);
                        }
                        else DecodeCount(CountType.Permission);
                    }
                    break;
                case Command.USERRESULT:
                    {
                        var resultCode = cmdReceived.ConfigValue();
                        var description = _cs.ReceiveS();
                        OnUserResult(resultCode, description);
                    }
                    break;
                case Command.IPRESTRICTION:
                    {
                        if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                        {
                            var ipaddress = _cs.ReceiveS();
                            var mask = _cs.ReceiveI();
                            OnIPRestriction(cmdReceived, ipaddress, mask);
                        }
                        else DecodeCount(CountType.IPRestriction);
                    }
                    break;
                default:
                    OnUnknownCommand(cmdReceived, "possibly a malformed CONFIG");
                    break;
            }
        }

        private void DecodeSystemCommand(Command cmdReceived)
        {
            switch (cmdReceived & Command.SYSTEM_OPMASK)
            {
                case Command.SENDLOGMESSAGE:
                    _cs.ReceiveS();
                    break;
                case Command.FILENAME:
                    if (cmdReceived.HasFlag(Command.SYSTEM_MODEMASK))
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        var index = _cs.ReceiveI();
                        var description = _cs.ReceiveS();
                        OnDirectoryFileAdd(directoryIndex, index, description);
                    }
                    else
                    {
                        var directoryIndex = cmdReceived.SystemValue();
                        _ = _cs.ReceiveI();
                        var niceDirectoryName = _cs.ReceiveS();
                        OnDirectoryPrepare(directoryIndex, niceDirectoryName);
                    }
                    break;
                case Command.VERSION:
                    {
                        var version = _cs.ReceiveS();
                        var date = _cs.ReceiveS();
                        var time = _cs.ReceiveS();
                        var author = _cs.ReceiveS();
                        OnVersion(new VersionInfo { Version = version, Date = date, Time = time, Author = author });
                    }
                    break;
                case Command.FEEDBACK:
                    {
                        _ = _cs.ReceiveI();
                    }
                    break;
                case Command.SENDMESSAGE:
                    {
                        _ = _cs.ReceiveS();
                        _ = _cs.ReceiveS();
                        _ = _cs.ReceiveS();
                    }
                    break;
                case Command.CLIENTCHANGE:
                    {
                        _ = _cs.ReceiveS();
                    }
                    break;
                case Command.SCROLLTEXT:
                    {
                        var updown = cmdReceived.SystemValue() == 0 ? UpDown.Down : UpDown.Up;
                        OnTextScroll(updown);
                    }
                    break;
                case Command.TEXTSIZE:
                    {
                        var updown = cmdReceived.SystemValue() == 0 ? UpDown.Down : UpDown.Up;
                        OnTextSizeChange(updown);
                    }
                    break;
                case Command.QUIT:
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
