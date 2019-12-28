using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;

namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     Interface for objects that, given a response from a login prompt,
    ///     attempt to perform a login against a BAPS client.
    /// </summary>
    /// <typeparam name="TConn"></typeparam>
    public interface ILoginAttempter<out TConn> where TConn : class
    {
        TConn? Connection { get; }

        ILoginResult Attempt(ILoginPromptResponse promptResponse);
    }
}