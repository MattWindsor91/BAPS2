using System;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Encode;
using MessageIo = URY.BAPS.Common.Protocol.V2.MessageIo;
using PrimitiveIo = URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Server.Io
{
    /// <summary>
    ///     A handle for a particular BAPS server client.
    /// </summary>
    public sealed class ClientHandle : MessageIo.IConnection
    {
        /// <summary>
        ///     The message-IO connection to the client.
        /// </summary>
        private readonly MessageIo.IConnection _connection;

        public ClientHandle(MessageIo.IConnection connection)
        {
            _connection = connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        public IFullEventFeed EventFeed => _connection.EventFeed;

        public void Send(MessageBuilder? messageBuilder)
        {
            _connection.Send(messageBuilder);
        }
    }
}
