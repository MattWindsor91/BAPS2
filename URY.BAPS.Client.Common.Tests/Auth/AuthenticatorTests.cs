using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;
using Xunit;

namespace URY.BAPS.Client.Common.Tests.Auth
{
    /// <summary>
    ///     Tests covering the whole <see cref="LoginPerformer{TRawConn,TAuthConn}" />.
    /// </summary>
    public class AuthenticatorTests
    {
        private readonly DebugLoginErrorHandler _errorHandler = new DebugLoginErrorHandler();

        private LoginPerformer<,> MakeAuthenticator<T>(T connection, IAuthPromptResponse response, ILoginResult result)
            where T : class
        {
            var prompter = new DummyAuthPrompter(response);
            var builder = new DummyAuthPerformer<T>(connection, result);
            return new LoginPerformer<,>(prompter, _errorHandler, builder);
        }

        private string SummariseResult(ILoginResult r)
        {
            return string.Join('|', r.IsSuccess ? "S" : "s", r.IsFatal ? "D" : "d", r.IsUserFault ? "U" : "u",
                r.Description);
        }

        private void AssertOneResultMatching(string summary)
        {
            Assert.Collection(_errorHandler.Results, result => Assert.Equal(summary, SummariseResult(result)));
        }

        [Fact]
        public void TestPromptQuit()
        {
            var auth = MakeAuthenticator("foo", new QuitAuthPromptResponse(), new SuccessLoginResult());
            _ = auth.Run();
            AssertOneResultMatching("s|D|U|User quit the login prompt.");
        }

        // TODO(@MattWindsor91): the current authenticator design makes this test loop infinitely!
#if false
        [Fact]
        public void TestUserFailure()
        {
            var auth =
 MakeAuthenticator("foo", new AuthPromptResponse("foo", "bar", "localhost", 1350), new UserFailureLoginResult("Invalid password."));
            _ = auth.Run();
            AssertOneResultMatching("s|d|U|Invalid password.");
        }
#endif
    }
}