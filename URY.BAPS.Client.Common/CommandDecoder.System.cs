using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common
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
                    ReportMalformedCommand(command, CommandGroup.System);
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

        private void DecodeServerVersion()
        {
            var version = ReceiveString();
            var date = ReceiveString();
            var time = ReceiveString();
            var author = ReceiveString();
            var sv = new ServerVersion(version, date, time, author);
            Dispatch(new ServerVersionArgs(sv));
        }

        private void DecodeFeedback()
        {
            _ = ReceiveUint();
        }

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