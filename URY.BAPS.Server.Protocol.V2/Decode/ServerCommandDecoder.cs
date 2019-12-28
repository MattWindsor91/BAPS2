using System;
using System.Threading;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Model;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Server.Protocol.V2.Decode
{
    /// <summary>
    ///     A decoder for the client side of the BapsNet protocol.
    ///     <para>
    ///         On commands whose shape differs based on whether the receiver
    ///         is on the client or the server, this decoder implements the
    ///         client (so any commands it receives are server responses).
    ///     </para>
    /// </summary>
    public class ServerCommandDecoder : CommandDecoder
    {
        public ServerCommandDecoder(IPrimitiveSource primitiveSource, CancellationToken token = default) : base(primitiveSource, token)
        {
        }

        protected override void DecodeItem(byte channelId)
        {
            // WORK NEEDED: getItem
        }

        protected override void DecodeServerVersion()
        {
            // The server version of this command has no arguments.
            Dispatch(new SystemRequestArgs(SystemRequest.GetVersion));
        }

        protected override void DecodeFeedback()
        {
            var feedback = ReceiveString();
            Dispatch(new FeedbackRequestArgs(feedback));
        }
    }
}