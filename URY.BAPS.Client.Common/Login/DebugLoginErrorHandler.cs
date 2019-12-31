using System.Collections.Generic;
using URY.BAPS.Client.Common.Login.LoginResult;

namespace URY.BAPS.Client.Common.Login
{
    /// <summary>
    ///     A simple login error handler that appends each received error to
    ///     a publicly available queue.
    /// </summary>
    public class DebugLoginErrorHandler : ILoginErrorHandler
    {
        public Queue<ILoginResult> Results = new Queue<ILoginResult>();

        public void Handle(ILoginResult result)
        {
            Results.Enqueue(result);
        }
    }
}
