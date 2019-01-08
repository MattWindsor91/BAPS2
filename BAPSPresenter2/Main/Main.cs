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

        private ChannelController[] controllers;
        private BAPSChannel[] bapsChannels;
        private BAPSDirectory[] bapsDirectories;

        private ClientCore core;
 
        /** Whether or not the timers are enabled **/
        private bool timersEnabled = true;

        private bool ChannelOutOfBounds(ushort channel) => bapsChannels.Length <= channel;
        private bool DirectoryOutOfBounds(ushort directory) => bapsDirectories.Length <= directory;

        public Main() : base()
        {
            InitializeComponent();
        }

        private Authenticator.Response LoginCallback()
        {
            using (var login = new Dialogs.Login())
            {
                if (login.ShowDialog() == DialogResult.Cancel)
                    return new Authenticator.Response { IsGivingUp = true };

                return new Authenticator.Response
                {
                    IsGivingUp = false,
                    Username = login.Username,
                    Password = login.Password,
                    Server = login.Server,
                    Port = login.Port
                };
            }
        }

        private void SetupAuthErrorReactions(object sender, Authenticator auth)
        {
            auth.ServerError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Server error:", MessageBoxButtons.OK);
                logError(errorMessage);
            };
            auth.UserError += (s, ErrorMessage) =>
            {
                MessageBox.Show(ErrorMessage, "Login error:", MessageBoxButtons.OK);
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ConfigManager.initConfigManager();

            core = new ClientCore(LoginCallback);
            core.AboutToAuthenticate += SetupAuthErrorReactions;
            core.Authenticated += Setup;
            core.ReceiverCreated += SetupReactions;
            var launched = core.Launch();
            if (!launched)
            {
                Close();
                return;
            }
        }

        private void Setup(object sender, EventArgs e)
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

            BAPSCommon.ConfigCache.initConfigCache();

            /** Enable or disable the timers depending on the config setting, enable on default when no registry config value set. **/
            var enableTimers = string.Compare(ConfigManager.getConfigValueString("EnableTimers", "Yes"), "Yes") == 0;
            EnableTimerControls(enableTimers);
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
            controllers = new ChannelController[bapsChannels.Length];
            foreach (var bc in bapsChannels)
            {
                Debug.Assert(0 <= bc.ChannelID, "Channel ID hasn't been set---check the channels' properties in the designer");

                controllers[bc.ChannelID] = new ChannelController((ushort)bc.ChannelID, core.SendQueue);

                bc.TrackListRequestChange += TrackList_RequestChange;
                bc.OpRequest += ChannelOperation_Click;
                bc.PositionRequestChange += HandlePositionChanged;
                bc.TimelineChanged += TimelineChanged;
                bc.ChannelConfigChange += HandleChannelConfigChange;
                //bc.TrackListContextMenuStripItemClicked += (e, x) => Invoke((ToolStripItemClickedEventHandler)trackListContextMenuStrip_ItemClicked, e, x);
            }
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

        private void SetupReactions(object sender, Receiver r)
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

        }

        public void OpenAudioWall(TrackList tl)
        {
            if (audioWall == null || !audioWall.Visible)
            {
                audioWall = new AudioWall(core.SendQueue, tl);
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
