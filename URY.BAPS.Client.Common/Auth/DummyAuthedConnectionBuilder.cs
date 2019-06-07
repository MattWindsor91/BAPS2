using System;

using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;

namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     An <see cref="IAuthedConnectionBuilder{TConn}"/> that always returns a
    ///     pre-built <see cref="ILoginResult"/> when asked to attempt a connection, and always returns a pre-built
    ///     connection when asked for one.
    /// </summary>
    /// <typeparam name="TConn">Type of connections provided by this builder.</typeparam>
    public class DummyAuthedConnectionBuilder<TConn> : IAuthedConnectionBuilder<TConn> where TConn : class
    {
        private readonly ILoginResult _result;

        public DummyAuthedConnectionBuilder(TConn? connection, ILoginResult? result)
        {
            Connection = connection;
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public TConn? Connection { get; }

        public ILoginResult Attempt(ILoginPromptResponse promptResponse)
        {
            return _result;
        }
    }
}