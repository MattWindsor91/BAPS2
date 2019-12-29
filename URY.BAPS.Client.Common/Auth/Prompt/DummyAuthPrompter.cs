namespace URY.BAPS.Client.Common.Auth.Prompt
{
    /// <summary>
    ///     A <see cref="IAuthPrompter"/> that does nothing when asked to
    ///     display a prompt,
    ///     and always returns the same <see cref="IAuthPromptResponse"/>
    ///     when asked.
    /// </summary>
    public class DummyAuthPrompter : IAuthPrompter
    {
        /// <summary>
        ///     Constructs a dummy login prompter with a particular stock
        ///     response.
        /// </summary>
        /// <param name="response">The response to give when asked.</param>
        public DummyAuthPrompter(IAuthPromptResponse response)
        {
            Response = response;
        }

        public void Prompt()
        {
        }

        public IAuthPromptResponse Response { get; }
    }
}