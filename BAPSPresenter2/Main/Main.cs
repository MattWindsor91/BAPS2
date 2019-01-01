using BAPSCommon;
using BAPSPresenter; // Legacy
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class Main : Form
    {
        /** This flag is used to cleanly exit the send/receive loops
            in the case of the receive loop, the flag will not take effect
            until data is received, so an abort message is still required
        **/
        private bool dead = false;
        // Accessor for the crashed variable.
        public bool HasCrashed { get; private set; } = false;

        /** A handle for the connection to the server **/
        private BAPSCommon.ClientSocket clientSocket;

        /** The current user **/
        private string username;

        private BAPSChannel[] bapsChannels;
        private BAPSDirectory[] bapsDirectories;

        /** The sender thread **/
        private System.Threading.Thread senderThread;
        /** The receiver thread **/
        private System.Threading.Thread receiverThread;
 
        /** Whether or not the timers are enabled **/
        private bool timersEnabled = true;

        /** The outgoing message queue **/
        private System.Collections.Concurrent.BlockingCollection<BAPSCommon.Message> msgQueue;

        private bool ChannelOutOfBounds(ushort channel) => 3 <= channel;
        private bool DirectoryOutOfBounds(ushort directory) => 3 <= directory;

        public Main()
        {
            ConfigManager.initConfigManager();

            var login = new Dialogs.Login();
            /** This flag defines success of the login procedure
                (along with the implicit knowledge that the server is ready)
            **/
            var authenticated = false;
            /** If a server error occurs we set this flag so that the connection is
                attempted again
            **/
            var wasServerError = false;
            /** Each session with a server has a security string used to encrypt secrets **/
            string randomSecurityString = "";
            /** Keep looping until we authenticate, throw an exception or exit forcefully **/
            while (!authenticated)
            {
                /** If we cancel login we can assume we wish to abort as there is nothing
                    else to do
                **/
                if (login.ShowDialog() == DialogResult.Cancel)
                {
                    Process.GetCurrentProcess().Kill();
                }
                /** If either the server or port have been changed since last attempt
                    we need to reconnect.
                **/
                if (login.needsToConnect() || wasServerError)
                {
                    try
                    {
                        /** Destroy old connection (if present) **/
                        clientSocket?.Dispose();
                    }
                    catch (Exception)
                    {
                        /** Do nothing **/
                    }
                    try
                    {
                        /** Attempt to make a connection to the specified server **/
                        clientSocket = new BAPSCommon.ClientSocket(login.Server, login.Port);
                    }
                    catch (Exception e)
                    {
                        /** If an error occurs just give the exception message and start again **/
                        var errorMessage = string.Concat("System Error:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                        MessageBox.Show(errorMessage, "Server error:", MessageBoxButtons.OK);
                        logError(errorMessage);
                        wasServerError = true;
                        continue;
                    }
                    /** Receive the greeting string, this is the only communication
                        that does not follow the 'command' 'command-length' 'argument1'...
                        structure
                    **/
                    var greeting = clientSocket.ReceiveS();
                    /** Let the server know we are a binary client **/
                    clientSocket.Send(Command.SYSTEM | Command.SETBINARYMODE);
                    /** Specify the length of the command **/
                    clientSocket.Send(0U);
                    /** Receive what should be the SEED command **/
                    var seedCmd = clientSocket.ReceiveC();
                    /** Receive the length of the seed command **/
                    clientSocket.ReceiveI();
                    /** Verify the server is sending what we expect **/
                    if ((seedCmd & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.SEED))
                    {
                        MessageBox.Show("Server login procedure is not compatible with this client.", "Server error:", MessageBoxButtons.OK);
                        /** Notify a server error so a reconnect is attempted **/
                        wasServerError = true;
                        continue;
                    }
                    else
                    {
                        /** Receive the SEED **/
                        randomSecurityString = clientSocket.ReceiveS();
                    }
                    /** Clear any server error **/
                    wasServerError = false;
                }
                /** Encrypt the password **/
                var securedPassword = Md5sum(string.Concat(randomSecurityString, Md5sum(login.Password)));
                /** Send LOGIN command **/
                clientSocket.Send(Command.SYSTEM | Command.LOGIN | 0);
                /** Send correct command length **/
                clientSocket.Send((uint)(login.Username.Length + securedPassword.Length));
                /** Send username **/
                clientSocket.Send(login.Username);
                /** Send encrypted password **/
                clientSocket.Send(securedPassword);
                /** Receive what should be the login result **/
                var authResult = clientSocket.ReceiveC();
                /** Verify it is what we expected **/
                if ((authResult & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.LOGINRESULT))
                {
                    MessageBox.Show("Server login procedure is not compatible with this client.", "Server error:", MessageBoxButtons.OK);
                    /** This is a server error as the server is incompatible with this client **/
                    wasServerError = true;
                    continue;
                }
                else
                {
                    /** Receive the result command length **/
                    clientSocket.ReceiveI();
                    /** Receive the description of the result code **/
                    var description = clientSocket.ReceiveS();
                    /** Check the value for '0' meaning success **/
                    authenticated = (authResult & Command.SYSTEM_VALUEMASK) == 0;
                    if (!authenticated)
                    {
                        /** Tell the client of any failure **/
                        MessageBox.Show(description, "Login error:", MessageBoxButtons.OK);
                    }
                }
            }
            username = login.Username;
            /** Do the form initialization **/
            InitializeComponent();

            SetupChannels();
            SetupDirectories();

            countdownTimer = new Timer
            {
                Interval = 200
            };
            countdownTimer.Tick += countdownTick;
            countdownTimer.Start();

            textDialog = new Dialogs.Text("Write on me");
            textDialog.KeyDownForward += BAPSPresenterMain_KeyDown;

            ConfigCache.initConfigCache();
            /** Create a message queue for sending commands to the server **/
            msgQueue = new System.Collections.Concurrent.BlockingCollection<BAPSCommon.Message>(new System.Collections.Concurrent.ConcurrentQueue<BAPSCommon.Message>());
            /** Add the autoupdate message onto the queue (chat(2) and general(1)) **/
            /** Add the autoupdate message onto the queue (chat(2) and general(1)) **/
            Command cmd = Command.SYSTEM | Command.AUTOUPDATE | (Command)2 | (Command)1;
            msgQueue.Add(new BAPSCommon.Message(cmd));
            for (int i = 0; i < 3; i++)
            {
                /** Add the refresh folder onto the queue **/
                cmd = Command.SYSTEM | Command.LISTFILES | (Command)i;
                msgQueue.Add(new BAPSCommon.Message(cmd));
            }

            /** Enable or disable the timers depending on the config setting, enable on default when no registry config value set. **/
            var enableTimers = string.Compare(ConfigManager.getConfigValueString("EnableTimers", "Yes"), "Yes") == 0;
            EnableTimerControls(enableTimers);

            /** Start the receive thread so we are ready for the autoupdate messages **/
            receiverThread = new System.Threading.Thread(ReceiverFunc);
            receiverThread.Start();
            /** Start the sender thread last so that everything is ready for the autoupdate
                message to be sent and acted upon by the server
            **/
            senderThread = new System.Threading.Thread(SenderFunc);
            senderThread.Start();
        }

        private void SetupDirectories()
        {
            bapsDirectories = new BAPSDirectory[3];
            for (var i = 0; i < bapsDirectories.Length; i++)
            {
                bapsDirectories[i] = new BAPSDirectory()
                {
                    DirectoryID = i
                };
                bapsDirectories[i].RefreshRequest += RefreshDirectory;
            }
            directoryFlow.Controls.AddRange(bapsDirectories);
        }

        private void SetupChannels()
        {
            bapsChannels = new BAPSChannel[3] { bapsChannel1, bapsChannel2, bapsChannel3 };
            foreach (var bc in bapsChannels)
            {
                Debug.Assert(0 <= bc.ChannelID, "Channel ID hasn't been set---check the channels' properties in the designer");

                bc.TrackListRequestChange += TrackList_RequestChange;
                bc.OpRequest += ChannelOperation_Click;
                bc.PositionRequestChange += HandlePositionChanged;
                bc.TimelineChanged += TimelineChanged;
                //bc.TrackListContextMenuStripItemClicked += (e, x) => Invoke((ToolStripItemClickedEventHandler)trackListContextMenuStrip_ItemClicked, e, x);
            }
        }

        /** Loop to wait for a command and then process it correctly **/
        private void ReceiverFunc()
        {
            while (!dead)
            {
                DecodeCommand(clientSocket.ReceiveC());
            }
        }

        /** Generate an md5 sum of the raw argument **/
        private string Md5sum(string raw)
        {
            var md5serv = System.Security.Cryptography.MD5.Create();
            var stringbuff = new System.Text.StringBuilder();
            var buffer = System.Text.Encoding.ASCII.GetBytes(raw);
            var hash = md5serv.ComputeHash(buffer);

            foreach (var h in hash)
            {
                stringbuff.Append(h.ToString("x2"));
            }
            return stringbuff.ToString();
        }

        /** Enable or disable the timer controls **/
        private void EnableTimerControls(bool shouldEnable)
        {
            timersEnabled = shouldEnable;
            foreach (var chan in bapsChannels) chan.EnableTimerControls(shouldEnable);
            timeLine.DragEnabled = shouldEnable;
        }

        /** Notify AudioWall to Update **/
        private void RefreshAudioWall()
        {
            if (audioWall?.Visible ?? false)
            {
                audioWall.Invoke((Action)audioWall.refreshWall);
            }
        }

        /** Function to async send the notify of a Comms Error / allow a way to restart the client. **/
        private void SendQuit(string description, bool silent)
        {
            if (HasCrashed) return;

            _ = BeginInvoke((Action<string, bool>)Quit, description, silent);
        }

        /** Function to notify of a Comms Error **/
        private void Quit(string description, bool silent)
        {
            /** On Communications errors this is called to notify the user **/
            /** Only current option is to die **/
            dead = true;
            if (!silent)
            {
                MessageBox.Show(string.Concat(description, "\nClick OK to restart the Presenter Interface.\nPlease notify support that an error occurred."), "System error:", MessageBoxButtons.OK);
                logError(description);
            }
            HasCrashed = true;
            Close();
        }

        /** Function to open write and close a log file -- FOR EMERGENCIES ONLY **/
        private void logError(string errorMessage)
        {
            try
            {
                using (var stream = new System.IO.StreamWriter("bapserror.log", true))
                {
                    stream.Write(errorMessage);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(string.Concat("Unable to write log file, Please write down the following information:\n", errorMessage), "Log file error", MessageBoxButtons.OK);
            }
        }

        /** Helper function to do command decoding **/
        private void DecodeCommand(Command cmdReceived)
        {
            _ /* length */ = clientSocket.ReceiveI();
            switch (cmdReceived & Command.GROUPMASK)
            {
                case Command.PLAYBACK:
                    var op = cmdReceived & Command.PLAYBACK_OPMASK;
                    switch (op)
                    {
                        case Command.PLAY:
                        case Command.PAUSE:
                        case Command.STOP:
                            {
                                var channel = cmdReceived & Command.PLAYBACK_CHANNELMASK;
                                Invoke((Action<ushort, Command>)showChannelOperation, channel, op);
                            }
                            break;
                        case Command.POSITION:
                            {
                                var channel = cmdReceived & Command.PLAYBACK_CHANNELMASK;
                                var position = clientSocket.ReceiveI();
                                Invoke((Action<ushort, uint>)showPosition, channel, position);
                            }
                            break;
                        case Command.VOLUME:
                            {
                                _ = clientSocket.ReceiveF();
                            }
                            break;
                        case Command.LOAD:
                            {
                                var channel = cmdReceived & Command.PLAYLIST_CHANNELMASK;
                                var index = clientSocket.ReceiveI();
                                var type = (Command)clientSocket.ReceiveI();
                                var description = clientSocket.ReceiveS();
                                switch (type)
                                {
                                    case Command.FILEITEM:
                                    case Command.LIBRARYITEM:
                                        {
                                            var duration = clientSocket.ReceiveI();
                                            Invoke((Action<ushort, uint>)showDuration, channel, duration);
                                            Invoke((Action<ushort, uint>)showPosition, channel, 0U);
                                        }
                                        goto case Command.VOIDITEM;
                                    case Command.VOIDITEM:
                                        {
                                            Invoke((Action<ushort, uint, Command, string>)showLoadedItem, channel, index, type, description);
                                        }
                                        break;
                                    case Command.TEXTITEM:
                                        {
                                            var text = clientSocket.ReceiveS();
                                            Invoke((Action<ushort, uint, string, string>)showText, channel, index, description, text);
                                        }
                                        break;
                                    default:
                                        Invoke((Action<ushort, uint>)showDuration, channel, 0U);
                                        Invoke((Action<ushort, uint>)showPosition, channel, 0U);
                                        break;
                                }
                            }
                            break;
                        case Command.CUEPOSITION:
                            {
                                var channel = cmdReceived.Channel();
                                var cuePosition = clientSocket.ReceiveI();
                                Invoke((Action<ushort, uint>)showCuePosition, channel, cuePosition);
                            }
                            break;
                        case Command.INTROPOSITION:
                            {
                                var channel = cmdReceived.Channel();
                                var introPosition = clientSocket.ReceiveI();
                                Invoke((Action<ushort, uint>)showIntroPosition, channel, introPosition);
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                SendQuit("Received unknown command, possibly a malformed PLAYBACK.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.PLAYLIST:
                    switch (cmdReceived & Command.PLAYLIST_OPMASK)
                    {
                        case Command.ITEM:
                            if (cmdReceived.HasFlag(Command.PLAYLIST_MODEMASK))
                            {
                                var channel = cmdReceived.Channel();
                                var index = clientSocket.ReceiveI();
                                var type = clientSocket.ReceiveI();
                                var description = clientSocket.ReceiveS();
                                Invoke((Action<ushort, uint, uint, string>)addItem, channel, index, type, description);
                            }
                            else
                            {
                                _ = clientSocket.ReceiveI();
                            }
                            break;
                        case Command.MOVEITEMTO:
                            {
                                var channel = cmdReceived.Channel();
                                var indexFrom = clientSocket.ReceiveI();
                                var indexTo = clientSocket.ReceiveI();
                                Invoke((Action<ushort, uint, uint>)moveItemTo, channel, indexFrom, indexTo);
                            }
                            break;
                        case Command.DELETEITEM:
                            {
                                var channel = cmdReceived.Channel();
                                var index = clientSocket.ReceiveI();
                                Invoke((Action<ushort, uint>)deleteItem, channel, index);
                            }
                            break;
                        case Command.RESETPLAYLIST:
                            {
                                var channel = cmdReceived.Channel();
                                Invoke((Action<ushort>)cleanPlaylist, channel);
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                SendQuit("Received unknown command, possibly a malformed PLAYLIST.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.DATABASE:
                    switch (cmdReceived & Command.DATABASE_OPMASK)
                    {
                        case Command.LIBRARYRESULT:
                            {
                                if (cmdReceived.HasFlag(Command.DATABASE_MODEMASK))
                                {
                                    var dirtyStatus = (ushort)(cmdReceived & Command.DATABASE_VALUEMASK);
                                    var resultid = clientSocket.ReceiveI();
                                    var description = clientSocket.ReceiveS();
                                    addLibraryResult(resultid, dirtyStatus, description);
                                }
                                else
                                {
                                    var count = clientSocket.ReceiveI();
                                    setLibraryResultCount((int)count);
                                }
                            }
                            break;
                        case Command.LIBRARYERROR:
                            {
                                var description = clientSocket.ReceiveS();
                                notifyLibraryError((ushort) (cmdReceived & Command.DATABASE_VALUEMASK), description);
                            }
                            break;
                        case Command.SHOW:
                            if (cmdReceived.HasFlag(Command.DATABASE_MODEMASK))
                            {
                                var showid = clientSocket.ReceiveI();
                                var description = clientSocket.ReceiveS();
                                addShowResult(showid, description);
                            }
                            else
                            {
                                var count = clientSocket.ReceiveI();
                                setShowResultCount((int)count);
                            }
                            break;
                        case Command.LISTING:
                            if (cmdReceived.HasFlag(Command.DATABASE_MODEMASK))
                            {
                                var listingid = clientSocket.ReceiveI();
                                var channel = clientSocket.ReceiveI();
                                var description = clientSocket.ReceiveS();
                                addListingResult(listingid, channel, description);
                            }
                            else
                            {
                                var count = clientSocket.ReceiveI();
                                setListingResultCount((int) count);
                            }
                            break;
                        case Command.BAPSDBERROR:
                            /** There is an error code in the command **/
                            notifyLoadShowError((ushort) (cmdReceived & Command.DATABASE_VALUEMASK), clientSocket.ReceiveS());
                            break;
                        default:
                            {
                                /** ERROR **/
                                SendQuit("Received unknown command, possibly a malformed DATABASE.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.CONFIG:
                    switch (cmdReceived & Command.CONFIG_OPMASK)
                    {
                        case Command.OPTION:
                            {
                                if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                                {
                                    var optionid = clientSocket.ReceiveI();
                                    var description = clientSocket.ReceiveS();
                                    var type = clientSocket.ReceiveI();
                                    processOption(cmdReceived, optionid, description, type);
                                }
                                else
                                {
                                    var count = clientSocket.ReceiveI();
                                    processOptionCount(count);
                                }
                            }
                            break;
                        case Command.OPTIONCHOICE:
                            {
                                if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                                {
                                    var optionid = clientSocket.ReceiveI();
                                    var choiceIndex = clientSocket.ReceiveI();
                                    var choiceDescription = clientSocket.ReceiveS();
                                    processChoice(optionid, choiceIndex, choiceDescription);
                                }
                                else
                                {
                                    var optionid = clientSocket.ReceiveI();
                                    var count = clientSocket.ReceiveI();
                                    processChoiceCount(optionid, (int)count);
                                }
                            }
                            break;
                        case Command.CONFIGSETTING:
                            {
                                if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                                {
                                    var optionid = clientSocket.ReceiveI();
                                    var type = clientSocket.ReceiveI();
                                    processConfigSetting(cmdReceived, optionid, (ConfigType)type);
                                }
                                else
                                {
                                    _ = clientSocket.ReceiveI();
                                }
                            }
                            break;
                        case Command.CONFIGRESULT:
                            {
                                var optionid = clientSocket.ReceiveI();
                                var result = clientSocket.ReceiveI();
                                processConfigResult(cmdReceived, optionid, (ConfigResult)result);
                            }
                            break;
                        case Command.CONFIGERROR:
                            {
                                var errorCode = cmdReceived & Command.CONFIG_VALUEMASK;
                                var description = clientSocket.ReceiveS();
                                processConfigError((uint)errorCode, description);
                            }
                            break;
                        case Command.USER:
                            {
                                if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                                {
                                    var username = clientSocket.ReceiveS();
                                    var permissions = clientSocket.ReceiveI();
                                    processUserInfo(username, permissions);
                                }
                                else
                                {
                                    var count = clientSocket.ReceiveI();
                                    processUserCount(count);
                                }
                            }
                            break;
                        case Command.PERMISSION:
                            {
                                if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                                {
                                    var permissionCode = clientSocket.ReceiveI();
                                    var description = clientSocket.ReceiveS();
                                    processPermissionInfo(permissionCode, description);
                                }
                                else
                                {
                                    var count = clientSocket.ReceiveI();
                                    processPermissionCount(count);
                                }
                            }
                            break;
                        case Command.USERRESULT:
                            {
                                var resultCode = cmdReceived & Command.CONFIG_VALUEMASK;
                                var description = clientSocket.ReceiveS();
                                processUserResult((uint)resultCode, description);
                            }
                            break;
                        case Command.IPRESTRICTION:
                            {
                                if (cmdReceived.HasFlag(Command.CONFIG_MODEMASK))
                                {
                                    var ipaddress = clientSocket.ReceiveS();
                                    var mask = clientSocket.ReceiveI();
                                    processIPRestriction(cmdReceived, ipaddress, mask);
                                }
                                else
                                {
                                    var count = clientSocket.ReceiveI();
                                    processIPRestrictionCount(cmdReceived, count);
                                }
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                SendQuit("Received unknown command, possibly a malformed CONFIG.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.SYSTEM:
                    switch (cmdReceived & Command.SYSTEM_OPMASK)
                    {
                        case Command.SENDLOGMESSAGE:
                            clientSocket.ReceiveS();
                            break;
                        case Command.FILENAME:
                            if (cmdReceived.HasFlag(Command.SYSTEM_MODEMASK))
                            {
                                var directoryIndex = cmdReceived & Command.SYSTEM_VALUEMASK;
                                var index = clientSocket.ReceiveI();
                                var description = clientSocket.ReceiveS();
                                Invoke((Action<ushort, uint, string>)addFileToDirectoryList, directoryIndex, index, description);
                            }
                            else
                            {
                                var directoryIndex = cmdReceived & Command.SYSTEM_VALUEMASK;
                                _ = clientSocket.ReceiveI();
                                var niceDirectoryName = clientSocket.ReceiveS();
                                Invoke((Action<ushort, string>)clearFiles, directoryIndex, niceDirectoryName);
                            }
                            break;
                        case Command.VERSION:
                            {
                                var version = clientSocket.ReceiveS();
                                var date = clientSocket.ReceiveS();
                                var time = clientSocket.ReceiveS();
                                var author = clientSocket.ReceiveS();
                                displayVersion(version, date, time, author);
                            }
                            break;
                        case Command.FEEDBACK:
                            {
                                _ = clientSocket.ReceiveI();
                            }
                            break;
                        case Command.SENDMESSAGE:
                            {
                                _ = clientSocket.ReceiveS();
                                _ = clientSocket.ReceiveS();
                                _ = clientSocket.ReceiveS();
                            }
                            break;
                        case Command.CLIENTCHANGE:
                            {
                                _ = clientSocket.ReceiveS();
                            }
                            break;
                        case Command.SCROLLTEXT:
                            {
                                var updown = cmdReceived & Command.SYSTEM_VALUEMASK;
                                Invoke((Action<int>)textDialog.scroll, (int)updown);
                            }
                            break;
                        case Command.TEXTSIZE:
                            {
                                var updown = cmdReceived & Command.SYSTEM_VALUEMASK;
                                Invoke((Action<int>)textDialog.textSize, (int)updown);
                            }
                            break;
                        case Command.QUIT:
                            {
                                //The server should send an int representing if this is an expected quit (0) or an exception error (1)."
                                bool expected = clientSocket.ReceiveI() == 0;
                                SendQuit("The Server is shutting down/restarting.\n", expected);
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                SendQuit("Received unknown command, possibly a malformed SYSTEM.\n", false);
                            }
                            break;
                    }
                    break;
                default:
                    {
                        /** ERROR **/
                        SendQuit("Received unknown command.\n", false);
                    }
                    break;
            }

        }

        /** Loop to watch for an outgoing message on the queue and send it **/
        private void SenderFunc()
        {
            /** Drop out if something goes horribly wrong elsewhere **/
            while (!dead)
            {
                var msg = msgQueue.Take();
                msg.Send(clientSocket);
            }
        }

        public void OpenAudioWall(TrackList tl)
        {
            if (audioWall == null || !audioWall.Visible)
            {
                audioWall = new AudioWall(msgQueue, tl);
                audioWall.KeyDownForward += BAPSPresenterMain_KeyDown;
                audioWall.Show();
            }
            else
            {
                audioWall.setChannel(tl);
                RefreshAudioWall();
            }
        }
    }
}
