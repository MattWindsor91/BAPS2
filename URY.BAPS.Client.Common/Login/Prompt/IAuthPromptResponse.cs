namespace URY.BAPS.Client.Common.Login.Prompt
{
    public interface IAuthPromptResponse
    {
        /// <summary>
        ///     Returns whether the <see cref="IAuthPrompter"/> received
        ///     valid credentials.
        /// </summary>
        bool HasCredentials { get; }

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
    }
}