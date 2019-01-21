﻿using System;

namespace BAPSCommon
{
    public interface IReceiver
    {
        event ServerUpdates.ChannelStateEventHandler ChannelState;
        event EventHandler<(uint optionID, uint choiceIndex, string choiceDescription)> ConfigChoice;
        event ServerUpdates.ConfigOptionHandler ConfigOption;
        event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;

        /// <summary>
        /// Event raised when the server declares that a setting on a
        /// config option has changed.
        /// </summary>
        event ServerUpdates.ConfigSettingHandler ConfigSetting;

        event EventHandler<(ushort directoryID, uint index, string description)> DirectoryFileAdd;
        event EventHandler<(ushort directoryID, string directoryName)> DirectoryPrepare;
        event EventHandler<(ushort channelID, uint duration)> Duration;
        event ServerUpdates.ErrorEventHandler Error;
        event ServerUpdates.CountEventHandler IncomingCount;
        event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IPRestriction;
        event ServerUpdates.ItemAddEventHandler ItemAdd;
        event ServerUpdates.ItemDeleteEventHandler ItemDelete;
        event ServerUpdates.ItemMoveEventHandler ItemMove;
        event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
        event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
        event EventHandler<(ushort channelID, uint index, TracklistItem entry)> LoadedItem;
        event EventHandler<(uint permissionCode, string description)> Permission;
        event EventHandler<(ushort channelID, PositionType type, uint position)> Position;
        event ServerUpdates.ChannelResetEventHandler ResetPlaylist;
        event EventHandler<bool> ServerQuit;
        event EventHandler<(uint showID, string description)> ShowResult;
        event EventHandler<(ushort ChannelID, uint index, TextTracklistItem entry)> TextItem;
        event EventHandler<ServerUpdates.UpDown> TextScroll;
        event EventHandler<ServerUpdates.UpDown> TextSizeChange;
        event EventHandler<(Command command, string description)> UnknownCommand;
        event EventHandler<(string username, uint permissions)> User;
        event EventHandler<(byte resultCode, string description)> UserResult;
        event EventHandler<Receiver.VersionInfo> Version;
    }
}