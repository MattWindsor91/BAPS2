using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Track;
using Xunit;

namespace URY.BAPS.Client.ViewModel.Tests
{
    /// <summary>
    ///     Tests for <see cref="TextViewModel" />.
    /// </summary>
    public class TextViewModelTests : ServerMessageViewModelTestsBase
    {
        public TextViewModelTests()
        {
            _viewModel = new TextViewModel(Events);
        }

        private readonly TextViewModel _viewModel;

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
            SendMessage(new TrackLoadArgs(new TrackIndex {ChannelId = 0, Position = 0}, track));
        }

        /// <summary>
        ///     Contains theory data with rows containing
        ///     (sequence of up/down tweaks, expected font scale, can increase, can decrease).
        /// </summary>
        public static TheoryData<string, int, bool, bool> FontScaleData = new TheoryData<string, int, bool, bool>
        {
            {"", 100, true, true},
            // Purely server messages
            {"u", 110, true, true},
            {"d", 90, true, true},
            {"ud", 100, true, true},
            {"du", 100, true, true},
            {"dddddddddddddddddddd", 50, true, false},
            {"uuuuuuuuuuuuuuuuuuuu", 200, false, true},
            // Purely client commands
            {"U", 110, true, true},
            {"D", 90, true, true},
            {"UD", 100, true, true},
            {"DU", 100, true, true},
            {"DDDDDDDDDDDDDDDDDDDD", 50, true, false},
            {"UUUUUUUUUUUUUUUUUUUU", 200, false, true},
            // Mixed bags
            {"uD", 100, true, true},
            {"Ud", 100, true, true},
            {"Uu", 120, true, true},
            {"uU", 120, true, true},
            {"Dd", 80, true, true},
            {"dD", 80, true, true}
        };

        /// <summary>
        ///     Checks the font scale after performing a sequence of up and
        ///     down tweaks.
        /// </summary>
        /// <param name="sequence">The sequence of up/down tweaks.</param>
        /// <param name="finalScale">The expected final value.</param>
        /// <param name="canIncrease">The expected final value.</param>
        /// <param name="canDecrease">The expected final value.</param>
        [Theory]
        [MemberData(nameof(FontScaleData))]
        public void TestFontScale_AfterSequence(string sequence, int finalScale, bool canIncrease, bool canDecrease)
        {
            ApplyFontScaleSequence(sequence);
            AssertFontScaleEqual(finalScale);
            AssertCanExecute(canDecrease, _viewModel.DecreaseFontScale);
            AssertCanExecute(canIncrease, _viewModel.IncreaseFontScale);
        }

        /// <summary>
        ///     Performs a series of up and
        ///     down tweaks, expressed as a string of the letters 'u'
        ///     (server-side up), 'U' (client-side up), 'd' (server-side down),
        ///     and 'D' (client-side down).
        /// </summary>
        /// <param name="sequence">The sequence of up/down tweaks.</param>
        private void ApplyFontScaleSequence(string sequence)
        {
            var cookedSequence =
                from ch in sequence
                select (char.IsUpper(ch), DirectionFromChar(char.ToLower(ch)));

            foreach (var (useCommand, direction) in cookedSequence)
                if (useCommand)
                    ApplyFontScaleCommand(direction);
                else
                    SendFontScaleMessage(direction);
        }

        private static TextSettingDirection DirectionFromChar(char ch)
        {
            return ch switch
            {
                'u' => TextSettingDirection.Up,
                'd' => TextSettingDirection.Down,
                _ => throw new ArgumentOutOfRangeException(nameof(ch))
            };
        }

        [AssertionMethod]
        private static void AssertCanExecute(bool expected, IReactiveCommand cmd)
        {
            // If the command is currently executing, CanExecute will return
            // false even if the command _can_, theoretically, execute.
            // This is insurance to prevent heisenbugs occurring if we check
            // CanExecute before the command is done.
            //
            // Yes, this has happened in practice.
            cmd.IsExecuting.Where(x => !x).FirstAsync().Wait();
            Assert.Equal(expected, cmd.CanExecute.FirstAsync().Wait());
        }

        [AssertionMethod]
        private void AssertFontScaleEqual(int expected)
        {
            Assert.Equal(expected, _viewModel.FontScale);
        }

        private void ApplyFontScaleCommand(TextSettingDirection direction)
        {
            var command = CommandOfDirection(direction);
            command.Execute().Wait();
        }

        private ReactiveCommand<Unit, Unit> CommandOfDirection(TextSettingDirection direction)
        {
            return direction switch
            {
                TextSettingDirection.Down => _viewModel.DecreaseFontScale,
                TextSettingDirection.Up => _viewModel.IncreaseFontScale,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }

        private void SendFontScaleMessage(TextSettingDirection direction)
        {
            SendMessage(new TextSettingArgs(TextSetting.FontSize, direction));
        }

        /// <summary>
        ///     Checks that disposing the view model doesn't break things.
        /// </summary>
        [Fact]
        public void TestDispose_NoCrashes()
        {
            _viewModel.Dispose();
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
        ///     Tests that, after a sequence of text item loads, the most
        ///     recent text item 'wins'.
        /// </summary>
        [Fact]
        public void TestText_DoubleTextItemLoad()
        {
            const string text1 = "It was the best of times";
            const string text2 = "It was the worst of times";

            SendTextTrackMessage(text1);
            SendTextTrackMessage(text2);
            AssertTextEqual(text2);
        }

        /// <summary>
        ///     Tests that the text of the view model is empty by default.
        /// </summary>
        [Fact]
        public void TestText_Initial()
        {
            AssertTextEqual("");
        }
    }
}