using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    public partial class CommandDecoderBase
    {
        public void Visit(DatabaseCommand command)
        {
            switch (command.Op)
            {
                case DatabaseOp.LibraryResult when command.ModeFlag:
                    DecodeLibraryResult(command.Value);
                    break;
                case DatabaseOp.LibraryResult when !command.ModeFlag:
                    DecodeCount(CountType.LibraryItem);
                    break;
                case DatabaseOp.LibraryError:
                    DecodeError(ErrorType.Library, command.Value);
                    break;
                case DatabaseOp.Show when command.ModeFlag:
                    DecodeShow();
                    break;
                case DatabaseOp.Show when !command.ModeFlag: 
                    DecodeCount(CountType.Show);
                    break;
                case DatabaseOp.Listing when command.ModeFlag:
                    DecodeListing();
                    break;
                case DatabaseOp.Listing when !command.ModeFlag:
                    DecodeCount(CountType.Listing);
                    break;
                case DatabaseOp.BapsDbError:
                    DecodeError(ErrorType.BapsDb, command.Value);
                    break;
                default:
                    ReportMalformedCommand(CommandGroup.Database);
                    break;
            }
        }

        private void DecodeListing()
        {
            var listingId = ReceiveUint();
            var channelId = ReceiveUint();
            var description = ReceiveString();
            Dispatch(new ListingResultArgs(listingId, channelId, description));
        }

        private void DecodeShow()
        {
            var showId = ReceiveUint();
            var description = ReceiveString();
            Dispatch(new ShowResultArgs(showId, description));
        }

        private void DecodeLibraryResult(byte dirtyStatus)
        {
            var resultId = ReceiveUint();
            var description = ReceiveString();
            Dispatch(new LibraryResultArgs(resultId, dirtyStatus, description));
        }
    }
}