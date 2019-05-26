using JetBrains.Annotations;

namespace URY.BAPS.Client.Common.Auth
{
    public interface ILoginPromptResponse
    {
        /// <summary>
        ///     Returns whether the <see cref="ILoginPrompter"/> received
        ///     valid credentials.
        /// </summary>
        bool HasCredentials { get; }

        /// <summary>
        ///     Checks whether this loginPrompt promptResponse is targeting a different
        ///     server from another promptResponse.
        /// </summary>
        /// <param name="other">
        ///     The other promptResponse to check.
        /// </param>
        /// <returns>
        ///     True if the responses' servers are different.
        ///     This method may over-approximate (for example, if one
        ///     promptResponse failed and the other succeeded, then this should
        ///     return true).
        /// </returns>
        [Pure]
        bool IsDifferentServer(ILoginPromptResponse other);

        /// <summary>
        ///     If <see cref="HasCredentials" /> is true, this contains
        ///     the user's requested username.
        /// </summary>
        string Username { get; }

        /// <summary>
        ///     If <see cref="HasCredentials" /> is true, this contains
        ///     the user's requested password (caution: in plain text).
        /// </summary>
        string Password { get; }

        /// <summary>
        ///     If <see cref="HasCredentials" /> is true, this contains
        ///     the user's requested server host.
        /// </summary>
        string Host { get; }

        /// <summary>
        ///     If <see cref="HasCredentials" /> is true, this contains
        ///     the user's requested server port.
        /// </summary>
        int Port { get; }
    }
}