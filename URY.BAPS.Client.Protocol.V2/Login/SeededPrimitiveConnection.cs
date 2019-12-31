using System;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Client.Protocol.V2.Login
{
    /// <summary>
    ///     A primitive-level connection, wrapped up with the seed that was
    ///     received from the handshaking process.
    /// </summary>
    public sealed class SeededPrimitiveConnection : IDisposable
    {
        public void Deconstruct(out string seed, out IPrimitiveConnection connection)
        {
            seed = Seed;
            connection = Connection;
        }

        public SeededPrimitiveConnection(string seed, IPrimitiveConnection connection)
        {
            Seed = seed;
            Connection = connection;
        }

        public string Seed { get; }
        public IPrimitiveConnection Connection { get; }

        /// <summary>
        ///     Provides a disconnected <see cref="SeededPrimitiveConnection"/>.
        /// </summary>
        /// <returns>
        ///     A connection with no seed that will fail whenever used.
        /// </returns>
        public static SeededPrimitiveConnection Invalid()
        {
            return new SeededPrimitiveConnection("", new DeadPrimitiveConnection());
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}