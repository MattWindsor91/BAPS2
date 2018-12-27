using System;
using System.Windows.Forms;
using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (msgQueue != null)
            {
                /** When we die it is only fair to tell the server **/
                var cmd = Command.SYSTEM | Command.END;
                msgQueue.Enqueue(new ActionMessageString((ushort)cmd, "Normal Termination"));
                /** Wait 500ms for the command to be sent **/
                int timeout = 500;
                while (msgQueue.Count > 0 && timeout > 0)
                {
                    System.Threading.Thread.Sleep(1);
                    timeout--;
                }
            }
            /** Notify the send/receive threads they should die **/
            dead = true;
            /** Empty the config cache **/
            ConfigCache.closeConfigCache();
            /** Force the receive thread to abort FIRST so that we cant receive
                any messages that need automatic responses **/
            if (receiverThread != null)
            {
                receiverThread.Abort();
                receiverThread.Join();
            }
            /** Force the sender thread to die (should be dead already) **/
            if (senderThread != null)
            {
                senderThread.Abort();
                senderThread.Join();
            }
            /** Close the connection properly **/
            clientSocket?.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /** Sub-form handles **/
        RecordLibrarySearch recordLibrarySearch = null;
        ConfigDialog configDialog = null;
        LoadShowDialog loadShowDialog = null;
        SecurityDialog securityDialog = null;
        AboutDialog about = null;
        TextDialog textDialog = null;
        AudioWall audioWall = null;

        /** Arrays with channel number indices for easy updating **/
        private ListBox[] directoryList;
        private Button[] directoryRefresh;
        private Label[] loadedText;
        private BAPSFormControls.BAPSLabel[] trackLengthText;
        private BAPSFormControls.BAPSLabel[] timeLeftText;
        private BAPSFormControls.BAPSLabel[] timeGoneText;
        private Button[] channelPlay;
        private Button[] channelPause;
        private Button[] channelStop;
        private TrackTime[] trackTime;
        private Timer[] loadImpossibleTimer;
        private Timer[] nearEndTimer;
        private Timer countdownTimer;
        private ContextMenuStrip trackListContextMenuStrip;

        private ToolStripMenuItem resetChannelStripMenuItem;

        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem deleteItemToolStripMenuItem;
        private ToolStripMenuItem automaticAdvanceToolStripMenuItem;
        private ToolStripMenuItem playOnLoadToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem repeatAllToolStripMenuItem;
        private ToolStripMenuItem repeatNoneToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;

        private ToolStripMenuItem repeatOneToolStripMenuItem;
        private TextBox MainTextDisplay;

        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem showAudioWallToolStripMenuItem;
        private TimeLine timeLine;
        private ListBox Directory0;
        private ListBox Directory1;
        private ListBox Directory2;
        private Button loadShowButton;
        private Button bapsButton1;
        private Button Directory0Refresh;
        private Button Directory1Refresh;
        private Button Directory2Refresh;
        private Button Channel0Play;
        private Button Channel0Pause;
        private Button Channel0Stop;
        private Button Channel1Play;
        private Button Channel1Pause;
        private Button Channel1Stop;
        private Button Channel2Play;
        private Button Channel2Pause;
        private Button Channel2Stop;
        private Label Channel0LoadedText;
        private Label Channel1LoadedText;
        private Label Channel2LoadedText;

        TrackList[] trackList;

        private BAPSFormControls.BAPSLabel Channel0TimeGone;
        private BAPSFormControls.BAPSLabel Channel0TimeLeft;
        private BAPSFormControls.BAPSLabel Channel0Length;
        private BAPSFormControls.BAPSLabel Channel1TimeGone;
        private BAPSFormControls.BAPSLabel Channel1TimeLeft;
        private BAPSFormControls.BAPSLabel Channel1Length;
        private BAPSFormControls.BAPSLabel Channel2TimeGone;
        private BAPSFormControls.BAPSLabel Channel2TimeLeft;
        private BAPSFormControls.BAPSLabel Channel2Length;

        private TrackTime trackTime0;
        private TrackTime trackTime1;
        private TrackTime trackTime2;
        private TrackList trackList0;
        private TrackList trackList1;
        private TrackList trackList2;


#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.trackListContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetChannelStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.automaticAdvanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playOnLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.repeatAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repeatOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repeatNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showAudioWallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTextDisplay = new System.Windows.Forms.TextBox();
            this.Directory0 = new System.Windows.Forms.ListBox();
            this.Directory1 = new System.Windows.Forms.ListBox();
            this.Directory2 = new System.Windows.Forms.ListBox();
            this.loadShowButton = new System.Windows.Forms.Button();
            this.bapsButton1 = new System.Windows.Forms.Button();
            this.Directory0Refresh = new System.Windows.Forms.Button();
            this.Directory1Refresh = new System.Windows.Forms.Button();
            this.Directory2Refresh = new System.Windows.Forms.Button();
            this.Channel0Play = new System.Windows.Forms.Button();
            this.Channel0Pause = new System.Windows.Forms.Button();
            this.Channel0Stop = new System.Windows.Forms.Button();
            this.Channel1Play = new System.Windows.Forms.Button();
            this.Channel1Pause = new System.Windows.Forms.Button();
            this.Channel1Stop = new System.Windows.Forms.Button();
            this.Channel2Play = new System.Windows.Forms.Button();
            this.Channel2Pause = new System.Windows.Forms.Button();
            this.Channel2Stop = new System.Windows.Forms.Button();
            this.Channel0LoadedText = new System.Windows.Forms.Label();
            this.Channel1LoadedText = new System.Windows.Forms.Label();
            this.Channel2LoadedText = new System.Windows.Forms.Label();
            this.timeLine = new BAPSPresenter.TimeLine();
            this.trackList0 = new BAPSPresenter.TrackList();
            this.trackList2 = new BAPSPresenter.TrackList();
            this.trackList1 = new BAPSPresenter.TrackList();
            this.trackTime1 = new BAPSPresenter.TrackTime();
            this.trackTime2 = new BAPSPresenter.TrackTime();
            this.trackTime0 = new BAPSPresenter.TrackTime();
            this.Channel2Length = new BAPSFormControls.BAPSLabel();
            this.Channel1TimeLeft = new BAPSFormControls.BAPSLabel();
            this.Channel1Length = new BAPSFormControls.BAPSLabel();
            this.Channel0Length = new BAPSFormControls.BAPSLabel();
            this.Channel2TimeLeft = new BAPSFormControls.BAPSLabel();
            this.Channel0TimeLeft = new BAPSFormControls.BAPSLabel();
            this.Channel1TimeGone = new BAPSFormControls.BAPSLabel();
            this.Channel2TimeGone = new BAPSFormControls.BAPSLabel();
            this.Channel0TimeGone = new BAPSFormControls.BAPSLabel();
            this.trackListContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackListContextMenuStrip
            // 
            this.trackListContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetChannelStripMenuItem,
            this.toolStripSeparator2,
            this.deleteItemToolStripMenuItem,
            this.toolStripSeparator3,
            this.automaticAdvanceToolStripMenuItem,
            this.playOnLoadToolStripMenuItem,
            this.toolStripSeparator1,
            this.repeatAllToolStripMenuItem,
            this.repeatOneToolStripMenuItem,
            this.repeatNoneToolStripMenuItem,
            this.toolStripSeparator4,
            this.showAudioWallToolStripMenuItem});
            this.trackListContextMenuStrip.Name = "trackListContextMenuStrip";
            this.trackListContextMenuStrip.ShowCheckMargin = true;
            this.trackListContextMenuStrip.ShowImageMargin = false;
            this.trackListContextMenuStrip.Size = new System.Drawing.Size(178, 204);
            this.trackListContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.trackListContextMenuStrip_Opening);
            this.trackListContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.trackListContextMenuStrip_ItemClicked);
            // 
            // resetChannelStripMenuItem
            // 
            this.resetChannelStripMenuItem.Name = "resetChannelStripMenuItem";
            this.resetChannelStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.resetChannelStripMenuItem.Text = "&Reset Channel";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // deleteItemToolStripMenuItem
            // 
            this.deleteItemToolStripMenuItem.Name = "deleteItemToolStripMenuItem";
            this.deleteItemToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.deleteItemToolStripMenuItem.Text = "&Delete Item";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(174, 6);
            // 
            // automaticAdvanceToolStripMenuItem
            // 
            this.automaticAdvanceToolStripMenuItem.Name = "automaticAdvanceToolStripMenuItem";
            this.automaticAdvanceToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.automaticAdvanceToolStripMenuItem.Text = "&Automatic advance";
            // 
            // playOnLoadToolStripMenuItem
            // 
            this.playOnLoadToolStripMenuItem.Name = "playOnLoadToolStripMenuItem";
            this.playOnLoadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.playOnLoadToolStripMenuItem.Text = "&Play on load";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // repeatAllToolStripMenuItem
            // 
            this.repeatAllToolStripMenuItem.Name = "repeatAllToolStripMenuItem";
            this.repeatAllToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.repeatAllToolStripMenuItem.Text = "Repeat a&ll";
            // 
            // repeatOneToolStripMenuItem
            // 
            this.repeatOneToolStripMenuItem.Name = "repeatOneToolStripMenuItem";
            this.repeatOneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.repeatOneToolStripMenuItem.Text = "Repeat &one";
            // 
            // repeatNoneToolStripMenuItem
            // 
            this.repeatNoneToolStripMenuItem.Name = "repeatNoneToolStripMenuItem";
            this.repeatNoneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.repeatNoneToolStripMenuItem.Text = "Repeat &none";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(174, 6);
            // 
            // showAudioWallToolStripMenuItem
            // 
            this.showAudioWallToolStripMenuItem.Name = "showAudioWallToolStripMenuItem";
            this.showAudioWallToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showAudioWallToolStripMenuItem.Text = "&Show AudioWall";
            // 
            // MainTextDisplay
            // 
            this.MainTextDisplay.AcceptsReturn = true;
            this.MainTextDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainTextDisplay.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTextDisplay.Location = new System.Drawing.Point(8, 647);
            this.MainTextDisplay.Multiline = true;
            this.MainTextDisplay.Name = "MainTextDisplay";
            this.MainTextDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MainTextDisplay.Size = new System.Drawing.Size(1024, 82);
            this.MainTextDisplay.TabIndex = 29;
            this.MainTextDisplay.Text = "<You can type notes here>";
            // 
            // Directory0
            // 
            this.Directory0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory0.Location = new System.Drawing.Point(12, 116);
            this.Directory0.Name = "Directory0";
            this.Directory0.Size = new System.Drawing.Size(234, 132);
            this.Directory0.TabIndex = 215;
            this.Directory0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Directory_MouseDown);
            // 
            // Directory1
            // 
            this.Directory1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory1.Location = new System.Drawing.Point(12, 276);
            this.Directory1.Name = "Directory1";
            this.Directory1.Size = new System.Drawing.Size(234, 132);
            this.Directory1.TabIndex = 216;
            this.Directory1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Directory_MouseDown);
            // 
            // Directory2
            // 
            this.Directory2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory2.Location = new System.Drawing.Point(12, 436);
            this.Directory2.Name = "Directory2";
            this.Directory2.Size = new System.Drawing.Size(234, 132);
            this.Directory2.TabIndex = 217;
            this.Directory2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Directory_MouseDown);
            // 
            // loadShowButton
            // 
            this.loadShowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadShowButton.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadShowButton.Location = new System.Drawing.Point(12, 12);
            this.loadShowButton.Name = "loadShowButton";
            this.loadShowButton.Size = new System.Drawing.Size(114, 64);
            this.loadShowButton.TabIndex = 218;
            this.loadShowButton.Text = "Load Show";
            this.loadShowButton.Click += new System.EventHandler(this.loadShow_Click);
            // 
            // bapsButton1
            // 
            this.bapsButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bapsButton1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bapsButton1.Location = new System.Drawing.Point(132, 12);
            this.bapsButton1.Name = "bapsButton1";
            this.bapsButton1.Size = new System.Drawing.Size(114, 64);
            this.bapsButton1.TabIndex = 219;
            this.bapsButton1.Text = "Search Library";
            this.bapsButton1.Click += new System.EventHandler(this.SearchRecordLib_Click);
            // 
            // Directory0Refresh
            // 
            this.Directory0Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory0Refresh.Location = new System.Drawing.Point(12, 94);
            this.Directory0Refresh.Name = "Directory0Refresh";
            this.Directory0Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory0Refresh.TabIndex = 220;
            this.Directory0Refresh.Click += new System.EventHandler(this.RefreshDirectory_Click);
            // 
            // Directory1Refresh
            // 
            this.Directory1Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory1Refresh.Location = new System.Drawing.Point(12, 254);
            this.Directory1Refresh.Name = "Directory1Refresh";
            this.Directory1Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory1Refresh.TabIndex = 221;
            this.Directory1Refresh.Click += new System.EventHandler(this.RefreshDirectory_Click);
            // 
            // Directory2Refresh
            // 
            this.Directory2Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory2Refresh.Location = new System.Drawing.Point(12, 414);
            this.Directory2Refresh.Name = "Directory2Refresh";
            this.Directory2Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory2Refresh.TabIndex = 222;
            this.Directory2Refresh.Click += new System.EventHandler(this.RefreshDirectory_Click);
            // 
            // Channel0Play
            // 
            this.Channel0Play.BackColor = System.Drawing.SystemColors.Control;
            this.Channel0Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel0Play.Location = new System.Drawing.Point(252, 415);
            this.Channel0Play.Name = "Channel0Play";
            this.Channel0Play.Size = new System.Drawing.Size(75, 23);
            this.Channel0Play.TabIndex = 223;
            this.Channel0Play.Text = "F1 - Play";
            this.Channel0Play.UseVisualStyleBackColor = false;
            this.Channel0Play.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel0Pause
            // 
            this.Channel0Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel0Pause.Location = new System.Drawing.Point(343, 415);
            this.Channel0Pause.Name = "Channel0Pause";
            this.Channel0Pause.Size = new System.Drawing.Size(75, 23);
            this.Channel0Pause.TabIndex = 223;
            this.Channel0Pause.Text = "F2 - Pause";
            this.Channel0Pause.UseVisualStyleBackColor = true;
            this.Channel0Pause.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel0Stop
            // 
            this.Channel0Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel0Stop.Location = new System.Drawing.Point(433, 415);
            this.Channel0Stop.Name = "Channel0Stop";
            this.Channel0Stop.Size = new System.Drawing.Size(75, 23);
            this.Channel0Stop.TabIndex = 223;
            this.Channel0Stop.Text = "F3 - Stop";
            this.Channel0Stop.UseVisualStyleBackColor = true;
            this.Channel0Stop.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel1Play
            // 
            this.Channel1Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel1Play.Location = new System.Drawing.Point(514, 415);
            this.Channel1Play.Name = "Channel1Play";
            this.Channel1Play.Size = new System.Drawing.Size(75, 23);
            this.Channel1Play.TabIndex = 223;
            this.Channel1Play.Text = "F5 - Play";
            this.Channel1Play.UseVisualStyleBackColor = true;
            this.Channel1Play.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel1Pause
            // 
            this.Channel1Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel1Pause.Location = new System.Drawing.Point(604, 415);
            this.Channel1Pause.Name = "Channel1Pause";
            this.Channel1Pause.Size = new System.Drawing.Size(75, 23);
            this.Channel1Pause.TabIndex = 223;
            this.Channel1Pause.Text = "F6 - Pause";
            this.Channel1Pause.UseVisualStyleBackColor = true;
            this.Channel1Pause.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel1Stop
            // 
            this.Channel1Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel1Stop.Location = new System.Drawing.Point(695, 415);
            this.Channel1Stop.Name = "Channel1Stop";
            this.Channel1Stop.Size = new System.Drawing.Size(75, 23);
            this.Channel1Stop.TabIndex = 223;
            this.Channel1Stop.Text = "F7 - Stop";
            this.Channel1Stop.UseVisualStyleBackColor = true;
            this.Channel1Stop.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel2Play
            // 
            this.Channel2Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel2Play.Location = new System.Drawing.Point(776, 415);
            this.Channel2Play.Name = "Channel2Play";
            this.Channel2Play.Size = new System.Drawing.Size(75, 23);
            this.Channel2Play.TabIndex = 223;
            this.Channel2Play.Text = "F9 - Play";
            this.Channel2Play.UseVisualStyleBackColor = true;
            this.Channel2Play.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel2Pause
            // 
            this.Channel2Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel2Pause.Location = new System.Drawing.Point(866, 415);
            this.Channel2Pause.Name = "Channel2Pause";
            this.Channel2Pause.Size = new System.Drawing.Size(75, 23);
            this.Channel2Pause.TabIndex = 223;
            this.Channel2Pause.Text = "F10 - Pause";
            this.Channel2Pause.UseVisualStyleBackColor = true;
            this.Channel2Pause.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel2Stop
            // 
            this.Channel2Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel2Stop.Location = new System.Drawing.Point(957, 415);
            this.Channel2Stop.Name = "Channel2Stop";
            this.Channel2Stop.Size = new System.Drawing.Size(75, 23);
            this.Channel2Stop.TabIndex = 223;
            this.Channel2Stop.Text = "F11 - Stop";
            this.Channel2Stop.UseVisualStyleBackColor = true;
            this.Channel2Stop.Click += new System.EventHandler(this.ChannelOperation_Click);
            // 
            // Channel0LoadedText
            // 
            this.Channel0LoadedText.AutoEllipsis = true;
            this.Channel0LoadedText.BackColor = System.Drawing.SystemColors.Window;
            this.Channel0LoadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Channel0LoadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Channel0LoadedText.Location = new System.Drawing.Point(252, 441);
            this.Channel0LoadedText.Name = "Channel0LoadedText";
            this.Channel0LoadedText.Size = new System.Drawing.Size(256, 30);
            this.Channel0LoadedText.TabIndex = 224;
            this.Channel0LoadedText.Text = "--NONE--";
            this.Channel0LoadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Channel1LoadedText
            // 
            this.Channel1LoadedText.AutoEllipsis = true;
            this.Channel1LoadedText.BackColor = System.Drawing.SystemColors.Window;
            this.Channel1LoadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Channel1LoadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Channel1LoadedText.Location = new System.Drawing.Point(514, 441);
            this.Channel1LoadedText.Name = "Channel1LoadedText";
            this.Channel1LoadedText.Size = new System.Drawing.Size(256, 30);
            this.Channel1LoadedText.TabIndex = 224;
            this.Channel1LoadedText.Text = "--NONE--";
            this.Channel1LoadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Channel2LoadedText
            // 
            this.Channel2LoadedText.AutoEllipsis = true;
            this.Channel2LoadedText.BackColor = System.Drawing.SystemColors.Window;
            this.Channel2LoadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Channel2LoadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Channel2LoadedText.Location = new System.Drawing.Point(776, 441);
            this.Channel2LoadedText.Name = "Channel2LoadedText";
            this.Channel2LoadedText.Size = new System.Drawing.Size(256, 30);
            this.Channel2LoadedText.TabIndex = 224;
            this.Channel2LoadedText.Text = "--NONE--";
            this.Channel2LoadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeLine
            // 
            this.timeLine.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLine.Location = new System.Drawing.Point(12, 589);
            this.timeLine.Name = "timeLine";
            this.timeLine.Size = new System.Drawing.Size(1020, 52);
            this.timeLine.TabIndex = 214;
            this.timeLine.TabStop = false;
            this.timeLine.Text = "timeLine1";
            this.timeLine.StartTimeChanged += new BAPSPresenter.TimeLineEventHandler(this.timeLine_StartTimeChanged);
            // 
            // trackList0
            // 
            this.trackList0.AllowDrop = true;
            this.trackList0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.trackList0.Channel = 0;
            this.trackList0.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList0.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackList0.LastIndexClicked = -1;
            this.trackList0.LoadedIndex = -1;
            this.trackList0.Location = new System.Drawing.Point(252, 12);
            this.trackList0.Name = "trackList0";
            this.trackList0.Size = new System.Drawing.Size(256, 397);
            this.trackList0.TabIndex = 10;
            this.trackList0.Text = "trackList0";
            this.trackList0.RequestChange += new BAPSPresenter.RequestChangeEventHandler(this.TrackList_RequestChange);
            // 
            // trackList2
            // 
            this.trackList2.AllowDrop = true;
            this.trackList2.Channel = 2;
            this.trackList2.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackList2.LastIndexClicked = -1;
            this.trackList2.LoadedIndex = -1;
            this.trackList2.Location = new System.Drawing.Point(776, 12);
            this.trackList2.Name = "trackList2";
            this.trackList2.Size = new System.Drawing.Size(256, 397);
            this.trackList2.TabIndex = 14;
            this.trackList2.Text = "trackList2";
            this.trackList2.RequestChange += new BAPSPresenter.RequestChangeEventHandler(this.TrackList_RequestChange);
            // 
            // trackList1
            // 
            this.trackList1.AllowDrop = true;
            this.trackList1.Channel = 1;
            this.trackList1.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackList1.LastIndexClicked = -1;
            this.trackList1.LoadedIndex = -1;
            this.trackList1.Location = new System.Drawing.Point(514, 12);
            this.trackList1.Name = "trackList1";
            this.trackList1.Size = new System.Drawing.Size(256, 397);
            this.trackList1.TabIndex = 12;
            this.trackList1.Text = "trackList1";
            this.trackList1.RequestChange += new BAPSPresenter.RequestChangeEventHandler(this.TrackList_RequestChange);
            // 
            // trackTime1
            // 
            this.trackTime1.Channel = 1;
            this.trackTime1.CuePosition = 0;
            this.trackTime1.Duration = 0;
            this.trackTime1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackTime1.IntroPosition = 0;
            this.trackTime1.Location = new System.Drawing.Point(514, 474);
            this.trackTime1.Name = "trackTime1";
            this.trackTime1.Position = 0;
            this.trackTime1.SilencePosition = 0;
            this.trackTime1.Size = new System.Drawing.Size(256, 72);
            this.trackTime1.TabIndex = 26;
            this.trackTime1.Text = "trackTime1";
            // 
            // trackTime2
            // 
            this.trackTime2.Channel = 2;
            this.trackTime2.CuePosition = 0;
            this.trackTime2.Duration = 0;
            this.trackTime2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackTime2.IntroPosition = 0;
            this.trackTime2.Location = new System.Drawing.Point(776, 474);
            this.trackTime2.Name = "trackTime2";
            this.trackTime2.Position = 0;
            this.trackTime2.SilencePosition = 0;
            this.trackTime2.Size = new System.Drawing.Size(256, 72);
            this.trackTime2.TabIndex = 27;
            this.trackTime2.Text = "trackTime2";
            // 
            // trackTime0
            // 
            this.trackTime0.Channel = 0;
            this.trackTime0.CuePosition = 0;
            this.trackTime0.Duration = 0;
            this.trackTime0.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackTime0.IntroPosition = 0;
            this.trackTime0.Location = new System.Drawing.Point(252, 474);
            this.trackTime0.Name = "trackTime0";
            this.trackTime0.Position = 0;
            this.trackTime0.SilencePosition = 0;
            this.trackTime0.Size = new System.Drawing.Size(256, 72);
            this.trackTime0.TabIndex = 25;
            this.trackTime0.Text = "trackTime0";
            // 
            // Channel2Length
            // 
            this.Channel2Length.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel2Length.HighlightColor = System.Drawing.Color.Red;
            this.Channel2Length.Highlighted = false;
            this.Channel2Length.InfoText = "End At: 59:50";
            this.Channel2Length.Location = new System.Drawing.Point(950, 551);
            this.Channel2Length.Name = "Channel2Length";
            this.Channel2Length.Size = new System.Drawing.Size(82, 32);
            this.Channel2Length.TabIndex = 117;
            this.Channel2Length.TabStop = false;
            this.Channel2Length.Tag = "";
            this.Channel2Length.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChannelLength_MouseDown);
            // 
            // Channel1TimeLeft
            // 
            this.Channel1TimeLeft.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel1TimeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.Channel1TimeLeft.Highlighted = false;
            this.Channel1TimeLeft.InfoText = "Remaining:";
            this.Channel1TimeLeft.Location = new System.Drawing.Point(599, 551);
            this.Channel1TimeLeft.Name = "Channel1TimeLeft";
            this.Channel1TimeLeft.Size = new System.Drawing.Size(82, 32);
            this.Channel1TimeLeft.TabIndex = 113;
            this.Channel1TimeLeft.TabStop = false;
            // 
            // Channel1Length
            // 
            this.Channel1Length.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel1Length.HighlightColor = System.Drawing.Color.Red;
            this.Channel1Length.Highlighted = false;
            this.Channel1Length.InfoText = "End At: 59:50";
            this.Channel1Length.Location = new System.Drawing.Point(687, 551);
            this.Channel1Length.Name = "Channel1Length";
            this.Channel1Length.Size = new System.Drawing.Size(82, 32);
            this.Channel1Length.TabIndex = 114;
            this.Channel1Length.TabStop = false;
            this.Channel1Length.Tag = "";
            this.Channel1Length.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChannelLength_MouseDown);
            // 
            // Channel0Length
            // 
            this.Channel0Length.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel0Length.HighlightColor = System.Drawing.Color.Red;
            this.Channel0Length.Highlighted = false;
            this.Channel0Length.InfoText = "End At: 59:50";
            this.Channel0Length.Location = new System.Drawing.Point(426, 551);
            this.Channel0Length.Name = "Channel0Length";
            this.Channel0Length.Size = new System.Drawing.Size(82, 32);
            this.Channel0Length.TabIndex = 111;
            this.Channel0Length.TabStop = false;
            this.Channel0Length.Tag = "";
            this.Channel0Length.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChannelLength_MouseDown);
            // 
            // Channel2TimeLeft
            // 
            this.Channel2TimeLeft.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel2TimeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.Channel2TimeLeft.Highlighted = false;
            this.Channel2TimeLeft.InfoText = "Remaining:";
            this.Channel2TimeLeft.Location = new System.Drawing.Point(863, 551);
            this.Channel2TimeLeft.Name = "Channel2TimeLeft";
            this.Channel2TimeLeft.Size = new System.Drawing.Size(82, 32);
            this.Channel2TimeLeft.TabIndex = 116;
            this.Channel2TimeLeft.TabStop = false;
            // 
            // Channel0TimeLeft
            // 
            this.Channel0TimeLeft.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel0TimeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.Channel0TimeLeft.Highlighted = false;
            this.Channel0TimeLeft.InfoText = "Remaining:";
            this.Channel0TimeLeft.Location = new System.Drawing.Point(339, 551);
            this.Channel0TimeLeft.Name = "Channel0TimeLeft";
            this.Channel0TimeLeft.Size = new System.Drawing.Size(82, 32);
            this.Channel0TimeLeft.TabIndex = 110;
            this.Channel0TimeLeft.TabStop = false;
            // 
            // Channel1TimeGone
            // 
            this.Channel1TimeGone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel1TimeGone.HighlightColor = System.Drawing.Color.Red;
            this.Channel1TimeGone.Highlighted = false;
            this.Channel1TimeGone.InfoText = "Elapsed:";
            this.Channel1TimeGone.Location = new System.Drawing.Point(514, 551);
            this.Channel1TimeGone.Name = "Channel1TimeGone";
            this.Channel1TimeGone.Size = new System.Drawing.Size(82, 32);
            this.Channel1TimeGone.TabIndex = 112;
            this.Channel1TimeGone.TabStop = false;
            // 
            // Channel2TimeGone
            // 
            this.Channel2TimeGone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel2TimeGone.HighlightColor = System.Drawing.Color.Red;
            this.Channel2TimeGone.Highlighted = false;
            this.Channel2TimeGone.InfoText = "Elapsed:";
            this.Channel2TimeGone.Location = new System.Drawing.Point(776, 551);
            this.Channel2TimeGone.Name = "Channel2TimeGone";
            this.Channel2TimeGone.Size = new System.Drawing.Size(82, 32);
            this.Channel2TimeGone.TabIndex = 115;
            this.Channel2TimeGone.TabStop = false;
            // 
            // Channel0TimeGone
            // 
            this.Channel0TimeGone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Channel0TimeGone.HighlightColor = System.Drawing.Color.Red;
            this.Channel0TimeGone.Highlighted = false;
            this.Channel0TimeGone.InfoText = "Elapsed:";
            this.Channel0TimeGone.Location = new System.Drawing.Point(252, 551);
            this.Channel0TimeGone.Name = "Channel0TimeGone";
            this.Channel0TimeGone.Size = new System.Drawing.Size(82, 32);
            this.Channel0TimeGone.TabIndex = 109;
            this.Channel0TimeGone.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1044, 737);
            this.ControlBox = false;
            this.Controls.Add(this.Channel2LoadedText);
            this.Controls.Add(this.Channel0LoadedText);
            this.Controls.Add(this.Channel1LoadedText);
            this.Controls.Add(this.Channel2Stop);
            this.Controls.Add(this.Channel1Stop);
            this.Controls.Add(this.Channel0Stop);
            this.Controls.Add(this.Channel2Pause);
            this.Controls.Add(this.Channel1Pause);
            this.Controls.Add(this.Channel0Pause);
            this.Controls.Add(this.Channel2Play);
            this.Controls.Add(this.Channel1Play);
            this.Controls.Add(this.loadShowButton);
            this.Controls.Add(this.Channel0Play);
            this.Controls.Add(this.Directory0Refresh);
            this.Controls.Add(this.bapsButton1);
            this.Controls.Add(this.Directory2Refresh);
            this.Controls.Add(this.Directory1Refresh);
            this.Controls.Add(this.timeLine);
            this.Controls.Add(this.MainTextDisplay);
            this.Controls.Add(this.Directory2);
            this.Controls.Add(this.Channel2Length);
            this.Controls.Add(this.Channel1TimeLeft);
            this.Controls.Add(this.Directory1);
            this.Controls.Add(this.Channel1Length);
            this.Controls.Add(this.Directory0);
            this.Controls.Add(this.trackList0);
            this.Controls.Add(this.Channel0Length);
            this.Controls.Add(this.Channel2TimeLeft);
            this.Controls.Add(this.Channel0TimeLeft);
            this.Controls.Add(this.Channel1TimeGone);
            this.Controls.Add(this.Channel2TimeGone);
            this.Controls.Add(this.Channel0TimeGone);
            this.Controls.Add(this.trackList2);
            this.Controls.Add(this.trackList1);
            this.Controls.Add(this.trackTime1);
            this.Controls.Add(this.trackTime2);
            this.Controls.Add(this.trackTime0);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BAPS Presenter";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BAPSPresenterMain_KeyDown);
            this.trackListContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
#endregion

        private void trackList0_DragDrop(object sender, DragEventArgs e)
        {
        }
    }
}