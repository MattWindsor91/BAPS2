using System;
using System.Reactive.Linq;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Encode;

namespace URY.BAPS.Common.Protocol.V2.MessageIo
{
    /// <summary>
    ///     A message-level connection that can't send, and receives no events.
    /// </summary>
    public sealed class DeadMessageConnection : IMessageConnection
    {
        public void Dispose()
        {
        }

        public IObservable<MessageArgsBase> RawEventFeed => Observable.Empty<MessageArgsBase>();

        public void StartLoops()
        {
            throw new InvalidOperationException("Can't start loops on a dead connection");
        }

        public void StopLoops()
        {
            throw new InvalidOperationException("Can't stop loops on a dead connection");
        }

        public void Send(MessageBuilder? messageBuilder)
        {
            throw new InvalidOperationException("Can't send on a dead connection");
        }
    }
}