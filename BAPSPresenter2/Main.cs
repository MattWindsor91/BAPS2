using BAPSPresenter; // Legacy
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class Main : Form
    {
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
                    catch (System.Exception)
                    {
                        /** Do nothing **/
                    }
                    try
                    {
                        /** Attempt to make a connection to the specified server **/
                        clientSocket = new ClientSocket(login.getServer(), login.getPort());
                    }
                    catch (System.Exception e)
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
                var securedPassword = md5sum(string.Concat(randomSecurityString, md5sum(login.getPassword())));
                /** Send LOGIN command **/
                clientSocket.send((ushort)(Command.SYSTEM | Command.LOGIN | 0));
                /** Send correct command length **/
                clientSocket.send((uint)(login.getUsername().Length + securedPassword.Length));
                /** Send username **/
                clientSocket.send(login.getUsername());
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
            username = login.getUsername();
            /** Do the form initialization **/
            InitializeComponent();

            /** Tag codes for the controls so they can be identified at runtime **/
            object number0 = 0;
            object number1 = 1;
            object number2 = 2;

            /** Array initialisation so that controls can be found by channel
                number at runtime
            **/
            trackLengthText = new BAPSLabel[3];
            trackLengthText[0] = Channel0Length;
            trackLengthText[1] = Channel1Length;
            trackLengthText[2] = Channel2Length;
            Channel0Length.Tag = new CountDownState(0);
            Channel1Length.Tag = new CountDownState(1);
            Channel2Length.Tag = new CountDownState(2);

            timeLeftText = new BAPSLabel[3];
            timeLeftText[0] = Channel0TimeLeft;
            timeLeftText[1] = Channel1TimeLeft;
            timeLeftText[2] = Channel2TimeLeft;
            timeGoneText = new BAPSLabel[3];
            timeGoneText[0] = Channel0TimeGone;
            timeGoneText[1] = Channel1TimeGone;
            timeGoneText[2] = Channel2TimeGone;

            channelPlay = new Button[3];
            channelPlay[0] = Channel0Play;
            channelPlay[0].Tag = new ChannelOperationLookup(0, (ushort)Command.PLAY);
            channelPlay[1] = Channel1Play;
            channelPlay[1].Tag = new ChannelOperationLookup(1, (ushort)Command.PLAY);
            channelPlay[2] = Channel2Play;
            channelPlay[2].Tag = new ChannelOperationLookup(2, (ushort)Command.PLAY);

            channelPause = new Button[3];
            channelPause[0] = Channel0Pause;
            channelPause[0].Tag = new ChannelOperationLookup(0, (ushort)Command.PAUSE);
            channelPause[1] = Channel1Pause;
            channelPause[1].Tag = new ChannelOperationLookup(1, (ushort)Command.PAUSE);
            channelPause[2] = Channel2Pause;
            channelPause[2].Tag = new ChannelOperationLookup(2, (ushort)Command.PAUSE);

            channelStop = new Button[3];
            channelStop[0] = Channel0Stop;
            channelStop[0].Tag = new ChannelOperationLookup(0, (ushort)Command.STOP);
            channelStop[1] = Channel1Stop;
            channelStop[1].Tag = new ChannelOperationLookup(1, (ushort)Command.STOP);
            channelStop[2] = Channel2Stop;
            channelStop[2].Tag = new ChannelOperationLookup(2, (ushort)Command.STOP);

            trackList = new TrackList[3];
            trackList[0] = trackList0;
            trackList[1] = trackList1;
            trackList[2] = trackList2;

            directoryList = new ListBox[3];
            directoryList[0] = Directory0;
            directoryList[1] = Directory1;
            directoryList[2] = Directory2;
            directoryList[0].Tag = number0;
            directoryList[1].Tag = number1;
            directoryList[2].Tag = number2;

            directoryRefresh = new Button[3];
            directoryRefresh[0] = Directory0Refresh;
            directoryRefresh[1] = Directory1Refresh;
            directoryRefresh[2] = Directory2Refresh;
            directoryRefresh[0].Tag = number0;
            directoryRefresh[1].Tag = number1;
            directoryRefresh[2].Tag = number2;

            loadedText = new Label[3];
            loadedText[0] = Channel0LoadedText;
            loadedText[1] = Channel1LoadedText;
            loadedText[2] = Channel2LoadedText;

            trackTime = new TrackTime[3];
            trackTime[0] = trackTime0;
            trackTime[1] = trackTime1;
            trackTime[2] = trackTime2;

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
            receiverThread = new System.Threading.Thread(receiverFunc);
            receiverThread.Start();
            /** Start the sender thread last so that everything is ready for the autoupdate
                message to be sent and acted upon by the server
            **/
            senderThread = new System.Threading.Thread(senderFunc);
            senderThread.Start();
        }
    }
}
