using System.Net.Sockets;

namespace URY.BAPS.Common.Infrastructure.Login
{
    /// <summary>
    ///     Interface for objects that handle one side of the BAPS login procedure.
    ///     <para>
    ///         'Login', in this case, is a catch-all term for the entire process from starting a TCP connection
    ///         to bringing up a correctly authenticated message-level wrapper over it.
    /// </summary>
    public interface ILoginPerformer<out TConn>
    {
        /// <summary>
        ///     The most recently authenticated connection.
        /// </summary>
        public TConn Connection { get; }
 
        /// <summary>
        ///     Whether the last call to <see cref="Run"/> was successful.
        /// </summary>
        public bool HasConnection { get; }

        /// <summary>
        ///     Tries to construct an authenticated <see cref="TAuthConn" />.  If successful,
        ///     <see cref="HasConnection"/> will be <c>true</c>, and <see cref="Connection"/> will point to the
        ///     authenticated connection.
        /// </summary>
        public void Run(TcpClient client);
    }
}