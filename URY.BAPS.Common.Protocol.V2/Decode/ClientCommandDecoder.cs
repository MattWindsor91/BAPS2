using System.Threading;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Model;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    /// <summary>
    ///     A decoder for the client side of the BapsNet protocol.
    ///     <para>
    ///         On commands whose shape differs based on whether the receiver
    ///         is on the client or the server, this decoder implements the
    ///         client (so any commands it receives are server responses).
    ///     </para>
    /// </summary>
    public class ClientCommandDecoder : CommandDecoderBase
    {
        public ClientCommandDecoder(IPrimitiveSource primitiveSource, CancellationToken token = default) : base(primitiveSource, token)
        {
        }

        protected override void DecodeItem(byte channelId)
        {
            // The client version of this command gets the item information.
            var index = ReceiveUint();
            var type = DecodeTrackType();
            var description = ReceiveString();
            var entry = TrackFactory.Create(type, description);
            Dispatch(new TrackAddArgs(channelId, index, entry));
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
            // The client version of this command just contains a 'yes/no' result.
            _ = ReceiveUint();
        }
    }
}