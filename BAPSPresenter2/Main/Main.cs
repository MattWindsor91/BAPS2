using BAPSCommon;
using BAPSPresenter; // Legacy
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class Main : Form
    {
        /** This flag is used to cleanly exit the send/receive loops
            in the case of the receive loop, the flag will not take effect
            until data is received, so an abort message is still required
        **/
        private System.Threading.CancellationTokenSource dead = new System.Threading.CancellationTokenSource();

        // Accessor for the crashed variable.
        public bool HasCrashed { get; private set; } = false;

        /** A handle for the connection to the server **/
        private BAPSCommon.ClientSocket clientSocket;

        /** The current user **/
        private string username;

        private BAPSChannel[] bapsChannels;
        private BAPSDirectory[] bapsDirectories;

        private Task senderTask;
        private Receiver receiver;
        private Task receiverTask;
 
        /** Whether or not the timers are enabled **/
        private bool timersEnabled = true;

        /** The outgoing message queue **/
        private System.Collections.Concurrent.BlockingCollection<BAPSCommon.Message> msgQueue;

        private bool ChannelOutOfBounds(ushort channel) => 3 <= channel;
        private bool DirectoryOutOfBounds(ushort directory) => 3 <= directory;

        public Main() : base()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ConfigManager.initConfigManager();

            var authenticated = Authenticate();
            if (!authenticated)
            {
                Close();
                return;
            }

            Setup();
        }

        private bool Authenticate()
        {
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
                if (login.ShowDialog() == DialogResult.Cancel) return false;
                /** If either the server or port have been changed since last attempt
                    we need to reconnect.
                **/
                if (login.needsToConnect() || wasServerError)
                {
                    randomSecurityString = MakeNewConnection(login.Server, login.Port);
                    if (randomSecurityString == null)
                    {
                        wasServerError = true;
                        continue;
                    }
                }
                var securedPassword = Md5sum(string.Concat(randomSecurityString, Md5sum(login.Password)));

                (authenticated, wasServerError) = TryLogin(login.Username, securedPassword);
            }

            username = login.Username;
            return true;
        }

        /// <summary>
        /// Makes a new bapsnet connection to the given server and port.
        /// <para>
        /// On success, the new connection is placed in <see cref="clientSocket"/>.
        /// </para>
        /// </summary>
        /// <param name="server">The server host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>
        /// The seed to use for encrypting the password when logging
        /// into the server.  If null, there was a connection failure.
        /// </returns>
        private string MakeNewConnection(string server, int port)
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
                clientSocket = new BAPSCommon.ClientSocket(server, port, dead.Token, dead.Token);
            }
            catch (Exception e)
            {
                /** If an error occurs just give the exception message and start again **/
                var errorMessage = string.Concat("System Error:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                MessageBox.Show(errorMessage, "Server error:", MessageBoxButtons.OK);
                logError(errorMessage);
                return null;
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
                return null;
            }

            /** Receive the SEED **/
            var randomSecurityString = clientSocket.ReceiveS();
            return randomSecurityString;
        }

        private (bool authenticated, bool wasServerError) TryLogin(string username, string password)
        {
            clientSocket.Send(Command.SYSTEM | Command.LOGIN | 0);
            clientSocket.Send((uint)(username.Length + password.Length));
            clientSocket.Send(username);
            clientSocket.Send(password);

            var authResult = clientSocket.ReceiveC();
            /** Verify it is what we expected **/
            if ((authResult & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.LOGINRESULT))
            {
                MessageBox.Show("Server login procedure is not compatible with this client.", "Server error:", MessageBoxButtons.OK);
                /** This is a server error as the server is incompatible with this client **/
                return (authenticated: false, wasServerError: true);
            }

            /** Receive the result command length **/
            clientSocket.ReceiveI();
            /** Receive the description of the result code **/
            var description = clientSocket.ReceiveS();
            /** Check the value for '0' meaning success **/
            var authenticated = (authResult & Command.SYSTEM_VALUEMASK) == 0;
            if (!authenticated)
            {
                /** Tell the client of any failure **/
                MessageBox.Show(description, "Login error:", MessageBoxButtons.OK);
            }

            return (authenticated, wasServerError: false);
        }

        private void Setup()
        {
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

            var tf = new TaskFactory(dead.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None, TaskScheduler.Current);

            receiver = new Receiver(clientSocket, dead.Token);
            SetupReactions(receiver);

            receiverTask = tf.StartNew(receiver.Run);
            senderTask = tf.StartNew(SenderFunc);
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
            dead.Cancel();
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

        private void SetupReactions(Receiver r)
        {
            SetupPlaybackReactions(r);
            SetupPlaylistReactions(r);
            SetupDatabaseReactions(r);
            SetupConfigReactions(r);
            SetupSystemReactions(r);
            SetupGeneralReactions(r);
        }

        private void SetupGeneralReactions(Receiver r)
        {
            r.IncomingCount += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Receiver.CountEventHandler)HandleCount, sender, e);
                }
                else HandleCount(sender, e);
            };
            r.Error += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Receiver.ErrorEventHandler)HandleError, sender, e);
                }
                else HandleError(sender, e);
            };
            r.UnknownCommand += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<Command, string>)HandleUnknownCommand, e.command, e.description);
                }
                else HandleUnknownCommand(e.command, e.description);
            };
        }

        private void HandleCount(object sender, Receiver.CountEventArgs e)
        {
            switch (e.Type)
            {
                case Receiver.CountType.ConfigChoice:
                    processChoiceCount(e.Extra, (int)e.Count);
                    break;
                case Receiver.CountType.ConfigOption:
                    processOptionCount(e.Count);
                    break;
                case Receiver.CountType.IPRestriction:
                    break;
                case Receiver.CountType.LibraryItem:
                    setLibraryResultCount((int)e.Count);
                    break;
                case Receiver.CountType.Listing:
                    setListingResultCount((int)e.Count);
                    break;
                case Receiver.CountType.Permission:
                    processPermissionCount(e.Count);
                    break;
                case Receiver.CountType.Show:
                    setShowResultCount((int)e.Count);
                    break;
                case Receiver.CountType.User:
                    processUserCount(e.Count);
                    break;
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException("e.Type", e.Type, "Unsupported count type");
#else
                    break;
#endif
            }
        }

        private void HandleError(object sender, Receiver.ErrorEventArgs e)
        {
            switch (e.Type)
            {
                case Receiver.ErrorType.Library:
                    notifyLibraryError(e.Code, e.Description);
                    break;
                case Receiver.ErrorType.BapsDB:
                    notifyLoadShowError(e.Code, e.Description);
                    break;
                case Receiver.ErrorType.Config:
                    processConfigError(e.Code, e.Description);
                    break;
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException("e.Type", e.Type, "Unsupported error type");
#else
                    break;
#endif
            }
        }

        private void HandleUnknownCommand(Command command, string description)
        {
            SendQuit($"Received unknown command: {description}\n", false);
        }


        /** Loop to watch for an outgoing message on the queue and send it **/
        private void SenderFunc()
        {
            var tok = dead.Token;
            try
            {
                while (true)
                {
                    tok.ThrowIfCancellationRequested();
                    var msg = msgQueue.Take(tok);
                    msg.Send(clientSocket);
                }
            }
            finally
            {
                clientSocket.ShutdownSend();
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
