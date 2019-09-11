using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Playback;
using Xunit;

namespace URY.BAPS.Client.Common.Tests.Updaters
{
    /// <summary>
    ///     Tests for <see cref="DetachableEventFeed"/>.
    /// </summary>
    public class DetachableServerUpdaterTests
    {
        private readonly DetachableEventFeed _updater = new DetachableEventFeed();

        private event EventHandler<MessageArgsBase>? Messages;
        private readonly IObservable<MessageArgsBase> _messageObservable;
        public DetachableServerUpdaterTests()
        {
            _messageObservable = Observable.FromEventPattern<MessageArgsBase>(e => Messages += e, e => Messages -= e).Select(x => x.EventArgs);
        }


        /// <summary>
        ///     Tests that detaching on an updater with nothing attached
        ///     throws no exceptions.
        /// </summary>
        [Fact]
        public void TestDetach_NothingAttached_NoThrow()
        {
            _updater.Detach();
        }

        [Fact]
        public void TestAttachDetach_WithMessages()
        {
            var results = new List<MessageArgsBase>();
            var observer = Observer.Create<MessageArgsBase>(x => results.Add(x));

            using var unused1 = _updater.ObserveIncomingCount.Subscribe(observer);
            var message1 = new CountArgs(CountType.ConfigChoice, 10, 0);

            using var unused2 = _updater.ObserveMarker.Subscribe(observer);
            var message2 = new MarkerChangeArgs(0, MarkerType.Cue, 2001);

            using var unused3 = _updater.ObservePlayerState.Subscribe(observer);
            var message3 = new PlaybackStateChangeArgs(20, PlaybackState.Stopped);

            Messages?.Invoke(this, message1);
            _updater.Attach(_messageObservable);
            Messages?.Invoke(this, message2);
            _updater.Detach();
            Messages?.Invoke(this, message3);

            Assert.Collection(results, m =>
            {
                var m2 = Assert.IsType<MarkerChangeArgs>(m);
                Assert.Equal(0, m2.ChannelId);
                Assert.Equal(MarkerType.Cue, m2.Marker);
                Assert.Equal(2001u, m2.NewValue);
            });
        }
    }
}
