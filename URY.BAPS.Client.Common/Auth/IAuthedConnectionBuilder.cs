using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;

namespace URY.BAPS.Client.Common.Auth
{
    public interface IAuthedConnectionBuilder<out TConn> where TConn : class
    {
        TConn? Connection { get; }

        ILoginResult Attempt(ILoginPromptResponse promptResponse);
    }
}