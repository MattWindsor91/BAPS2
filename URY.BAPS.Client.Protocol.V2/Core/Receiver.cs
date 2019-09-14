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
        private readonly CommandDecoder _decoder;

        private readonly CancellationToken _token;

        /// <summary>
        ///     Constructs a <see cref="Receiver"/>.
        /// </summary>
        /// <param name="bapsNet">
        ///     A downstream source for BapsNet primitives.
        /// </param>
        /// <param name="decoder">
        ///     A <see cref="ClientCommandDecoder"/> used to decode command
        ///     words.  It should be using the same primitive source and
        ///     cancellation token as this receiver.
        /// </param>
        /// <param name="token">
        ///     The cancellation token used to cancel this receiver.
        ///     This is provided up-front as the decoder should also
        ///     be using the same token.
        /// </param>
        public Receiver(IPrimitiveSource? bapsNet, CommandDecoder decoder, CancellationToken token)
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
            while (true)
            {
                _token.ThrowIfCancellationRequested();
                var cmdReceived = _bapsNet.ReceiveCommand(_token);
                DecodeCommand(cmdReceived);
            }
        }

        private void DecodeCommand(ushort word)
        {
            _bapsNet.ReceiveUint(_token); /* ignore length */
            var cmd = CommandFactory.Unpack(word);
            cmd.Accept(_decoder);
        }
    }
}