using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.PrimitiveIo
{
    /// <summary>
    ///     A BapsNet primitive source that reads primitive items from a queue.
    /// </summary>
    public class DebugPrimitiveSource : IPrimitiveSource
    {
        private readonly Queue<object> _itemQueue = new Queue<object>();

        public ushort ReceiveCommand(CancellationToken token = default)
        {
            
            throw new System.NotImplementedException("Can't receive commands from this source.");
        }

        public string ReceiveString(CancellationToken token = default)
        {
            return (string) _itemQueue.Dequeue();
        }

        public float ReceiveFloat(CancellationToken token = default)
        {
            return (float) _itemQueue.Dequeue();
        }

        public uint ReceiveUint(CancellationToken token = default)
        {
            return (uint) _itemQueue.Dequeue();
        }

        /// <summary>
        ///     Adds a string onto the queue of available items.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public DebugPrimitiveSource AddString(string s)
        {
            _itemQueue.Enqueue(s);
            return this;
        }
        
        public DebugPrimitiveSource AddFloat(float f)
        {
            _itemQueue.Enqueue(f);
            return this;
        }       
        
        public DebugPrimitiveSource AddUint(uint i)
        {
            _itemQueue.Enqueue(i);
            return this;
        }              
    }
}