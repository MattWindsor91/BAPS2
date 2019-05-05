using System;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater
        : IConfigServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater, IPlaylistServerUpdater,
            ISystemServerUpdater
    {
        IObservable<IpRestriction> ObserveIpRestriction { get; }

        IObservable<(uint resultID, byte dirtyStatus, string description)> ObserveLibraryResult { get; }
        IObservable<(uint listingID, uint channelID, string description)> ObserveListingResult { get; }
        IObservable<(uint permissionCode, string description)> ObservePermission { get; }
        IObservable<(uint showID, string description)> ObserveShowResult { get; }

        IObservable<(CommandWord command, string description)> ObserveUnknownCommand { get; }
        IObservable<(string username, uint permissions)> ObserveUser { get; }
        IObservable<(byte resultCode, string description)> ObserveUserResult { get; }
    }
}