using URY.BAPS.Client.Common.Events;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common
{
    public partial class CommandDecoder
    {
        public void Visit(DatabaseCommand command)
        {
            switch (command.Op)
            {
                case DatabaseOp.LibraryResult:
                {
                    if (command.ModeFlag)
                    {
                        DecodeLibraryResult(command.Value);
                    }
                    else
                    {
                        DecodeCount(CountType.LibraryItem);
                    }
                }
                    break;
                case DatabaseOp.LibraryError:
                {
                    DecodeError(ErrorType.Library, command.Value);
                }
                    break;
                case DatabaseOp.Show:
                    if (command.ModeFlag)
                    {
                        DecodeShow();
                    }
                    else
                    {
                        DecodeCount(CountType.Show);
                    }

                    break;
                case DatabaseOp.Listing:
                    if (command.ModeFlag)
                    {
                        DecodeListing();
                    }
                    else
                    {
                        DecodeCount(CountType.Listing);
                    }

                    break;
                case DatabaseOp.BapsDbError:
                    DecodeError(ErrorType.BapsDb, command.Value);
                    break;
                default:
                    ReportMalformedCommand(command, CommandGroup.Database);
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