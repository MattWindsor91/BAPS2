using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     A <see cref="IPlaylistEventFeed"/> that filters another
    ///     <see cref="IPlaylistEventFeed"/> to select only events on a single
    ///     channel.
    /// </summary>
    public class SinglePlaylistEventFeed : IPlaylistEventFeed
    {
        public SinglePlaylistEventFeed(IPlaylistEventFeed parent, ushort channelId)
        {
            ChannelId = channelId;

            // TODO(@MattWindsor91): filter these properly
            ObserveIncomingCount = parent.ObserveIncomingCount;
            ObserveUnknownCommand = parent.ObserveUnknownCommand;

            ObserveTrackAdd = OnThisChannel(parent.ObserveTrackAdd);
            ObserveTrackDelete = OnThisChannel(parent.ObserveTrackDelete);
            ObserveTrackMove = OnThisChannel(parent.ObserveTrackMove);
            ObservePlaylistReset = OnThisChannel(parent.ObservePlaylistReset);
        }

        public ushort ChannelId { get; set; }

        public IObservable<CountArgs> ObserveIncomingCount { get; }
        public IObservable<UnknownCommandArgs> ObserveUnknownCommand { get; }
        public IObservable<TrackAddArgs> ObserveTrackAdd { get; }
        public IObservable<TrackDeleteArgs> ObserveTrackDelete { get; }
        public IObservable<TrackMoveArgs> ObserveTrackMove { get; }
        public IObservable<PlaylistResetArgs> ObservePlaylistReset { get; }

        /// <summary>
        ///     Restricts a channel observable to returning only events for this channel.
        /// </summary>
        /// <param name="source">The observable to restrict.</param>
        /// <typeparam name="TResult">Type of results from <paramref name="source"/>.</typeparam>
        /// <returns>
        ///     <paramref name="source"/>, but restricted to returning only events for
        ///     channel <see cref="ChannelId"/>.
        /// </returns>
        [Pure]
        protected IObservable<TResult> OnThisChannel<TResult>(IObservable<TResult> source)
            where TResult : ChannelArgsBase
        {
            return from ev in source where ev.ChannelId == ChannelId select ev;
        }
    }
}
