using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Base class for view models that refer to part or all of a channel, and therefore have an associated
    ///     channel ID.
    /// </summary>
    public abstract class ChannelComponentViewModelBase : SubscribingViewModel
    {
        protected ChannelComponentViewModelBase(ushort channelId)
        {
            ChannelId = channelId;
        }

        /// <summary>
        ///     The ID of the channel this view model concerns.
        /// </summary>
        public ushort ChannelId { get; }

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