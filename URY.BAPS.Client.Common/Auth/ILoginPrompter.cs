﻿namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     Interface for authenticator components that display loginPrompt prompts.
    /// </summary>
    public interface ILoginPrompter
    {
        /// <summary>
        ///     Asks the <see cref="ILoginPrompter"/> to prompt the user for
        ///     loginPrompt details.
        /// </summary>
        void Prompt();

        /// <summary>
        ///     Gets the <see cref="ILoginPromptResponse"/> corresponding to the last
        ///     call to <see cref="Prompt"/>.
        /// </summary>
        ILoginPromptResponse Response { get; }
    }
}