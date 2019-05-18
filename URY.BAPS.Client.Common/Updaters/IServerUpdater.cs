namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Event interface for classes that send BAPSNet server updates.
    /// </summary>
    public interface IServerUpdater
        : IConfigServerUpdater, IDatabaseServerUpdater, IDirectoryServerUpdater, IPlaybackServerUpdater,
            IPlaylistServerUpdater,
            ISystemServerUpdater
    {
    }
}