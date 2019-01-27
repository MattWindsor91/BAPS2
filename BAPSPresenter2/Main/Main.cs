using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BAPSClientCommon;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSPresenter2.Dialogs;
using Timer = System.Windows.Forms.Timer;
// Legacy

namespace BAPSPresenter2
{
    public partial class Main : Form
    {
        /// <summary>
        /// Singleton for the config cache.
        /// </summary>
        public static ConfigCache Config => _config ?? (_config = new ConfigCache());
        private static ConfigCache _config;

        /** This flag is used to cleanly exit the send/receive loops
            in the case of the receive loop, the flag will not take effect
            until data is received, so an abort message is still required
        **/
        private CancellationTokenSource dead = new CancellationTokenSource();

        // Accessor for the crashed variable.
        public bool HasCrashed { get; private set; }

        private ChannelController[] _controllers;
        private BAPSChannel[] _channels;
        private BAPSDirectory[] _directories;

        private ClientCore _core;
 
        /** Whether or not the timers are enabled **/
        private bool _timersEnabled = true;

        private bool ChannelOutOfBounds(ushort channel) => _channels.Length <= channel;
        private bool DirectoryOutOfBounds(ushort directory) => _directories.Length <= directory;

        public Main()
        {
            InitializeComponent();
        }

        private Authenticator.Response LoginCallback()
        {
            using (var login = new Login())
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
            auth.UserError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Login error:", MessageBoxButtons.OK);
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ConfigManager.initConfigManager();

            _core = new ClientCore(LoginCallback);
            _core.AboutToAuthenticate += SetupAuthErrorReactions;
            _core.Authenticated += Setup;
            _core.ReceiverCreated += SetupReactions;
            var launched = _core.Launch();
            if (!launched)
            {
                Close();
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

            textDialog = new Text("Write on me");
            textDialog.KeyDownForward += BAPSPresenterMain_KeyDown;

            /** Enable or disable the timers depending on the config setting, enable on default when no registry config value set. **/
            var enableTimers = string.Compare(ConfigManager.getConfigValueString("EnableTimers", "Yes"), "Yes") == 0;
            EnableTimerControls(enableTimers);
        }

        private void SetupDirectories()
        {
            _directories = new BAPSDirectory[3];
            for (var i = 0; i < _directories.Length; i++)
            {
                _directories[i] = new BAPSDirectory
                {
                    DirectoryID = i
                };
                _directories[i].RefreshRequest += RefreshDirectory;
            }
            directoryFlow.Controls.AddRange(_directories);
        }

        private void SetupChannels()
        {
            _channels = new BAPSChannel[3] { bapsChannel1, bapsChannel2, bapsChannel3 };
            _controllers = new ChannelController[_channels.Length];
            foreach (var bc in _channels)
            {
                Debug.Assert(0 <= bc.ChannelId, "Channel ID hasn't been set---check the channels' properties in the designer");

                var cont = new ChannelController((ushort)bc.ChannelId, _core.SendQueue, Config);            
                _controllers[bc.ChannelId] = cont;
                bc.TrackListRequestChange += TrackList_RequestChange;
                bc.OpRequest += HandleChannelStateRequest;
                bc.PositionRequestChange += HandlePositionChanged;
                bc.TimelineChanged += TimelineChanged;
                bc.ChannelConfigChange += HandleChannelConfigChange;
                bc.AudioWallRequest += (sender, args) => OpenAudioWall(args);
                bc.ItemDeleteRequest += (sender, args) => cont.DeleteItemAt(args.Index);
                bc.ChannelResetRequest += (sender, args) => cont.Reset();
            }
        }

        /** Enable or disable the timer controls **/
        private void EnableTimerControls(bool shouldEnable)
        {
            _timersEnabled = shouldEnable;
            foreach (var chan in _channels) chan.EnableTimerControls(shouldEnable);
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
                using (var stream = new StreamWriter("bapserror.log", true))
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
                    Invoke((Updates.CountEventHandler)HandleCount, sender, e);
                }
                else HandleCount(sender, e);
            };
            r.Error += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Updates.ErrorEventHandler)HandleError, sender, e);
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

        private void HandleCount(object sender, Updates.CountEventArgs e)
        {
            switch (e.Type)
            {
                case Updates.CountType.ConfigChoice:
                    processChoiceCount(e.Extra, (int)e.Count);
                    break;
                case Updates.CountType.ConfigOption:
                    processOptionCount(e.Count);
                    break;
                case Updates.CountType.IpRestriction:
                    break;
                case Updates.CountType.LibraryItem:
                    setLibraryResultCount((int)e.Count);
                    break;
                case Updates.CountType.Listing:
                    setListingResultCount((int)e.Count);
                    break;
                case Updates.CountType.Permission:
                    processPermissionCount(e.Count);
                    break;
                case Updates.CountType.Show:
                    setShowResultCount((int)e.Count);
                    break;
                case Updates.CountType.User:
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

        private void HandleError(object sender, Updates.ErrorEventArgs e)
        {
            switch (e.Type)
            {
                case Updates.ErrorType.Library:
                    notifyLibraryError(e.Code, e.Description);
                    break;
                case Updates.ErrorType.BapsDb:
                    notifyLoadShowError(e.Code, e.Description);
                    break;
                case Updates.ErrorType.Config:
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
                audioWall = new AudioWall(_core.SendQueue, tl);
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
