using System.Threading;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Model;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    /// <summary>
    ///     Decodes BapsNet commands, pulls their non-command arguments from an
    ///     <see cref="IBapsNetSource"/>, builds event structures,
    ///     and forwards them to an <see cref="IMessageSink"/>.
    /// </summary>
    public partial class CommandDecoder : ICommandVisitor
    {
        private readonly IMessageSink _receiver;
        private readonly IBapsNetSource _bapsNetSource;
        private readonly CancellationToken _token;

        public CommandDecoder(IMessageSink r, IBapsNetSource bapsNetSource, CancellationToken token)
        {
            _receiver = r;
            _bapsNetSource = bapsNetSource;
            _token = token;
        }

        private void Dispatch(MessageArgsBase message)
        {
            _receiver.OnMessageReceived(message);
        }
        
        #region Shortcuts for receiving from the sink

        private string ReceiveString()
        {
            return _bapsNetSource.ReceiveString(_token);
        }

        private float ReceiveFloat()
        {
            return _bapsNetSource.ReceiveFloat(_token);
        }

        private uint ReceiveUint()
        {
            return _bapsNetSource.ReceiveUint(_token);
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
        
        private TrackType DecodeTrackType()
        {
            return (TrackType) ReceiveUint();
        }

        #endregion Decoders for common BapsNet idioms
    }
}