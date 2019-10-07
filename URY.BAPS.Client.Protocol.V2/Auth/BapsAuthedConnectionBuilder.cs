﻿using System;
using System.Diagnostics;
using System.Net.Sockets;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.LoginResult;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Client.Protocol.V2.Auth
{
    public class BapsAuthedConnectionBuilder : IAuthedConnectionBuilder<TcpConnection>, IDisposable
    {
        /// <summary>
        ///     Cached instance of <see cref="SuccessLoginResult" />.
        /// </summary>
        private readonly SuccessLoginResult _success = new SuccessLoginResult();

        private TcpConnection? _connection;

        private ILoginPromptResponse _lastResponse = new QuitLoginPromptResponse();

        private string? _seed;

        private bool ConnectionReady => _connection != null && _seed != null;

        public TcpConnection? Connection => ConnectionReady ? _connection : null;

        public ILoginResult Attempt(ILoginPromptResponse response)
        {
            var connectionResult = ConnectIfNeeded(response);
            if (!connectionResult.IsSuccess) return connectionResult;

            _lastResponse = response;

            var loginResult = TryLogin(response);
            if (!loginResult.IsSuccess) DisposeSocketIfExists();
            return loginResult;
        }

        private void DisposeSocketIfExists()
        {
            _connection?.Dispose();
            _connection = null;
        }

        [Pure]
        private bool NeedNewConnection(ILoginPromptResponse promptResponse)
        {
            return !ConnectionReady || HasServerChanged(promptResponse);
        }

        [Pure]
        private bool HasServerChanged(ILoginPromptResponse promptResponse)
        {
            return _lastResponse.IsDifferentServer(promptResponse);
        }

        private ILoginResult ConnectIfNeeded(ILoginPromptResponse promptResponse)
        {
            if (!NeedNewConnection(promptResponse)) return _success;

            DisposeSocketIfExists();
            return Connect(promptResponse);
        }

        private ILoginResult Connect(ILoginPromptResponse promptResponse)
        {
            TcpClient? clientSocket = null;

            try
            {
                clientSocket = new TcpClient(promptResponse.Host, promptResponse.Port) { LingerState = new LingerOption(false, 0), NoDelay = true };
            }
            catch (SocketException e)
            {
                clientSocket?.Dispose();
                return new SocketFailureLoginResult(e);
            }
            _connection = new TcpConnection(clientSocket);


            /** Receive the greeting string, this is the only communication
                that does not follow the 'command' 'command-length' 'argument1'...
                structure
             **/
            _ = _connection.ReceiveString();
            SetBinaryMode();

            var (wasSeed, _, seed) = _connection.ReceiveSystemStringCommand(SystemOp.Seed);
            if (!wasSeed) return new InvalidProtocolLoginResult("seed");
            _seed = seed;
            return _success;
        }

        private void SetBinaryMode()
        {
            if (_connection is null)
                throw new InvalidOperationException("Cannot set binary mode without a connection.");

            var binaryModeCommand = new SystemCommand(SystemOp.SetBinaryMode);
            var binaryModeMessage = new MessageBuilder(binaryModeCommand);
            binaryModeMessage.Send(_connection);
        }

        private ILoginResult TryLogin(ILoginPromptResponse promptResponse)
        {
            if (_connection is null)
                throw new InvalidOperationException("Cannot log in without a connection.");
            if (_seed is null)
                throw new InvalidOperationException("Cannot log in without a seed.");

            var lp = new LoginPerformer(_connection, _seed);
            return lp.TryLogin(promptResponse);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}