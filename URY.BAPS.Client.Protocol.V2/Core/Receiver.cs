using System;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     Listens on an <see cref="IPrimitiveSource" /> for incoming BapsNet commands, decodes them, and sends server update
    ///     events through an observer.
    /// </summary>
    public class Receiver
    {
        /// <summary>
        ///     The BapsNet connection on which we're receiving commands.
        /// </summary>
        [NotNull] private readonly IPrimitiveSource _bapsNet;

        /// <summary>
        ///     The decoder used to receive and process the bodies of BapsNet messages.
        /// </summary>
        private readonly CommandDecoderBase _decoder;

        private readonly CancellationToken _token;

        public Receiver(IPrimitiveSource? bapsNet, ClientCommandDecoder decoder, CancellationToken token)
        {
            _bapsNet = bapsNet ?? throw new ArgumentNullException(nameof(bapsNet));
            _token = token;

            _decoder = decoder;
        }

        /// <summary>
        ///     Observable used to receive decoded BapsNet messages.
        /// </summary>
        public IObservable<MessageArgsBase> ObserveMessage =>
            _decoder.ObserveMessage;

        public void Run()
        {
            while (!_token.IsCancellationRequested)
            {
                var cmdReceived = _bapsNet.ReceiveCommand(_token);
                DecodeCommand(cmdReceived);
            }

            _token.ThrowIfCancellationRequested();
        }

        private void DecodeCommand(CommandWord word)
        {
            _ /* length */ = _bapsNet.ReceiveUint();
            var cmd = word.Unpack();
            cmd.Accept(_decoder);
        }
    }
}