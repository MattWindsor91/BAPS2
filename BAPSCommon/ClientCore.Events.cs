using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon
{
    public partial class ClientCore
    {
        public event EventHandler<Updates.ConfigChoiceArgs> ConfigChoice;
        public event EventHandler<Updates.ConfigOptionArgs> ConfigOption;
        public event EventHandler<Updates.ConfigSettingArgs> ConfigSetting;
        public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;
        public event EventHandler<Updates.DirectoryFileAddArgs> DirectoryFileAdd;
        public event EventHandler<Updates.DirectoryPrepareArgs> DirectoryPrepare;
        public event EventHandler<Updates.PlayerStateEventArgs> ChannelState;
        public event EventHandler<Updates.MarkerEventArgs> ChannelMarker;
        public event EventHandler<Updates.ErrorEventArgs> Error;
        public event EventHandler<Updates.CountEventArgs> IncomingCount;
        public event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IpRestriction;
        public event EventHandler<Updates.TrackAddEventArgs> ItemAdd;
        public event EventHandler<Updates.TrackDeleteEventArgs> ItemDelete;
        public event EventHandler<Updates.TrackMoveEventArgs> ItemMove;
        public event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        public event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        public event EventHandler<Updates.TrackLoadEventArgs> TrackLoad;
        public event EventHandler<(uint permissionCode, string description)> Permission;
        public event EventHandler<Updates.ChannelResetEventArgs> ResetPlaylist;
        public event EventHandler<bool> ServerQuit;
        public event EventHandler<(uint showID, string description)> ShowResult;
        public event EventHandler<Updates.UpDown> TextScroll;
        public event EventHandler<Updates.UpDown> TextSizeChange;
        public event EventHandler<(Command command, string description)> UnknownCommand;
        public event EventHandler<(string username, uint permissions)> User;
        public event EventHandler<(byte resultCode, string description)> UserResult;
        public event EventHandler<Receiver.VersionInfo> Version;

        /// <summary>
        ///     Installs events on <see cref="_receiver" /> that forward each server update to the <see cref="ClientCore" />'s own
        ///     events.
        ///     <para>
        ///         This is done so that the <see cref="ClientCore"/> can expose the events to the client before it
        ///         connects to 
        ///     </para>
        /// </summary>
        private void AttachReceiverEvents()
        {
            _receiver.ConfigChoice += OnConfigChoice;
            _receiver.ConfigOption += OnConfigOption;
            _receiver.ConfigSetting += OnConfigSetting;
            _receiver.ConfigResult += OnConfigResult;
            _receiver.DirectoryFileAdd += OnDirectoryFileAdd;
            _receiver.DirectoryPrepare += OnDirectoryPrepare;
            _receiver.ChannelState += OnChannelState;
            _receiver.ChannelMarker += OnChannelMarker;
            _receiver.Error += OnError;
            _receiver.IncomingCount += OnIncomingCount;
            _receiver.IpRestriction += OnIpRestriction;
            _receiver.ItemAdd += OnItemAdd;
            _receiver.ItemDelete += OnItemDelete;
            _receiver.ItemMove += OnItemMove;
            _receiver.LibraryResult += OnLibraryResult;
            _receiver.ListingResult += OnListingResult;
            _receiver.TrackLoad += OnTrackLoad;
            _receiver.Permission += OnPermission;
            _receiver.ResetPlaylist += OnResetPlaylist;
            _receiver.ServerQuit += OnServerQuit;
            _receiver.ShowResult += OnShowResult;
            _receiver.TextScroll += OnTextScroll;
            _receiver.TextSizeChange += OnTextSizeChange;
            _receiver.UnknownCommand += OnUnknownCommand;
            _receiver.User += OnUser;
            _receiver.UserResult += OnUserResult;
            _receiver.Version += OnVersion;
        }

        protected virtual void OnConfigChoice(object sender, Updates.ConfigChoiceArgs e)
        {
            ConfigChoice?.Invoke(this, e);
        }

        protected virtual void OnConfigOption(object sender, Updates.ConfigOptionArgs e)
        {
            ConfigOption?.Invoke(this, e);
        }

        protected virtual void OnConfigSetting(object sender, Updates.ConfigSettingArgs e)
        {
            ConfigSetting?.Invoke(this, e);
        }

        protected virtual void OnConfigResult(object sender, (Command cmdReceived, uint optionID, ConfigResult result) e)
        {
            ConfigResult?.Invoke(this, e);
        }

        protected virtual void OnDirectoryFileAdd(object sender, Updates.DirectoryFileAddArgs e)
        {
            DirectoryFileAdd?.Invoke(this, e);
        }

        protected virtual void OnDirectoryPrepare(object sender, Updates.DirectoryPrepareArgs e)
        {
            DirectoryPrepare?.Invoke(this, e);
        }

        protected virtual void OnChannelState(object sender, Updates.PlayerStateEventArgs e)
        {
            ChannelState?.Invoke(this, e);
        }

        protected virtual void OnChannelMarker(object sender, Updates.MarkerEventArgs args)
        {
            ChannelMarker?.Invoke(this, args);
        }

        protected virtual void OnError(object sender, Updates.ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnIncomingCount(object sender, Updates.CountEventArgs e)
        {
            IncomingCount?.Invoke(this, e);
        }

        protected virtual void OnIpRestriction(object sender, (Command cmdReceived, string ipAddress, uint mask) e)
        {
            IpRestriction?.Invoke(this, e);
        }

        protected virtual void OnItemAdd(object sender, Updates.TrackAddEventArgs e)
        {
            ItemAdd?.Invoke(this, e);
        }

        protected virtual void OnItemDelete(object sender, Updates.TrackDeleteEventArgs e)
        {
            ItemDelete?.Invoke(this, e);
        }

        protected virtual void OnItemMove(object sender, Updates.TrackMoveEventArgs e)
        {
            ItemMove?.Invoke(this, e);
        }

        protected virtual void OnLibraryResult(object sender, (uint resultID, byte dirtyStatus, string description) e)
        {
            LibraryResult?.Invoke(this, e);
        }

        protected virtual void OnListingResult(object sender, (uint listingID, uint channelID, string description) e)
        {
            ListingResult?.Invoke(this, e);
        }

        protected virtual void OnTrackLoad(object sender, Updates.TrackLoadEventArgs e)
        {
            TrackLoad?.Invoke(this, e);
        }

        protected virtual void OnPermission(object sender, (uint permissionCode, string description) e)
        {
            Permission?.Invoke(this, e);
        }

        protected virtual void OnResetPlaylist(object sender, Updates.ChannelResetEventArgs e)
        {
            ResetPlaylist?.Invoke(this, e);
        }

        protected virtual void OnServerQuit(object sender, bool e)
        {
            ServerQuit?.Invoke(this, e);
        }

        protected virtual void OnShowResult(object sender, (uint showID, string description) e)
        {
            ShowResult?.Invoke(this, e);
        }

        protected virtual void OnTextScroll(object sender, Updates.UpDown e)
        {
            TextScroll?.Invoke(this, e);
        }

        protected virtual void OnTextSizeChange(object sender, Updates.UpDown e)
        {
            TextSizeChange?.Invoke(this, e);
        }

        protected virtual void OnUnknownCommand(object sender, (Command command, string description) e)
        {
            UnknownCommand?.Invoke(this, e);
        }

        protected virtual void OnUser(object sender, (string username, uint permissions) e)
        {
            User?.Invoke(this, e);
        }

        protected virtual void OnUserResult(object sender, (byte resultCode, string description) e)
        {
            UserResult?.Invoke(this, e);
        }

        protected virtual void OnVersion(object sender, Receiver.VersionInfo e)
        {
            Version?.Invoke(this, e);
        }
    }
}