using System.Collections.Generic;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <inheritdoc />
    /// <summary>
    ///     A BapsNet sink that dumps each primitive item in a BapsNet command into an array.
    /// </summary>
    public class DebugPrimitiveSink : IPrimitiveSink
    {
        private readonly Queue<object> _itemQueue = new Queue<object>();

        public object[] Items { get; private set; } = { };

        public void SendCommand(ushort cmd)
        {
            _itemQueue.Enqueue(cmd);
        }

        public void SendString(string s)
        {
            _itemQueue.Enqueue(s);
        }

        public void SendFloat(float f)
        {
            _itemQueue.Enqueue(f);
        }

        public void SendUint(uint i)
        {
            _itemQueue.Enqueue(i);
        }


        public void Flush()
        {
            Items = _itemQueue.ToArray();
        }
    }
}