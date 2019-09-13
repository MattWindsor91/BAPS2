using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.LoginResult;

namespace URY.BAPS.Client.Console
{
    /// <summary>
    ///     A <see cref="ILoginErrorHandler"/> that dumps errors onto standard
    ///     error.
    /// </summary>
    public class ConsoleLoginErrorHandler : ILoginErrorHandler
    {
        public void Handle(ILoginResult result)
        {
            System.Console.Error.WriteLine($"{BlameString(result)} error: {result.Description}");
        }

        private static string BlameString(ILoginResult result)
        {
            return result.IsUserFault ? "User" : "Server";
        }
    }
}
