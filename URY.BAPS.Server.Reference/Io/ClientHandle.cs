using System;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Encode;
using MessageIo = URY.BAPS.Common.Protocol.V2.MessageIo;
using PrimitiveIo = URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Server.Io
{
    /// <summary>
    ///     A handle for a particular BAPS server client.
    /// </summary>
    public sealed class ClientHandle
    {
        /// <summary>
        ///     The message-IO connection to the client.
        /// </summary>
        private readonly MessageIo.IMessageConnection _messageConnection;

        public ClientHandle(MessageIo.IMessageConnection messageConnection)
        {
            _messageConnection = messageConnection;
        }

        public IObservable<MessageArgsBase> RawEventFeed => _messageConnection.RawEventFeed;

        public void Dispose()
        {
            _messageConnection?.Dispose();
        }

        public void Send(MessageBuilder? messageBuilder)
        {
            _messageConnection.Send(messageBuilder);
        }
    }
}