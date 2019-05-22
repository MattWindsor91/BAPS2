using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Model;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    /// <summary>
    ///     Decodes BapsNet commands, pulls their non-command arguments from an
    ///     <see cref="IPrimitiveSource" />, builds event structures,
    ///     and forwards them to an <see cref="IMessageSink" />.
    ///     <para>
    ///         This class is abstract because parts of it act differently
    ///         depending on whether the decoder is in a client or server
    ///         position.
    ///     </para>
    ///  </summary>
    ///  <seealso cref="ClientCommandDecoder"/>
    ///  <seealso cref="ServerCommandDecoder"/>
    public abstract partial class CommandDecoderBase : ICommandVisitor
    {
        /// <summary>
        ///     An event that fires every time the <see cref="CommandDecoderBase"/>
        ///     decodes a new message.
        /// </summary>
        public event EventHandler<MessageArgsBase> OnMessage;

        private IObservable<MessageArgsBase> _observeMessage;
        
        /// <summary>
        ///     An observable wrapping <see cref="OnMessage"/>.
        /// </summary>
        public IObservable<MessageArgsBase> ObserveMessage => _observeMessage ??= Observable.FromEventPattern<MessageArgsBase>(ev => OnMessage += ev, ev => OnMessage -= ev).Select(x => x.EventArgs);
        
        private readonly IPrimitiveSource _primitiveSource;
        private readonly CancellationToken _token;
        
        protected CommandDecoderBase(IPrimitiveSource primitiveSource, CancellationToken token)
        {
            _primitiveSource = primitiveSource;
            _token = token;
        }

        protected void Dispatch(MessageArgsBase message)
        {
            OnMessage?.Invoke(this, message);
        }

        #region Shortcuts for receiving from the sink

        protected string ReceiveString()
        {
            return _primitiveSource.ReceiveString(_token);
        }

        private float ReceiveFloat()
        {
            return _primitiveSource.ReceiveFloat(_token);
        }

        protected uint ReceiveUint()
        {
            return _primitiveSource.ReceiveUint(_token);
        }

        #endregion Shortcuts for receiving from the sink

        #region Decoders for common BapsNet idioms

        private void ReportMalformedCommand(CommandGroup group)
        {
            Dispatch(new UnknownCommandArgs($"possibly a malformed {group.ToString().ToUpper()}"));
        }

        private void DecodeCount(CountType type, uint extra = 0)
        {
            var count = ReceiveUint();
            Dispatch(new CountArgs(type, count, extra));
        }

        private void DecodeError(ErrorType type, byte errorCode)
        {
            var description = ReceiveString();
            Dispatch(new ErrorEventArgs(type, errorCode, description));
        }

        protected TrackType DecodeTrackType()
        {
            return (TrackType) ReceiveUint();
        }

        #endregion Decoders for common BapsNet idioms
    }
}