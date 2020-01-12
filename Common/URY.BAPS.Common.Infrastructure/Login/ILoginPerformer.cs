using System.Net.Sockets;

namespace URY.BAPS.Common.Infrastructure.Login
{
    /// <summary>
    ///     Interface for objects that handle one side of the BAPS login procedure.
    ///     <para>
    ///         'Login', in this case, is a catch-all term for the entire process from starting a TCP connection
    ///         to bringing up a correctly authenticated message-level wrapper over it.
    /// </summary>
    public interface ILoginPerformer<TConn>
    {
        /// <summary>
        ///     Tries to construct an authenticated <see cref="TConn" />.
        /// </summary>
        /// <param name="client">
        ///     The incoming TCP client (must be non-null, and generally should be pre-connected).
        /// </param>
        /// <param name="conn">
        ///     If the login is successful, this variable receives the new connection.
        /// </param>
        /// <returns>
        ///     Whether the login was successful.
        /// </returns>
        public bool TryLogin(TcpClient client, out TConn conn);
    }
}