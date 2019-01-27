using BAPSClientCommon.Model;
using NUnit.Framework;

namespace BAPSClientCommonTests
{
    /// <summary>
    ///     Tests methods related to <see cref="ChannelState"/>.
    /// </summary>
    [TestFixture]
    public class ChannelStateTests
    {
        /// <summary>
        ///     Checks that converting a state to a command, then back, does nothing.
        /// </summary>
        /// <param name="state">The state to test.</param>
        [Test, Combinatorial]
        public void TestChannelStateCommandRoundTrip([Values] ChannelState state)
        {
            Assert.That(state.AsCommand().AsChannelState(), Is.EqualTo(state));
        }
    }
}