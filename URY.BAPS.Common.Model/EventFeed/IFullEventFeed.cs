using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Interface for classes that contain a 'full' set of event feeds for
    ///     incoming BAPS messages.
    /// </summary>
    public interface IFullEventFeed
        : IConfigEventFeed, IDatabaseEventFeed, IDirectoryEventFeed, IPlaybackEventFeed,
            IPlaylistEventFeed,
            ISystemEventFeed
    {
        /// <summary>
        ///     An observable ranging over all incoming BAPS messages.
        /// </summary>
        IObservable<MessageArgsBase> ObserveMessages { get; }
    }
}