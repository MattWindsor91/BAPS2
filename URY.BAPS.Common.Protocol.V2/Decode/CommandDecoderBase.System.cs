using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    public partial class CommandDecoder
    {
        public void Visit(SystemCommand command)
        {
            switch (command.Op)
            {
                case SystemOp.SendLogMessage:
                    DecodeSendLogMessage();
                    break;
                case SystemOp.Filename when command.ModeFlag:
                    DecodeDirectoryFileAdd(command.Value);
                    break;
                case SystemOp.Filename when !command.ModeFlag:
                    DecodeDirectoryPrepare(command.Value);
                    break;
                case SystemOp.ServerVersion:
                    DecodeServerVersion();
                    break;
                case SystemOp.Feedback:
                    DecodeFeedback();
                    break;
                case SystemOp.SendMessage:
                    DecodeSendMessage();
                    break;
                case SystemOp.ClientChange:
                    DecodeClientChange();
                    break;
                case SystemOp.ScrollText:
                    DecodeTextSetting(command.Value, TextSetting.Scroll);
                    break;
                case SystemOp.TextSize:
                    DecodeTextSetting(command.Value, TextSetting.FontSize);
                    break;
                case SystemOp.Quit:
                    DecodeQuit();
                    break;
                default:
                    ReportMalformedCommand(CommandGroup.System);
                    break;
            }
        }

        private void DecodeSendLogMessage()
        {
            _ = ReceiveString();
        }

        private void DecodeDirectoryFileAdd(byte directoryIndex)
        {
            var index = ReceiveUint();
            var description = ReceiveString();
            Dispatch(new DirectoryFileAddArgs(directoryIndex, index, description));
        }

        private void DecodeDirectoryPrepare(byte directoryIndex)
        {
            _ = ReceiveUint();
            var niceDirectoryName = ReceiveString();
            Dispatch(new DirectoryPrepareEventArgs(directoryIndex, niceDirectoryName));
        }

        protected abstract void DecodeServerVersion();

        protected abstract void DecodeFeedback();

        private void DecodeSendMessage()
        {
            _ = ReceiveString();
            _ = ReceiveString();
            _ = ReceiveString();
        }

        private void DecodeClientChange()
        {
            _ = ReceiveString();
        }

        private void DecodeTextSetting(byte value, TextSetting setting)
        {
            var upDown = ValueToUpDown(value);
            Dispatch(new TextSettingArgs(setting, upDown));
        }

        private void DecodeQuit()
        {
            //The server should send an int representing if this is an expected quit (0) or an exception error (1)."
            var expected = ReceiveUint() == 0;
            Dispatch(new ServerQuitArgs(expected));
        }

        private static TextSettingDirection ValueToUpDown(byte value)
        {
            return value == 0 ? TextSettingDirection.Down : TextSettingDirection.Up;
        }
    }
}