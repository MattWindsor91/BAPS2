﻿using NUnit.Framework;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Tests
{
    /// <summary>
    ///     Tests methods related to <see cref="PlaybackState" />.
    /// </summary>
    [TestFixture]
    public class ChannelStateTests
    {
        /// <summary>
        ///     Checks that converting a state to a command, then back, does nothing.
        /// </summary>
        /// <param name="state">The state to test.</param>
        [Test]
        [Combinatorial]
        public void TestChannelStateCommandRoundTrip([Values] PlaybackState state)
        {
            Assert.That(state.AsCommand().AsPlaybackState(), Is.EqualTo(state));
        }
    }
}