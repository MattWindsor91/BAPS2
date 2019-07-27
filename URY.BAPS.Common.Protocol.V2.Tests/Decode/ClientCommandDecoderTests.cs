using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.ServerConfig;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Model;
using URY.BAPS.Common.Protocol.V2.Ops;
using URY.BAPS.Common.Protocol.V2.Tests.Utils;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Decode
{
    /// <summary>
    ///     Tests for the <see cref="ClientCommandDecoder"/>, including any decoding logic common to both client
    ///     and server.
    /// </summary>
    public class ClientCommandDecoderTests
    {
        private readonly DebugPrimitiveSource _primitiveSource = new DebugPrimitiveSource(); 
        private readonly ClientCommandDecoder _decoder;
        
        public ClientCommandDecoderTests()
        {
            _decoder = new ClientCommandDecoder(_primitiveSource, CancellationToken.None);
        }
        
        /// <summary>
        ///     Stock channel ID used for playback/playlist command decoding tests.
        /// </summary>
        private const byte ChannelId = 42;

        /// <summary>
        ///     Timeout used when waiting for a message to arrive from the decoder.
        /// </summary>
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
        
        private T DecodeAndAssertMessageType<T>(ICommand c) where T : MessageArgsBase
        {
            // Use a timeout to avoid tests from hanging indefinitely if there isn't a message.
            var task = _decoder.ObserveMessage.FirstAsync().Timeout(Timeout).ToTask();
            c.Accept(_decoder);
            var message = task.Result;
            // We could also do the conversion in the observable, but doing it here
            // makes failures a bit more obvious.
            return Assert.IsAssignableFrom<T>(message);
            
        }

        [UsedImplicitly]
        public static TheoryData<PlaybackState> PlaybackStateData =
            new TheoryData<PlaybackState>
            {
                PlaybackState.Paused, PlaybackState.Playing, PlaybackState.Stopped
            };

        /// <summary>
        ///     Tests that playback state change updates from the server are
        ///     decoded properly.
        /// </summary>
        /// <param name="state">The playback state under test.</param>
        [Theory, MemberData(nameof(PlaybackStateData))]
        public void TestDecode_PlaybackStateChange(PlaybackState state)
        {
            var command = new PlaybackCommand(state.AsPlaybackOp(), ChannelId);
            
            var message = DecodeAndAssertMessageType<PlaybackStateChangeArgs>(command);

            Assert.Equal(ChannelId, message.ChannelId);
            Assert.Equal(state, message.State);
        }
        
        [UsedImplicitly]
        public static TheoryData<MarkerType> MarkerTypeData =
            new TheoryData<MarkerType>
            {
                MarkerType.Cue, MarkerType.Intro, MarkerType.Position
            };
       
        /// <summary>
        ///     Tests that marker position change updates from the server are
        ///     decoded properly.
        /// </summary>
        /// <param name="marker">The marker under test.</param>
        [Theory, MemberData(nameof(MarkerTypeData))]
        public void TestDecode_MarkerChange(MarkerType marker)
        {
            var command = new PlaybackCommand(marker.AsPlaybackOp(), 42);
            _primitiveSource.AddUint(1001);
            
            var message = DecodeAndAssertMessageType<MarkerChangeArgs>(command);

            Assert.Equal(ChannelId, message.ChannelId);
            Assert.Equal(marker, message.Marker);
            Assert.Equal(1001U, message.NewValue);
        }

        /// <summary>
        ///     Tests to see if the decoder handles track
        ///     <see cref="CountArgs"/> messages properly.
        /// </summary>       
        [Fact]
        public void TestDecode_ItemCount()
        {
            var command = new PlaylistCommand(PlaylistOp.Item, ChannelId);
            const uint count = 500;
            _primitiveSource.AddUint(count);
            
            var message = DecodeAndAssertMessageType<CountArgs>(command);

            Assert.Equal(CountType.PlaylistItem, message.Type);
            Assert.Equal(count, message.Count);
            Assert.Equal(ChannelId, message.Index);
        }

        /// <summary>
        ///     Tests to see if the decoder handles
        ///     <see cref="TrackAddArgs"/> messages properly.
        /// </summary>       
        [Fact]
        public void TestDecode_Item()
        {
            const uint index = 30U;
            const TrackType type = TrackType.Library;
            const string description = "Abacab";
            
            var command = new PlaylistCommand(PlaylistOp.Item, ChannelId, true);
            _primitiveSource.AddUint(index).AddUint((uint) type).AddString(description);
            
            var message = DecodeAndAssertMessageType<TrackAddArgs>(command);

            Assert.Equal(ChannelId, message.ChannelId);
            Assert.Equal(index, message.Index);
            Assert.Equal("e|l|A|I|t|Abacab|0|", message.Item.Summarise());
        }

        /// <summary>
        ///     Tests to see if the decoder handles
        ///     <see cref="ServerVersionArgs"/> messages properly.
        /// </summary>
        [Fact]
        public void TestDecode_ServerVersion()
        {
            const string version = "Test v1.3.37";
            const string date = "1970-01-01";
            const string time = "00:00:00";
            const string author = "Mr Website";
            
            var command = new SystemCommand(SystemOp.ServerVersion);
            _primitiveSource.AddString(version).AddString(date).AddString(time).AddString(author);

            var message = DecodeAndAssertMessageType<ServerVersionArgs>(command);
            var serverVersion = message.Version;

            Assert.Equal(version, serverVersion.Version);
            Assert.Equal(date, serverVersion.Date);
            Assert.Equal(time, serverVersion.Time);
            Assert.Equal(author, serverVersion.Author);
        }

        // We currently don't support the feedback command, and so
        // it isn't tested here.
    }
}