namespace BAPSPresenter2
{
    partial class BAPSChannel
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.loadedText = new System.Windows.Forms.Label();
            this.trackTime = new BAPSFormControls.TrackTime();
            this.trackList = new TrackList();
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
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.timeGone = new BAPSFormControls.BAPSLabel();
            this.timeLeft = new BAPSFormControls.BAPSLabel();
            this.length = new BAPSFormControls.BAPSLabel();
            this.loadImpossibleTimer = new System.Windows.Forms.Timer(this.components);
            this.nearEndTimer = new System.Windows.Forms.Timer(this.components);
            this.layoutPanel.SuspendLayout();
            this.trackListContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoSize = true;
            this.layoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layoutPanel.ColumnCount = 3;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.layoutPanel.Controls.Add(this.loadedText, 0, 2);
            this.layoutPanel.Controls.Add(this.trackTime, 0, 3);
            this.layoutPanel.Controls.Add(this.trackList, 0, 0);
            this.layoutPanel.Controls.Add(this.playButton, 0, 1);
            this.layoutPanel.Controls.Add(this.stopButton, 2, 1);
            this.layoutPanel.Controls.Add(this.pauseButton, 1, 1);
            this.layoutPanel.Controls.Add(this.timeGone, 0, 4);
            this.layoutPanel.Controls.Add(this.timeLeft, 1, 4);
            this.layoutPanel.Controls.Add(this.length, 2, 4);
            this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 5;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.layoutPanel.Size = new System.Drawing.Size(372, 689);
            this.layoutPanel.TabIndex = 0;
            // 
            // loadedText
            // 
            this.loadedText.AutoEllipsis = true;
            this.loadedText.BackColor = System.Drawing.SystemColors.Window;
            this.loadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layoutPanel.SetColumnSpan(this.loadedText, 3);
            this.loadedText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.loadedText.Location = new System.Drawing.Point(3, 515);
            this.loadedText.Name = "loadedText";
            this.loadedText.Size = new System.Drawing.Size(366, 32);
            this.loadedText.TabIndex = 231;
            this.loadedText.Text = "--NONE--";
            this.loadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackTime
            // 
            this.trackTime.Channel = 0;
            this.layoutPanel.SetColumnSpan(this.trackTime, 3);
            this.trackTime.CuePosition = 0;
            this.trackTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackTime.Duration = 0;
            this.trackTime.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackTime.IntroPosition = 0;
            this.trackTime.Location = new System.Drawing.Point(3, 550);
            this.trackTime.Name = "trackTime";
            this.trackTime.Position = 0;
            this.trackTime.Size = new System.Drawing.Size(366, 72);
            this.trackTime.TabIndex = 227;
            this.trackTime.Text = "trackTime0";
            this.trackTime.PositionChanged += new BAPSCommon.PositionRequestChangeEventHandler(this.OnPositionChanged);
            // 
            // trackList
            // 
            this.trackList.AllowDrop = true;
            this.trackList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.trackList.Channel = 0;
            this.layoutPanel.SetColumnSpan(this.trackList, 3);
            this.trackList.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackList.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackList.LastIndexClicked = -1;
            this.trackList.LoadedIndex = -1;
            this.trackList.Location = new System.Drawing.Point(3, 3);
            this.trackList.Name = "trackList0";
            this.trackList.Size = new System.Drawing.Size(366, 477);
            this.trackList.TabIndex = 11;
            this.trackList.Text = "trackList";
            this.trackList.RequestChange += new RequestChangeEventHandler(this.TrackList_RequestChange);
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
            this.trackListContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.TrackListContextMenuStrip_Opening);
            this.trackListContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.TrackListContextMenuStrip_ItemClicked);
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
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.SystemColors.Control;
            this.playButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Location = new System.Drawing.Point(3, 486);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(117, 23);
            this.playButton.TabIndex = 225;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = false;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopButton.Location = new System.Drawing.Point(250, 486);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(119, 23);
            this.stopButton.TabIndex = 226;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.BackColor = System.Drawing.SystemColors.Control;
            this.pauseButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.pauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pauseButton.Location = new System.Drawing.Point(126, 486);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(118, 23);
            this.pauseButton.TabIndex = 224;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = false;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // timeGone
            // 
            this.timeGone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeGone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeGone.HighlightColor = System.Drawing.Color.Red;
            this.timeGone.Highlighted = false;
            this.timeGone.InfoText = "Elapsed:";
            this.timeGone.Location = new System.Drawing.Point(3, 628);
            this.timeGone.Name = "timeGone";
            this.timeGone.Size = new System.Drawing.Size(117, 58);
            this.timeGone.TabIndex = 228;
            this.timeGone.TabStop = false;
            // 
            // timeLeft
            // 
            this.timeLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeLeft.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.timeLeft.Highlighted = false;
            this.timeLeft.InfoText = "Remaining:";
            this.timeLeft.Location = new System.Drawing.Point(126, 628);
            this.timeLeft.Name = "timeLeft";
            this.timeLeft.Size = new System.Drawing.Size(118, 58);
            this.timeLeft.TabIndex = 229;
            this.timeLeft.TabStop = false;
            // 
            // length
            // 
            this.length.Dock = System.Windows.Forms.DockStyle.Fill;
            this.length.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.length.HighlightColor = System.Drawing.Color.Red;
            this.length.Highlighted = false;
            this.length.InfoText = "End At: 59:50";
            this.length.Location = new System.Drawing.Point(250, 628);
            this.length.Name = "length";
            this.length.Size = new System.Drawing.Size(119, 58);
            this.length.TabIndex = 230;
            this.length.TabStop = false;
            this.length.Tag = "";
            this.length.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Length_MouseDown);
            // 
            // loadImpossibleTimer
            // 
            this.loadImpossibleTimer.Interval = 70;
            this.loadImpossibleTimer.Tick += new System.EventHandler(this.LoadImpossibleFlicker);
            // 
            // nearEndTimer
            // 
            this.nearEndTimer.Tick += new System.EventHandler(this.NearEndFlash);
            // 
            // BAPSChannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutPanel);
            this.Name = "BAPSChannel";
            this.Size = new System.Drawing.Size(372, 689);
            this.layoutPanel.ResumeLayout(false);
            this.trackListContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layoutPanel;
        private TrackList trackList;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button stopButton;
        private BAPSFormControls.BAPSLabel timeGone;
        private BAPSFormControls.BAPSLabel timeLeft;
        private BAPSFormControls.BAPSLabel length;
        private BAPSFormControls.TrackTime trackTime;
        private System.Windows.Forms.Label loadedText;
        private System.Windows.Forms.Timer loadImpossibleTimer;
        private System.Windows.Forms.Timer nearEndTimer;
        private System.Windows.Forms.ContextMenuStrip trackListContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem resetChannelStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem automaticAdvanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playOnLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem repeatAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repeatOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repeatNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem showAudioWallToolStripMenuItem;
    }
}
