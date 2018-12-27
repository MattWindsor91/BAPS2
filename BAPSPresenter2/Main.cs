﻿using BAPSPresenter; // Legacy
using System;
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

        /** Flag to say if the client crashed **/
        private bool crashed = false;
        // Accessor for the crashed variable.
        public bool hasCrashed { get => crashed; }

        /** A handle for the connection to the server **/
        private ClientSocket clientSocket;

        /** The current user **/
        private string username;


        /** The sender thread **/
        private System.Threading.Thread senderThread;
        /** The receiver thread **/
        private System.Threading.Thread receiverThread;
        /** Whether or not the timers are enabled **/
        private bool timersEnabled;

        /** The outgoing message queue (Should only have ActionMessage objects)**/
        private System.Collections.Queue msgQueue;

        private bool ChannelOutOfBounds(ushort channel) => 3 <= channel;

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
            trackLengthText = new BAPSFormControls.BAPSLabel[3] { Channel0Length, Channel1Length, Channel2Length };
            Channel0Length.Tag = new CountDownState(0);
            Channel1Length.Tag = new CountDownState(1);
            Channel2Length.Tag = new CountDownState(2);

            timeLeftText = new BAPSFormControls.BAPSLabel[3] { Channel0TimeLeft, Channel1TimeLeft, Channel2TimeLeft };
            timeGoneText = new BAPSFormControls.BAPSLabel[3] { Channel0TimeGone, Channel1TimeGone, Channel2TimeGone };
            timeGoneText[0] = Channel0TimeGone;
            timeGoneText[1] = Channel1TimeGone;
            timeGoneText[2] = Channel2TimeGone;

            channelPlay = new Button[3] { Channel0Play, Channel1Play, Channel2Play };
            channelPlay[0].Tag = new ChannelOperationLookup(0, (ushort)Command.PLAY);
            channelPlay[1].Tag = new ChannelOperationLookup(1, (ushort)Command.PLAY);
            channelPlay[2].Tag = new ChannelOperationLookup(2, (ushort)Command.PLAY);

            channelPause = new Button[3] { Channel0Pause, Channel1Pause, Channel2Pause };
            channelPause[0].Tag = new ChannelOperationLookup(0, (ushort)Command.PAUSE);
            channelPause[1].Tag = new ChannelOperationLookup(1, (ushort)Command.PAUSE);
            channelPause[2].Tag = new ChannelOperationLookup(2, (ushort)Command.PAUSE);

            channelStop = new Button[3] { Channel0Stop, Channel1Stop, Channel2Stop };
            channelStop[0].Tag = new ChannelOperationLookup(0, (ushort)Command.STOP);
            channelStop[1].Tag = new ChannelOperationLookup(1, (ushort)Command.STOP);
            channelStop[2].Tag = new ChannelOperationLookup(2, (ushort)Command.STOP);

            trackList = new TrackList[3] { trackList0, trackList1, trackList2 };

            directoryList = new ListBox[3] { Directory0, Directory1, Directory2 };
            directoryList[0].Tag = number0;
            directoryList[1].Tag = number1;
            directoryList[2].Tag = number2;

            directoryRefresh = new Button[3] { Directory0Refresh, Directory1Refresh, Directory2Refresh };
            directoryRefresh[0].Tag = number0;
            directoryRefresh[1].Tag = number1;
            directoryRefresh[2].Tag = number2;

            loadedText = new Label[3] { Channel0LoadedText, Channel1LoadedText, Channel2LoadedText };

            trackTime = new TrackTime[3] { trackTime0, trackTime1, trackTime2 };
            trackTime[0].Dock = DockStyle.None;
            trackTime[1].Dock = DockStyle.None;
            trackTime[2].Dock = DockStyle.None;
            Controls.Add(trackTime[0]);
            Controls.Add(trackTime[1]);
            Controls.Add(trackTime[2]);
            trackTime[0].PositionChanged += positionChanged;
            trackTime[1].PositionChanged += positionChanged;
            trackTime[2].PositionChanged += positionChanged;
            trackTime[0].CuePositionChanged += cuePositionChanged;
            trackTime[1].CuePositionChanged += cuePositionChanged;
            trackTime[2].CuePositionChanged += cuePositionChanged;
            trackTime[0].IntroPositionChanged += introPositionChanged;
            trackTime[1].IntroPositionChanged += introPositionChanged;
            trackTime[2].IntroPositionChanged += introPositionChanged;

            loadImpossibleTimer = new Timer[3];
            loadImpossibleTimer[0] = new Timer
            {
                Interval = 70,
                Tag = new ChannelTimeoutStruct(0, 10)
            };
            loadImpossibleTimer[0].Tick += loadImpossibleFlicker;
            loadImpossibleTimer[1] = new Timer
            {
                Interval = 70,
                Tag = new ChannelTimeoutStruct(1, 10)
            };
            loadImpossibleTimer[1].Tick += loadImpossibleFlicker;
            loadImpossibleTimer[2] = new Timer
            {
                Interval = 70,
                Tag = new ChannelTimeoutStruct(2, 10)
            };
            loadImpossibleTimer[2].Tick += loadImpossibleFlicker;
            nearEndTimer = new Timer[3];
            nearEndTimer[0] = new Timer
            {
                Interval = 100,
                Tag = 0
            };
            nearEndTimer[0].Tick += nearEndFlash;
            nearEndTimer[1] = new Timer
            {
                Interval = 100,
                Tag = 1
            };
            nearEndTimer[1].Tick += nearEndFlash;
            nearEndTimer[2] = new Timer
            {
                Interval = 100,
                Tag = 2
            };
            nearEndTimer[2].Tick += nearEndFlash;

            countdownTimer = new Timer
            {
                Interval = 200
            };
            countdownTimer.Tick += countdownTick;
            countdownTimer.Start();

#if false // TODO(@MattWindsor91): port this
                textDialog = new TextDialog(this, "Write on me");
#endif

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
                                        break;
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
                                Invoke((Action<ushort>) (x => textDialog.scroll(x)), updown);
                            }
                            break;
                        case Command.TEXTSIZE:
                            {
                                var updown = cmdReceived & Command.SYSTEM_VALUEMASK;
                                Invoke((Action<ushort>)(x => textDialog.textSize(x)), updown);
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
    }
}
