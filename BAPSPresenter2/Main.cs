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
        public bool hasCrashed { get; private set; } = false;

        /** A handle for the connection to the server **/
        private ClientSocket clientSocket;

        /** The current user **/
        private string username;

        private BAPSChannel[] bapsChannels;

        /** The sender thread **/
        private System.Threading.Thread senderThread;
        /** The receiver thread **/
        private System.Threading.Thread receiverThread;
 
        /** Whether or not the timers are enabled **/
        private bool timersEnabled = true;

        /** The outgoing message queue (Should only have ActionMessage objects)**/
        private System.Collections.Queue msgQueue;

        private bool ChannelOutOfBounds(ushort channel) => 3 <= channel;
        private bool DirectoryOutOfBounds(ushort directory) => 3 <= directory;

        public Main()
        {
            ConfigManager.initConfigManager();

            LoginDialog login = new LoginDialog();
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
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
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
                        clientSocket = new ClientSocket(login.Server, login.Port);
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
                    var greeting = clientSocket.receiveS();
                    /** Let the server know we are a binary client **/
                    clientSocket.send((ushort)(Command.SYSTEM | Command.SETBINARYMODE));
                    /** Specify the length of the command **/
                    clientSocket.send((uint)0);
                    /** Receive what should be the SEED command **/
                    Command seedCmd = (Command)clientSocket.receiveC();
                    /** Receive the length of the seed command **/
                    clientSocket.receiveI();
                    /** Verify the server is sending what we expect **/
                    if ((seedCmd & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.SEED))
                    {
                        MessageBox.Show("Server login procedure is not compatible with this client.", "Server error:", System.Windows.Forms.MessageBoxButtons.OK);
                        /** Notify a server error so a reconnect is attempted **/
                        wasServerError = true;
                        continue;
                    }
                    else
                    {
                        /** Receive the SEED **/
                        randomSecurityString = clientSocket.receiveS();
                    }
                    /** Clear any server error **/
                    wasServerError = false;
                }
                /** Encrypt the password **/
                var securedPassword = md5sum(string.Concat(randomSecurityString, md5sum(login.Password)));
                /** Send LOGIN command **/
                clientSocket.send((ushort)(Command.SYSTEM | Command.LOGIN | 0));
                /** Send correct command length **/
                clientSocket.send((uint)(login.Username.Length + securedPassword.Length));
                /** Send username **/
                clientSocket.send(login.Username);
                /** Send encrypted password **/
                clientSocket.send(securedPassword);
                /** Receive what should be the login result **/
                var authResult = (Command)clientSocket.receiveC();
                /** Verify it is what we expected **/
                if ((authResult & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.LOGINRESULT))
                {
                    MessageBox.Show("Server login procedure is not compatible with this client.", "Server error:", System.Windows.Forms.MessageBoxButtons.OK);
                    /** This is a server error as the server is incompatible with this client **/
                    wasServerError = true;
                    continue;
                }
                else
                {
                    /** Receive the result command length **/
                    clientSocket.receiveI();
                    /** Receive the description of the result code **/
                    var description = clientSocket.receiveS();
                    /** Check the value for '0' meaning success **/
                    authenticated = ((authResult & Command.SYSTEM_VALUEMASK) == 0);
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

            /** Tag codes for the controls so they can be identified at runtime **/
            object number0 = 0;
            object number1 = 1;
            object number2 = 2;

            /** Array initialisation so that controls can be found by channel
                number at runtime
            **/
            bapsChannels = new BAPSChannel[3] { bapsChannel1, bapsChannel2, bapsChannel3 };
            for (ushort i = 0; i < bapsChannels.Length; i++)
            {
                var bc = bapsChannels[i];
                // Channels get their IDs from their tags in the designer.
                Debug.Assert(bc.ChannelID == i, "Mismatch between channel IDs and array positions");
                // This is needed to make sure the lambdas below capture copies of the channel;
                // otherwise, they'll all get the value of 'i' at the end of the loop.
                ushort c = i;
                bc.TrackListRequestChange += (e, x) => Invoke((Action<ushort, RequestChangeEventArgs>)TrackList_RequestChange, c, x);
                bc.PlayRequested += (e, x) => Invoke((Action<ChannelOperationLookup>)ChannelOperation_Click, new ChannelOperationLookup(c, (ushort)Command.PLAY));
                bc.PauseRequested += (e, x) => Invoke((Action<ChannelOperationLookup>)ChannelOperation_Click, new ChannelOperationLookup(c, (ushort)Command.PAUSE));
                bc.StopRequested += (e, x) => Invoke((Action<ChannelOperationLookup>)ChannelOperation_Click, new ChannelOperationLookup(c, (ushort)Command.STOP));
                bc.PositionChanged += (e, pos) => Invoke((Action<ushort, int>)positionChanged, c, pos);
                bc.CuePositionChanged += (e, pos) => Invoke((Action<ushort, int>)cuePositionChanged, c, pos);
                bc.IntroPositionChanged += (e, pos) => Invoke((Action<ushort, int>)introPositionChanged, c, pos);
                bc.TimelineStartChanged += (e, pos) => Invoke((Action<int, int>)timeLine.UpdateStartTime, (int)c, pos);
                bc.TimelineDurationChanged += (e, pos) => Invoke((Action<int, int>)timeLine.UpdateDuration, (int)c, pos);
                bc.TimelinePositionChanged += (e, pos) => Invoke((Action<int, int>)timeLine.UpdatePosition, (int)c, pos);
                bc.TrackBarMoved += (e, x) => Invoke((Action<ushort, uint>)TrackBar_Scroll, c, x);
                //bc.TrackListContextMenuStripItemClicked += (e, x) => Invoke((ToolStripItemClickedEventHandler)trackListContextMenuStrip_ItemClicked, e, x);
            }

            directoryList = new ListBox[3] { Directory0, Directory1, Directory2 };
            directoryList[0].Tag = number0;
            directoryList[1].Tag = number1;
            directoryList[2].Tag = number2;

            directoryRefresh = new Button[3] { Directory0Refresh, Directory1Refresh, Directory2Refresh };
            directoryRefresh[0].Tag = number0;
            directoryRefresh[1].Tag = number1;
            directoryRefresh[2].Tag = number2;

            countdownTimer = new Timer
            {
                Interval = 200
            };
            countdownTimer.Tick += countdownTick;
            countdownTimer.Start();

            textDialog = new TextDialog(this, "Write on me");

            ConfigCache.initConfigCache();
            /** Create a message queue for sending commands to the server **/
            msgQueue = new System.Collections.Queue();
            /** It needs to be synchronized so that enqueue and dequeue are atomic **/
            msgQueue = System.Collections.Queue.Synchronized(msgQueue);
            /** Add the autoupdate message onto the queue (chat(2) and general(1)) **/
            /** Add the autoupdate message onto the queue (chat(2) and general(1)) **/
            Command cmd = Command.SYSTEM | Command.AUTOUPDATE | (Command)2 | (Command)1;
            msgQueue.Enqueue(new ActionMessage((ushort)cmd));
            for (int i = 0; i < 3; i++)
            {
                /** Add the refresh folder onto the queue **/
                cmd = Command.SYSTEM | Command.LISTFILES | (Command)i;
                msgQueue.Enqueue(new ActionMessage((ushort)cmd));
            }

            /** Enable or disable the timers depending on the config setting, enable on default when no registry config value set. **/
            var enableTimers = (string.Compare(ConfigManager.getConfigValueString("EnableTimers", "Yes"), "Yes") == 0);
            enableTimerControls(enableTimers);

            /** Start the receive thread so we are ready for the autoupdate messages **/
            receiverThread = new System.Threading.Thread(ReceiverFunc);
            receiverThread.Start();
            /** Start the sender thread last so that everything is ready for the autoupdate
                message to be sent and acted upon by the server
            **/
            senderThread = new System.Threading.Thread(SenderFunc);
            senderThread.Start();
        }

        /** Loop to wait for a command and then process it correctly **/
        private void ReceiverFunc()
        {
            while (!dead)
            {
                DecodeCommand((Command)clientSocket.receiveC());
            }
        }

        /** Generate an md5 sum of the raw argument **/
        private string md5sum(string raw)
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
        private void enableTimerControls(bool shouldEnable)
        {
            timersEnabled = shouldEnable;
            foreach (var chan in bapsChannels) chan.EnableTimerControls(shouldEnable);
            timeLine.DragEnabled = shouldEnable;
        }

        /** Notify AudioWall to Update **/
        private void refreshAudioWall()
        {
            if (audioWall?.Visible ?? false)
            {
                audioWall.Invoke((Action)audioWall.refreshWall);
            }
        }

        /** Function to async send the notify of a Comms Error / allow a way to restart the client. **/
        private void sendQuit(string description, bool silent)
        {
            if (hasCrashed) return;

            _ = BeginInvoke((Action<string, bool>)quit, description, silent);
        }

        /** Function to notify of a Comms Error **/
        private void quit(string description, bool silent)
        {
            /** On Communications errors this is called to notify the user **/
            /** Only current option is to die **/
            dead = true;
            if (!silent)
            {
                MessageBox.Show(string.Concat(description, "\nClick OK to restart the Presenter Interface.\nPlease notify support that an error occurred."), "System error:", MessageBoxButtons.OK);
                logError(description);
            }
            hasCrashed = true;
            Close();
        }

        /** Function to open write and close a log file -- FOR EMERGENCIES ONLY **/
        private void logError(string errorMessage)
        {
            System.IO.StreamWriter stream = null;
            try
            {
                stream = new System.IO.StreamWriter("bapserror.log", true);
                stream.Write(errorMessage);
            }
            catch (Exception)
            {
                MessageBox.Show(string.Concat("Unable to write log file, Please write down the following information:\n", errorMessage), "Log file error", MessageBoxButtons.OK);
            }
            finally
            {
                stream?.Close();
            }
        }

        /** Helper function to do command decoding **/
        private void DecodeCommand(Command cmdReceived)
        {
            _ /* length */ = clientSocket.receiveI();
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
                                var position = clientSocket.receiveI();
                                Invoke((Action<ushort, uint>)showPosition, channel, position);
                            }
                            break;
                        case Command.VOLUME:
                            {
                                _ = clientSocket.receiveF();
                            }
                            break;
                        case Command.LOAD:
                            {
                                var channel = cmdReceived & Command.PLAYLIST_CHANNELMASK;
                                var index = clientSocket.receiveI();
                                var type = (Command)clientSocket.receiveI();
                                var description = clientSocket.receiveS();
                                switch (type)
                                {
                                    case Command.FILEITEM:
                                    case Command.LIBRARYITEM:
                                        {
                                            var duration = clientSocket.receiveI();
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
                                            var text = clientSocket.receiveS();
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
                                var cuePosition = clientSocket.receiveI();
                                Invoke((Action<ushort, uint>)showCuePosition, channel, cuePosition);
                            }
                            break;
                        case Command.INTROPOSITION:
                            {
                                var channel = cmdReceived.Channel();
                                var introPosition = clientSocket.receiveI();
                                Invoke((Action<ushort, uint>)showIntroPosition, channel, introPosition);
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                sendQuit("Received unknown command, possibly a malformed PLAYBACK.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.PLAYLIST:
                    switch (cmdReceived & Command.PLAYLIST_OPMASK)
                    {
                        case Command.ITEM:
                            if (cmdReceived.IsFlagSet(Command.PLAYLIST_MODEMASK))
                            {
                                var channel = cmdReceived.Channel();
                                var index = clientSocket.receiveI();
                                var type = clientSocket.receiveI();
                                var description = clientSocket.receiveS();
                                Invoke((Action<ushort, uint, uint, string>)addItem, channel, index, type, description);
                            }
                            else
                            {
                                _ = clientSocket.receiveI();
                            }
                            break;
                        case Command.MOVEITEMTO:
                            {
                                var channel = cmdReceived.Channel();
                                var indexFrom = clientSocket.receiveI();
                                var indexTo = clientSocket.receiveI();
                                Invoke((Action<ushort, uint, uint>)moveItemTo, channel, indexFrom, indexTo);
                            }
                            break;
                        case Command.DELETEITEM:
                            {
                                var channel = cmdReceived.Channel();
                                var index = clientSocket.receiveI();
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
                                sendQuit("Received unknown command, possibly a malformed PLAYLIST.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.DATABASE:
                    switch (cmdReceived & Command.DATABASE_OPMASK)
                    {
                        case Command.LIBRARYRESULT:
                            {
                                if (cmdReceived.IsFlagSet(Command.DATABASE_MODEMASK))
                                {
                                    var dirtyStatus = (ushort)(cmdReceived & Command.DATABASE_VALUEMASK);
                                    var resultid = clientSocket.receiveI();
                                    var description = clientSocket.receiveS();
                                    addLibraryResult(resultid, dirtyStatus, description);
                                }
                                else
                                {
                                    var count = clientSocket.receiveI();
                                    setLibraryResultCount((int)count);
                                }
                            }
                            break;
                        case Command.LIBRARYERROR:
                            {
                                var description = clientSocket.receiveS();
                                notifyLibraryError((ushort) (cmdReceived & Command.DATABASE_VALUEMASK), description);
                            }
                            break;
                        case Command.SHOW:
                            if (cmdReceived.IsFlagSet(Command.DATABASE_MODEMASK))
                            {
                                var showid = clientSocket.receiveI();
                                var description = clientSocket.receiveS();
                                addShowResult(showid, description);
                            }
                            else
                            {
                                var count = clientSocket.receiveI();
                                setShowResultCount((int)count);
                            }
                            break;
                        case Command.LISTING:
                            if (cmdReceived.IsFlagSet(Command.DATABASE_MODEMASK))
                            {
                                var listingid = clientSocket.receiveI();
                                var channel = clientSocket.receiveI();
                                var description = clientSocket.receiveS();
                                addListingResult(listingid, channel, description);
                            }
                            else
                            {
                                var count = clientSocket.receiveI();
                                setListingResultCount((int) count);
                            }
                            break;
                        case Command.BAPSDBERROR:
                            /** There is an error code in the command **/
                            notifyLoadShowError((ushort) (cmdReceived & Command.DATABASE_VALUEMASK), clientSocket.receiveS());
                            break;
                        default:
                            {
                                /** ERROR **/
                                sendQuit("Received unknown command, possibly a malformed DATABASE.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.CONFIG:
                    switch (cmdReceived & Command.CONFIG_OPMASK)
                    {
                        case Command.OPTION:
                            {
                                if (cmdReceived.IsFlagSet(Command.CONFIG_MODEMASK))
                                {
                                    var optionid = clientSocket.receiveI();
                                    var description = clientSocket.receiveS();
                                    var type = clientSocket.receiveI();
                                    processOption(cmdReceived, optionid, description, type);
                                }
                                else
                                {
                                    var count = clientSocket.receiveI();
                                    processOptionCount(count);
                                }
                            }
                            break;
                        case Command.OPTIONCHOICE:
                            {
                                if (cmdReceived.IsFlagSet(Command.CONFIG_MODEMASK))
                                {
                                    var optionid = clientSocket.receiveI();
                                    var choiceIndex = clientSocket.receiveI();
                                    var choiceDescription = clientSocket.receiveS();
                                    processChoice(optionid, choiceIndex, choiceDescription);
                                }
                                else
                                {
                                    var optionid = clientSocket.receiveI();
                                    var count = clientSocket.receiveI();
                                    processChoiceCount(optionid, count);
                                }
                            }
                            break;
                        case Command.CONFIGSETTING:
                            {
                                if (cmdReceived.IsFlagSet(Command.CONFIG_MODEMASK))
                                {
                                    var optionid = clientSocket.receiveI();
                                    var type = clientSocket.receiveI();
                                    processConfigSetting(cmdReceived, optionid, (ConfigType)type);
                                }
                                else
                                {
                                    _ = clientSocket.receiveI();
                                }
                            }
                            break;
                        case Command.CONFIGRESULT:
                            {
                                var optionid = clientSocket.receiveI();
                                var result = clientSocket.receiveI();
                                processConfigResult(cmdReceived, optionid, result);
                            }
                            break;
                        case Command.CONFIGERROR:
                            {
                                var errorCode = cmdReceived & Command.CONFIG_VALUEMASK;
                                var description = clientSocket.receiveS();
                                processConfigError((uint)errorCode, description);
                            }
                            break;
                        case Command.USER:
                            {
                                if (cmdReceived.IsFlagSet(Command.CONFIG_MODEMASK))
                                {
                                    var username = clientSocket.receiveS();
                                    var permissions = clientSocket.receiveI();
                                    processUserInfo(username, permissions);
                                }
                                else
                                {
                                    var count = clientSocket.receiveI();
                                    processUserCount(count);
                                }
                            }
                            break;
                        case Command.PERMISSION:
                            {
                                if (cmdReceived.IsFlagSet(Command.CONFIG_MODEMASK))
                                {
                                    var permissionCode = clientSocket.receiveI();
                                    var description = clientSocket.receiveS();
                                    processPermissionInfo(permissionCode, description);
                                }
                                else
                                {
                                    var count = clientSocket.receiveI();
                                    processPermissionCount(count);
                                }
                            }
                            break;
                        case Command.USERRESULT:
                            {
                                var resultCode = cmdReceived & Command.CONFIG_VALUEMASK;
                                var description = clientSocket.receiveS();
                                processUserResult((uint)resultCode, description);
                            }
                            break;
                        case Command.IPRESTRICTION:
                            {
                                if (cmdReceived.IsFlagSet(Command.CONFIG_MODEMASK))
                                {
                                    var ipaddress = clientSocket.receiveS();
                                    var mask = clientSocket.receiveI();
                                    processIPRestriction(cmdReceived, ipaddress, mask);
                                }
                                else
                                {
                                    var count = clientSocket.receiveI();
                                    processIPRestrictionCount(cmdReceived, count);
                                }
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                sendQuit("Received unknown command, possibly a malformed CONFIG.\n", false);
                            }
                            break;
                    }
                    break;
                case Command.SYSTEM:
                    switch (cmdReceived & Command.SYSTEM_OPMASK)
                    {
                        case Command.SENDLOGMESSAGE:
                            clientSocket.receiveS();
                            break;
                        case Command.FILENAME:
                            if (cmdReceived.IsFlagSet(Command.SYSTEM_MODEMASK))
                            {
                                var directoryIndex = cmdReceived & Command.SYSTEM_VALUEMASK;
                                var index = clientSocket.receiveI();
                                var description = clientSocket.receiveS();
                                Invoke((Action<ushort, uint, string>)addFileToDirectoryList, directoryIndex, index, description);
                            }
                            else
                            {
                                var directoryIndex = cmdReceived & Command.SYSTEM_VALUEMASK;
                                _ = clientSocket.receiveI();
                                var niceDirectoryName = clientSocket.receiveS();
                                Invoke((Action<ushort, string>)clearFiles, directoryIndex, niceDirectoryName);
                            }
                            break;
                        case Command.VERSION:
                            {
                                var version = clientSocket.receiveS();
                                var date = clientSocket.receiveS();
                                var time = clientSocket.receiveS();
                                var author = clientSocket.receiveS();
                                displayVersion(version, date, time, author);
                            }
                            break;
                        case Command.FEEDBACK:
                            {
                                _ = clientSocket.receiveI();
                            }
                            break;
                        case Command.SENDMESSAGE:
                            {
                                _ = clientSocket.receiveS();
                                _ = clientSocket.receiveS();
                                _ = clientSocket.receiveS();
                            }
                            break;
                        case Command.CLIENTCHANGE:
                            {
                                _ = clientSocket.receiveS();
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
                                bool expected = clientSocket.receiveI() == 0;
                                sendQuit("The Server is shutting down/restarting.\n", expected);
                            }
                            break;
                        default:
                            {
                                /** ERROR **/
                                sendQuit("Received unknown command, possibly a malformed SYSTEM.\n", false);
                            }
                            break;
                    }
                    break;
                default:
                    {
                        /** ERROR **/
                        sendQuit("Received unknown command.\n", false);
                    }
                    break;
            }

        }

        /** Loop to watch for an outgoing message on the queue and send it **/
        private void SenderFunc()
        {
            ActionMessage currentMessage = null;
            /** Drop out if something goes horribly wrong elsewhere **/
            while (!dead)
            {
                /** Grab a message if there is one **/
                if (msgQueue.Count > 0)
                {
                    currentMessage = (ActionMessage)msgQueue.Dequeue();
                    currentMessage.sendMsg(clientSocket);
                    /** Don't sleep if we have just processed a message, it is
                        likely there will be another
                    **/
                }
                else
                {
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        public void OpenAudioWall(TrackList tl)
        {
            if (audioWall == null || !audioWall.Visible)
            {
                audioWall = new AudioWall(this, msgQueue, tl);
                audioWall.Show();
            }
            else
            {
                audioWall.setChannel(tl);
                refreshAudioWall();
            }
        }
    }
}
