using System.Threading;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;

namespace URY.BAPS.Client.Common
{
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

        private void ReportMalformedCommand(ICommand command, string typeName)
        {
            _receiver.OnUnknownCommand(command.Packed, $"possibly a malformed {typeName.ToUpper()}");
        }
        
        private void DecodeCount(CountType type, uint extra = 0)
        {
            var count = ReceiveUint();
            _receiver.OnIncomingCount(new CountEventArgs {Count = count, Type = type, Extra = extra});
        }

        private void DecodeError(ErrorType type, byte errorCode)
        {
            var description = ReceiveString();
            _receiver.OnError(new ErrorEventArgs {Type = type, Code = errorCode, Description = description});
        }
        
        #endregion Decoders for common BapsNet idioms
    }
}