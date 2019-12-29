using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;

namespace URY.BAPS.Client.Common.Auth
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
    public interface IAuthPerformer<in TRawConn, out TAuthConn>
    {
        /// <summary>
        ///     The last authenticated connection.
        /// </summary>
        TAuthConn Connection { get; }

        /// <summary>
        ///     The result of the last authentication attempt.
        /// </summary>
        ILoginResult Result { get; }
        
        void Attempt(TRawConn conn, IAuthPromptResponse promptResponse);
    }
}