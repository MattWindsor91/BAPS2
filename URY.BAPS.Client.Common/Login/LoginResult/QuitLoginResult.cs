using URY.BAPS.Client.Common.Login.Prompt;

namespace URY.BAPS.Client.Common.Login.LoginResult
{
    /// <summary>
    ///     Login result used when the user quits a login attempt.
    ///     <para>
    ///         This usually corresponds to a <see cref="QuitAuthPromptResponse"/>
    ///         further up.
    ///     </para>
    /// </summary>
    public class QuitLoginResult : ILoginResult
    {
        public bool IsSuccess => false;
        public bool IsFatal => true;
        public bool IsUserFault => true;
        public string Description => "User quit the login prompt.";
    }
}