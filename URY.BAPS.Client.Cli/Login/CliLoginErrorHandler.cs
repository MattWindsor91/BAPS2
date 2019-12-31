using URY.BAPS.Client.Common.Login;
using URY.BAPS.Client.Common.Login.LoginResult;

namespace URY.BAPS.Client.Cli.Login
{
    /// <summary>
    ///     A <see cref="ILoginErrorHandler"/> that dumps errors onto standard
    ///     error.
    /// </summary>
    public class CliLoginErrorHandler : ILoginErrorHandler
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
