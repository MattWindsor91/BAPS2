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
                    _receiver.OnUnknownCommand(command.Packed, "possibly a malformed SYSTEM");
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
            _receiver.OnDirectoryFileAdd(new DirectoryFileAddArgs(directoryIndex, index, description));
        }

        private void DecodeDirectoryPrepare(byte directoryIndex)
        {
            _ = ReceiveUint();
            var niceDirectoryName = ReceiveString();
            _receiver.OnDirectoryPrepare(new DirectoryPrepareEventArgs(directoryIndex, niceDirectoryName));
        }

        private void DecodeServerVersion()
        {
            var version = ReceiveString();
            var date = ReceiveString();
            var time = ReceiveString();
            var author = ReceiveString();
            _receiver.OnVersion(new ServerVersion(version, date, time, author));
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
            _receiver.OnTextSetting(new TextSettingEventArgs(setting, upDown));
        }

        private void DecodeQuit()
        {
            //The server should send an int representing if this is an expected quit (0) or an exception error (1)."
            var expected = ReceiveUint() == 0;
            _receiver.OnServerQuit(expected);
        }

        private static UpDown ValueToUpDown(byte value)
        {
            return value == 0 ? UpDown.Down : UpDown.Up;
        }
    }
}