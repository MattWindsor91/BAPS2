using System;
using URY.BAPS.Client.Common.BapsNet;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater
        : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater, IPlaylistServerUpdater,
            ISystemServerUpdater
    {
        IObservable<(Command cmdReceived, string ipAddress, uint mask)> ObserveIpRestriction { get; }

        IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult { get; }
        IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult { get; }
        IObservable<(uint permissionCode, string description)> ObservePermission { get; }
        IObservable<(uint showID, string description)> ObserveShowResult { get; }

        IObservable<(Command command, string description)> ObserveUnknownCommand { get; }
        IObservable<(string username, uint permissions)> ObserveUser { get; }
        IObservable<(byte resultCode, string description)> ObserveUserResult { get; }
    }
}