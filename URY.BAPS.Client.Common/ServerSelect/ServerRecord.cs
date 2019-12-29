using System;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace URY.BAPS.Client.Common.ServerSelect
{
    /// <summary>
    ///     A record containing configuration for a BAPS server.
    /// </summary>
    public sealed class ServerRecord
    {
        /// <summary>
        ///     The human-readable name of the server.
        /// </summary>
        public string Name { get; set; } = "<untitled>";
        
        /// <summary>
        ///     The hostname at which the server resides.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     The TCP port at which the server resides.
        /// </summary>
        public int Port { get; set; } = 1350;

        /// <summary>
        ///     The colour hint for the server, if any.
        /// </summary>
        public ServerColour Colour { get; set; }

        /// <summary>
        ///     Constructs an invalid, 'empty' server record.
        /// </summary>
        /// <returns>A placeholder server record, for which <see cref="IsValid"/> will return <c>false</c>.</returns>
        [Pure]
        public static ServerRecord Empty()
        {
            // This method mainly exists in case we change the representation of server records such that the default
            // constructor no longer returns an invalid record.
            return new ServerRecord();
        }

        /// <summary>
        ///     Returns a (vaguely) human-readable string representation of this record.
        /// </summary>
        /// <returns>
        ///     A string representing the record, containing its name, host, port, and colour.
        /// </returns>
        public override string ToString()
        {
            return $"{Name}[{Colour}]@{Host}:{Port}";
        }

        /// <summary>
        ///     Checks whether this server record is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                // Ideally, this shouldn't deliberately try to trip an exception, but I'm unsure of any easier way to
                // do this check.
                try
                {
                    _ = new DnsEndPoint(Host, Port);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Tries to connect to the server named by this <see cref="ServerRecord"/>.
        /// </summary>
        /// <returns>A <see cref="TcpClient"/> representing the low-level server connection.</returns>
        public TcpClient Dial()
        {
            // We could require IsValid to be true beforehand, but it's likely that the TcpClient constructor does the
            // exact same checks.
            return new TcpClient(Host, Port);
        }
    }
}