using System;
using System.Threading;

namespace URY.BAPS.Common.Protocol.V2.PrimitiveIo
{
    /// <summary>
    ///     An <see cref="IPrimitiveConnection"/> that fails whenever it is used.
    /// </summary>
    public sealed class DeadPrimitiveConnection : IPrimitiveConnection
    {
        private static Exception Fail()
        {
            return new InvalidOperationException("Tried to use a dead connection");
        }
        
        public ushort ReceiveCommand(CancellationToken token = default)
        {
            throw Fail();
        }

        public string ReceiveString(CancellationToken token = default)
        {
            throw Fail();
        }

        public float ReceiveFloat(CancellationToken token = default)
        {
            throw Fail();
        }

        public uint ReceiveUint(CancellationToken token = default)
        {
            throw Fail();
        }

        public void SendCommand(ushort cmd)
        {
            throw Fail();
        }

        public void SendString(string s)
        {
            throw Fail();
        }

        public void SendFloat(float f)
        {
            throw Fail();
        }

        public void SendUint(uint i)
        {
            throw Fail();
        }

        public void Flush()
        {
        }

        public void Dispose()
        {
        }
    }
}