using System;

using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;

namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     An <see cref="IAuthPerformerAuthPerformer{TRawConn,TAuthConn}"/> that always returns a
    ///     pre-built <see cref="ILoginResult"/> when asked to attempt a connection, and always returns a pre-built
    ///     connection when asked for one.
    /// </summary>
    /// <typeparam name="TAuthConn">Type of raw connections consumed by this builder.</typeparam>
    /// <typeparam name="TAuthConn">Type of authenticated connections provided by this builder.</typeparam>
    public class DummyAuthPerformer<TRawConn, TAuthConn> : IAuthPerformer<TRawConn,TAuthConn>
    {
        public ILoginResult Result { get; }

        public DummyAuthPerformer(TAuthConn connection, ILoginResult? result)
        {
            Connection = connection;
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public TAuthConn Connection { get; }

        public void Attempt(TRawConn rawConn, IAuthPromptResponse promptResponse)
        {
            (rawConn as IDisposable)?.Dispose();
        }
    }
}