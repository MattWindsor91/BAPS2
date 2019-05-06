using System;
using System.Reactive.Linq;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     Listens on an <see cref="ISource" /> for incoming BapsNet commands, decodes them, and sends server update
    ///     events through an observer.
    /// </summary>
    public class Receiver
    {
        /// <summary>
        ///     The BapsNet connection on which we're receiving commands.
        /// </summary>
        [NotNull] private readonly ISource _bapsNet;
        
        private event EventHandler<ArgsBase> MessageReceived;

        private IObservable<ArgsBase>? _observeMessages;
        
        /// <summary>
        ///     Observable used to receive decoded BapsNet messages.
        /// </summary>
        public IObservable<ArgsBase> ObserveMessage =>
            _observeMessages ??= Observable.FromEventPattern<ArgsBase>(
                ev => MessageReceived += ev,
                ev => MessageReceived -= ev
            ).Select(x => x.EventArgs);
 
        private readonly CancellationToken _token;

        /// <summary>
        ///     The decoder used to receive and process the bodies of BapsNet messages.
        /// </summary>
        private readonly CommandDecoder _decoder;
        
        public Receiver(ISource? bapsNet, CancellationToken token)
        {
            _bapsNet = bapsNet ?? throw new ArgumentNullException(nameof(bapsNet));
            _token = token;
            // TODO(@MattWindsor91): inject this dependency.
            _decoder = new CommandDecoder(this, _bapsNet, _token);
        }
        
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

        public virtual void OnMessageReceived(ArgsBase e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}