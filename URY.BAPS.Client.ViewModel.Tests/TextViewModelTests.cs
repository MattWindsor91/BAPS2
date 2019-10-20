using System;
using System.Reactive.Linq;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Track;
using Xunit;

namespace URY.BAPS.Client.ViewModel.Tests
{
    /// <summary>
    ///     Tests for <see cref="TextViewModel"/>.
    /// </summary>
    public class TextViewModelTests
    {
        private event EventHandler<MessageArgsBase>? sendMessage;
        private readonly TextViewModel _viewModel;

        public TextViewModelTests()
        {
            var messages =
                from ev in Observable.FromEventPattern<MessageArgsBase>(x => sendMessage += x, x => sendMessage -= x)
                select ev.EventArgs;
            var events = new FilteringEventFeed(messages);
            
            _viewModel = new TextViewModel(events);
        }
        
        /// <summary>
        ///     Tests that the text of the view model is empty by default.
        /// </summary>
        [Fact]
        public void TestText_Initial()
        {
            AssertTextEqual("");
        }

        /// <summary>
        ///     Tests that, after a text item is loaded on some channel, the
        ///     text of the view model updates to reflect the new text.
        /// </summary>
        [Fact]
        public void TestText_AfterTextLoad()
        {
            const string text = "This is the dawning of the age of Aquarius";
            SendTextTrackMessage(text);
            AssertTextEqual(text);
        }

        /// <summary>
        ///     Tests that a text item load surrounded by audio item loads
        ///     preserves the loaded text.
        /// </summary>
        [Fact]
        public void TestText_AfterAudioTextAudioItemLoad()
        {
            const string text = "Nobody puts Baby in a corner";
             
            SendAudioTrackMessage("Night Games", 8675309);
            SendTextTrackMessage(text);
            SendAudioTrackMessage("The Chain", 9876543);
            AssertTextEqual(text);
        }

        private void AssertTextEqual(string expected)
        {
            Assert.Equal(expected, _viewModel.Text);
        }

        private void SendAudioTrackMessage(string description, uint duration)
        {
            SendTrackLoadMessage(new LibraryTrack(description, duration));
        }

        private void SendTextTrackMessage(string text)
        {
            SendTrackLoadMessage(new TextTrack("Test text", text));
        }

        private void SendTrackLoadMessage(ITrack track)
        {
            sendMessage?.Invoke(this, new TrackLoadArgs(0, 0, track));
        }
    }
}