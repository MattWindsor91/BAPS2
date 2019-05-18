using System;
using System.Reactive.Linq;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     Listens on an <see cref="IBapsNetSource" /> for incoming BapsNet commands, decodes them, and sends server update
    ///     events through an observer.
    /// </summary>
    public class Receiver : IMessageSink
    {
        /// <summary>
        ///     The BapsNet connection on which we're receiving commands.
        /// </summary>
        [NotNull] private readonly IBapsNetSource _bapsNet;

        /// <summary>
        ///     The decoder used to receive and process the bodies of BapsNet messages.
        /// </summary>
        private readonly CommandDecoder _decoder;

        private readonly CancellationToken _token;

        private IObservable<MessageArgsBase>? _observeMessages;

        public Receiver(IBapsNetSource? bapsNet, CancellationToken token)
        {
            _bapsNet = bapsNet ?? throw new ArgumentNullException(nameof(bapsNet));
            _token = token;
            // TODO(@MattWindsor91): inject this dependency.
            _decoder = new CommandDecoder(this, _bapsNet, _token);
        }

        /// <summary>
        ///     Observable used to receive decoded BapsNet messages.
        /// </summary>
        public IObservable<MessageArgsBase> ObserveMessage =>
            _observeMessages ??= Observable.FromEventPattern<MessageArgsBase>(
                ev => MessageReceived += ev,
                ev => MessageReceived -= ev
            ).Select(x => x.EventArgs);

        public virtual void OnMessageReceived(MessageArgsBase e)
        {
            MessageReceived?.Invoke(this, e);
        }

        private event EventHandler<MessageArgsBase> MessageReceived;

        public void Run()
        {
            while (!_token.IsCancellationRequested)
            {
                var cmdReceived = _bapsNet.ReceiveCommand();
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