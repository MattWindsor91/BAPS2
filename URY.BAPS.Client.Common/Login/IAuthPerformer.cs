using URY.BAPS.Client.Common.Login.LoginResult;
using URY.BAPS.Client.Common.Login.Prompt;
using URY.BAPS.Common.Infrastructure;

namespace URY.BAPS.Client.Common.Login
{
    /// <summary>
    ///     Interface for objects that, given a response from a login prompt,
    ///     attempt to perform a login against a BAPS client.
    /// </summary>
    /// <typeparam name="TRawConn">
    ///     Type of raw (usually primitive-level) connection.
    /// </typeparam>
    /// <typeparam name="TAuthConn">
    ///     Type of authenticated (usually message-level) connection.
    /// </typeparam>
    public interface IAuthPerformer<in TRawConn, out TAuthConn> where TAuthConn : IMessageConnection
    {
        /// <summary>
        ///     A fallback connection value for use when part of the login process fails.  This should have that
        ///     <code>IsConnected == false</code>.
        /// </summary>
        TAuthConn Fallback { get; }
    
        /// <summary>
        ///     Attempts to authenticate a connection.
        /// </summary>
        /// <param name="conn">The raw connection that will be promoted to an authenticated connection on success.</param>
        /// <param name="promptResponse">The result of asking the user to authenticate.</param>
        /// <returns>
        ///     An authenticated connection object.
        /// </returns>
        /// <remarks>
        ///     The caller should take ownership of any <typeparamref name="TAuthConn"/> returned, and take
        ///     responsibility for disposing it if it is disposable.
        /// </remarks>
        TAuthConn Attempt(TRawConn conn, IAuthPromptResponse promptResponse);
    }
}