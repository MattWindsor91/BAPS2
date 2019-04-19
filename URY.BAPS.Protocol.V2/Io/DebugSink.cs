using System.Collections.Generic;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Protocol.V2.Io
{
    /// <inheritdoc />
    /// <summary>
    ///     A BapsNet sink that dumps each primitive item in a BapsNet command into an array.
    /// </summary>
    public class DebugSink : ISink
    {
        private readonly Queue<object> _items = new Queue<object>();

        public object[] Items => _items.ToArray();

        public void SendCommand(CommandWord cmd)
        {
            _items.Enqueue(cmd);
        }

        public void SendString(string s)
        {
            _items.Enqueue(s);
        }

        public void SendFloat(float f)
        {
            _items.Enqueue(f);
        }

        public void SendUint(uint i)
        {
            _items.Enqueue(i);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}