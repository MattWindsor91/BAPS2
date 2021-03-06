﻿using JetBrains.Annotations;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests methods related to <see cref="PlaybackOp" /> and <see cref="PlaybackOpExtensions" />.
    ///     extensions.
    /// </summary>
    public class PlaybackOpTests
    {
        [UsedImplicitly]
        public static TheoryData<PlaybackState> PlaybackStateData =>
            new TheoryData<PlaybackState> {PlaybackState.Paused, PlaybackState.Playing, PlaybackState.Stopped};

        /// <summary>
        ///     Checks that converting a state to a playback operation, then back, does nothing.
        /// </summary>
        /// <param name="state">The state to test.</param>
        [Theory]
        [MemberData(nameof(PlaybackStateData))]
        public void TestChannelStateOpRoundTrip(PlaybackState state)
        {
            Assert.Equal(state, state.AsPlaybackOp().AsPlaybackState());
        }
    }
}