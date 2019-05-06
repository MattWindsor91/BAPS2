using System.Threading;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     Decodes BapsNet commands, pulls their non-command arguments from an
    ///     <see cref="ISource"/>, builds event structures,
    ///     and forwards them to a <see cref="Receiver"/>.
    /// </summary>
    public partial class CommandDecoder : ICommandVisitor
    {
        private readonly Receiver _receiver;
        private readonly ISource _source;
        private readonly CancellationToken _token;

        public CommandDecoder(Receiver r, ISource source, CancellationToken token)
        {
            _receiver = r;
            _source = source;
            _token = token;
        }

        private void Dispatch(ArgsBase message)
        {
            _receiver.OnMessageReceived(message);
        }
        
        #region Shortcuts for receiving from the sink

        private string ReceiveString()
        {
            return _source.ReceiveString(_token);
        }

        private float ReceiveFloat()
        {
            return _source.ReceiveFloat(_token);
        }

        private uint ReceiveUint()
        {
            return _source.ReceiveUint(_token);
        }

        #endregion Shortcuts for receiving from the sink

        #region Decoders for common BapsNet idioms

        private void ReportMalformedCommand(ICommand command, CommandGroup group)
        {
            Dispatch(new UnknownCommandArgs(command.Packed, $"possibly a malformed {group.ToString().ToUpper()}"));
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
        
        #endregion Decoders for common BapsNet idioms
    }
}