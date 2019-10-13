using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     A <see cref="IPlaybackEventFeed"/> that filters another
    ///     <see cref="IPlaybackEventFeed"/> to select only events on a single
    ///     channel.
    /// </summary>
    public class SinglePlaybackEventFeed : IPlaybackEventFeed
    {
        public SinglePlaybackEventFeed(IPlaybackEventFeed parent, ushort channelId)
        {
            ChannelId = channelId;

            // TODO(@MattWindsor91): filter these properly
            ObserveIncomingCount = parent.ObserveIncomingCount;
            ObserveUnknownCommand = parent.ObserveUnknownCommand;

            ObservePlayerState = OnThisChannel(parent.ObservePlayerState);
            ObserveMarker = OnThisChannel(parent.ObserveMarker);
            ObserveTrackLoad = OnThisChannel(parent.ObserveTrackLoad);
        }

        public ushort ChannelId { get; set; }

        public IObservable<CountArgs> ObserveIncomingCount { get; }
        public IObservable<UnknownCommandArgs> ObserveUnknownCommand { get; }
        public IObservable<PlaybackStateChangeArgs> ObservePlayerState { get; }
        public IObservable<MarkerChangeArgs> ObserveMarker { get; }
        public IObservable<TrackLoadArgs> ObserveTrackLoad { get; }

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
