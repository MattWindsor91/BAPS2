namespace URY.BAPS.Client.Common.Auth.Prompt
{
    /// <summary>
    ///     A <see cref="ILoginPrompter"/> that does nothing when asked to
    ///     display a prompt,
    ///     and always returns the same <see cref="ILoginPromptResponse"/>
    ///     when asked.
    /// </summary>
    public class DummyLoginPrompter : ILoginPrompter
    {
        /// <summary>
        ///     Constructs a dummy login prompter with a particular stock
        ///     response.
        /// </summary>
        /// <param name="response">The response to give when asked.</param>
        public DummyLoginPrompter(ILoginPromptResponse response)
        {
            Response = response;
        }

        public void Prompt()
        {
        }

        public ILoginPromptResponse Response { get; }
    }
}