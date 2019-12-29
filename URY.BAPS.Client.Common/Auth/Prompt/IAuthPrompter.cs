namespace URY.BAPS.Client.Common.Auth.Prompt
{
    /// <summary>
    ///     Interface for authenticator components that display login prompts.
    /// </summary>
    public interface IAuthPrompter
    {
        /// <summary>
        ///     Asks the <see cref="IAuthPrompter"/> to prompt the user for
        ///     login details.
        /// </summary>
        void Prompt();

        /// <summary>
        ///     Gets the <see cref="IAuthPromptResponse"/> corresponding to the last
        ///     call to <see cref="Prompt"/>.
        /// </summary>
        IAuthPromptResponse Response { get; }
    }
}