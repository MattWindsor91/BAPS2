using System.Linq;
using System.Reactive.Concurrency;
using Moq;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Playback;
using Xunit;
using static URY.BAPS.Common.Model.Playback.PlaybackState;

namespace URY.BAPS.Client.ViewModel.Tests
{
    public class PlayerTransportViewModelTests : ServerMessageViewModelTestsBase
    {
        public PlayerTransportViewModelTests()
        {
            _controller = Mock.Of<IPlaybackController>(x => x.PlaybackUpdater == Events);
            _viewModel = new PlayerTransportViewModel(_controller, Scheduler.Immediate);
        }


        private readonly IPlaybackController _controller;
        private readonly PlayerTransportViewModel _viewModel;

        /// <summary>
        ///     Summarises the state flags as a string.
        /// </summary>
        private string SummariseStateFlags {
            get
            {
                var playing = _viewModel.IsPlaying ? "*|>*" : " |> ";
                var paused = _viewModel.IsPaused ? "*||*" : " || ";
                var stopped = _viewModel.IsStopped ? "*[]*" : " [] ";
                return $"[{playing}] [{paused}] [{stopped}]";
            }
        }

        /// <summary>
        ///     Gets the expected output of <see cref="SummariseStateFlags"/>
        ///     for a particular state.
        /// </summary>
        /// <param name="state">The state whose summary we expect.</param>
        /// <returns>A summary string for <paramref name="state"/>.</returns>
        private static string ExpectedSummaryForState(PlaybackState state) => state switch
        {
            Playing => "[*|>*] [ || ] [ [] ]",
            Paused => "[ |> ] [*||*] [ [] ]",
            Stopped => "[ |> ] [ || ] [*[]*]",
            _ => "???"
        };

        /// <summary>
        ///     Theory data containing sequences of playback states to send
        ///     in the <see cref="TestState_AfterMessages" /> test.
        /// </summary>
        public static TheoryData<PlaybackState[]> StateSequences => new TheoryData<PlaybackState[]>
        {
            new[] {Playing},
            new[] {Paused},
            new[] {Stopped},
            new[] {Playing, Paused, Stopped},
            new[] {Stopped, Paused, Playing},
            new[] {Playing, Playing}
        };

        /// <summary>
        ///     Tests that the playback state changes after receiving a
        /// </summary>
        /// <param name="states">
        ///     The sequence of states to play.
        ///     The final
        ///     state will be the expected view model state.
        /// </param>
        [Theory]
        [MemberData(nameof(StateSequences))]
        public void TestState_AfterMessages(PlaybackState[] states)
        {
            foreach (var state in states) SendMessage(new PlaybackStateChangeArgs(0, state));

            AssertState(states.Last());
        }

        /// <summary>
        ///     Tests that the initial state, before receiving any other
        ///     information, is <see cref="Stopped" />.
        /// </summary>
        [Fact]
        public void TestState_Initial()
        {
            var state = Stopped;
            AssertState(state);
        }

        /// <summary>
        ///     Asserts that the view model's state flags are consistent with
        ///     a given state.
        ///     <para>
        ///         The assertion occurs by comparing string summaries of the
        ///         flags.  The reason this happens, instead of direct
        ///         Boolean testing, is so that the resulting test failure when
        ///         something goes wrong is a little more discoverable.
        ///     </para>
        /// </summary>
        /// <param name="state"></param>
        private void AssertState(PlaybackState state)
        {
            Assert.Equal(ExpectedSummaryForState(state), SummariseStateFlags);
        }
    }
}