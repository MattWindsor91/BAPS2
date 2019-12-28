using System.Threading;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Model;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Client.Protocol.V2.Decode
{
    /// <summary>
    ///     A decoder for the client side of the BapsNet protocol.
    ///     <para>
    ///         On commands whose shape differs based on whether the receiver
    ///         is on the client or the server, this decoder implements the
    ///         client (so any commands it receives are server responses).
    ///     </para>
    /// </summary>
    public class ClientCommandDecoder : CommandDecoder
    {
        public ClientCommandDecoder(IPrimitiveSource primitiveSource, CancellationToken token = default) : base(primitiveSource, token)
        {
        }

        protected override void DecodeItem(byte channelId)
        {
            // The client version of this command gets the item information.
            var position = ReceiveUint();
            var index = new TrackIndex {ChannelId = channelId, Position = position};
            
            var type = DecodeTrackType();
            var description = ReceiveString();
            var entry = TrackFactory.Create(type, description);
            Dispatch(new TrackAddArgs(index, entry));
        }

        protected override void DecodeServerVersion()
        {
            // The client version of this command gets the version information.
            var version = ReceiveString();
            var date = ReceiveString();
            var time = ReceiveString();
            var author = ReceiveString();
            var sv = new ServerVersion(version, date, time, author);
            Dispatch(new ServerVersionArgs(sv));
        }

        protected override void DecodeFeedback()
        {
            // The client version of this command gets whether the feedback it
            // previously requested was sent, as a C-style Boolean.
            var wasSent = ReceiveUint() != 0;
            Dispatch(new FeedbackResponseArgs(wasSent));
        }
    }
}