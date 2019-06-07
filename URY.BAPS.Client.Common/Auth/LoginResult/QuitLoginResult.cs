using URY.BAPS.Client.Common.Auth.Prompt;

namespace URY.BAPS.Client.Common.Auth.LoginResult
{
    /// <summary>
    ///     Login result used when the user quits a login attempt.
    ///     <para>
    ///         This usually corresponds to a <see cref="QuitLoginPromptResponse"/>
    ///         further up.
    ///     </para>
    /// </summary>
    public class QuitLoginResult : ILoginResult
    {
        public bool IsSuccess => false;
        public bool IsDone => true;
        public bool IsUserFault => true;
        public string Description => "User quit the login prompt.";
    }
}