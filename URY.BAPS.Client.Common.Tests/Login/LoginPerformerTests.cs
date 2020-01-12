using System.Collections.Generic;
using System.Net.Sockets;
using System.Reactive;
using JetBrains.Annotations;
using Moq;
using URY.BAPS.Client.Common.Login;
using URY.BAPS.Client.Common.Login.LoginResult;
using URY.BAPS.Client.Common.Login.Prompt;
using Xunit;

namespace URY.BAPS.Client.Common.Tests.Login
{
    /// <summary>
    ///     Tests covering the whole <see cref="ClientSideLoginPerformer{TRawConn,TAuthConn}" />.
    /// </summary>
    public class LoginPerformerTests
    {
        private readonly IList<ILoginResult> _errors = new List<ILoginResult>();
    
        private readonly Mock<IHandshakePerformer<string>> _handshakePerformerMock = new Mock<IHandshakePerformer<string>>();
        private readonly Mock<IAuthPrompter> _authPrompterMock = new Mock<IAuthPrompter>();
        private readonly Mock<ILoginErrorHandler> _loginErrorHandlerMock = new Mock<ILoginErrorHandler>();
        private readonly Mock<IAuthPerformer<string, string>> _authPerformerMock = new Mock<IAuthPerformer<string, string>>();

        private ClientSideLoginPerformer<string,string> MakeLoginPerformer(string connection, IAuthPromptResponse response, ILoginResult result)
        {
            _authPrompterMock.SetupGet(t => t.Response).Returns(response);
            _authPerformerMock.SetupGet(t => t.Connection).Returns(connection);
            _authPerformerMock.SetupGet(t => t.Result).Returns(result);
            _loginErrorHandlerMock.Setup(t => t.Handle(It.IsAny<ILoginResult>())).Callback(new InvocationAction(RegisterLoginErrorHandlerCall) );
            
            return new ClientSideLoginPerformer<string,string>(_handshakePerformerMock.Object, _authPrompterMock.Object, _loginErrorHandlerMock.Object, _authPerformerMock.Object);
        }

        private void RegisterLoginErrorHandlerCall(IInvocation invocation)
        {
            _errors.Add((ILoginResult)invocation.Arguments[0]);
        }

        [Pure]
        private static string SummariseResult(ILoginResult r)
        {
            return string.Join('|', r.IsSuccess ? "S" : "s", r.IsFatal ? "D" : "d", r.IsUserFault ? "U" : "u",
                r.Description);
        }

        [AssertionMethod]
        private void AssertOneErrorMatching(string summary)
        {
            Assert.Collection(_errors, result => Assert.Equal(summary, SummariseResult(result)));
        }

        [Fact]
        public void TestPromptQuit()
        {
            var auth = MakeLoginPerformer("foo", new QuitAuthPromptResponse(), new SuccessLoginResult());
            
            // TcpClients without host/port in constructor don't make any connections, and the bits of the login
            // performer we're testing don't require one.
            auth.TryLogin(new TcpClient());
            
            AssertOneErrorMatching("s|D|U|User quit the login prompt.");
        }

        // TODO(@MattWindsor91): the current authenticator design makes this test loop infinitely!
#if false
        [Fact]
        public void TestUserFailure()
        {
            var auth =
 MakeLoginPerformer("foo", new AuthPromptResponse("foo", "bar", "localhost", 1350), new UserLoginException("Invalid password."));
            _ = auth.TryLogin();
            AssertOneErrorMatching("s|d|U|Invalid password.");
        }
#endif
    }
}