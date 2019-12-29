using System.Net.Sockets;

namespace URY.BAPS.Client.Common.ServerSelect
{
    /// <summary>
    ///     Interface of objects that handle errors from a <see cref="ServerSelector"/>.
    /// </summary>
    public interface IServerErrorHandler
    {
        /// <summary>
        ///     Handles a user abort of the server selection process.
        /// </summary>
        void HandleGivingUp();

        /// <summary>
        ///     Handles an attempt to select a server whose record is not valid.
        /// </summary>
        /// <param name="invalid">The invalid server record causing the error.</param>
        void HandleInvalidServer(ServerRecord invalid);

        /// <summary>
        ///     Handles a socket exception.
        /// </summary>
        /// <param name="exc">The exception causing the server connection to fail.</param>
        void HandleSocketException(SocketException exc);
    }
}