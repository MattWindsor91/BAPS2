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

        private ListBox[] directoryList;
        private Button[] directoryRefresh;
        private Timer countdownTimer;

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

#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showAudioWallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Directory0 = new System.Windows.Forms.ListBox();
            this.Directory1 = new System.Windows.Forms.ListBox();
            this.Directory2 = new System.Windows.Forms.ListBox();
            this.loadShowButton = new System.Windows.Forms.Button();
            this.bapsButton1 = new System.Windows.Forms.Button();
            this.Directory0Refresh = new System.Windows.Forms.Button();
            this.Directory1Refresh = new System.Windows.Forms.Button();
            this.Directory2Refresh = new System.Windows.Forms.Button();
            this.timeLine = new BAPSPresenter.TimeLine();
            this.bapsChannel1 = new BAPSPresenter2.BAPSChannel();
            this.bapsChannel2 = new BAPSPresenter2.BAPSChannel();
            this.bapsChannel3 = new BAPSPresenter2.BAPSChannel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.topButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.directory0Panel = new System.Windows.Forms.FlowLayoutPanel();
            this.directory1Panel = new System.Windows.Forms.FlowLayoutPanel();
            this.directory2Panel = new System.Windows.Forms.FlowLayoutPanel();
            this.MainTextDisplay = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.topButtonsPanel.SuspendLayout();
            this.directory0Panel.SuspendLayout();
            this.directory1Panel.SuspendLayout();
            this.directory2Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 6);
            // 
            // showAudioWallToolStripMenuItem
            // 
            this.showAudioWallToolStripMenuItem.Name = "showAudioWallToolStripMenuItem";
            this.showAudioWallToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // Directory0
            // 
            this.Directory0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory0.Location = new System.Drawing.Point(3, 32);
            this.Directory0.Name = "Directory0";
            this.Directory0.Size = new System.Drawing.Size(234, 132);
            this.Directory0.TabIndex = 215;
            this.Directory0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Directory_MouseDown);
            // 
            // Directory1
            // 
            this.Directory1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory1.Location = new System.Drawing.Point(3, 32);
            this.Directory1.Name = "Directory1";
            this.Directory1.Size = new System.Drawing.Size(234, 132);
            this.Directory1.TabIndex = 216;
            this.Directory1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Directory_MouseDown);
            // 
            // Directory2
            // 
            this.Directory2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory2.Location = new System.Drawing.Point(3, 32);
            this.Directory2.Name = "Directory2";
            this.Directory2.Size = new System.Drawing.Size(234, 132);
            this.Directory2.TabIndex = 217;
            this.Directory2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Directory_MouseDown);
            // 
            // loadShowButton
            // 
            this.loadShowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadShowButton.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadShowButton.Location = new System.Drawing.Point(3, 3);
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
            this.bapsButton1.Location = new System.Drawing.Point(123, 3);
            this.bapsButton1.Name = "bapsButton1";
            this.bapsButton1.Size = new System.Drawing.Size(114, 64);
            this.bapsButton1.TabIndex = 219;
            this.bapsButton1.Text = "Search Library";
            this.bapsButton1.Click += new System.EventHandler(this.SearchRecordLib_Click);
            // 
            // Directory0Refresh
            // 
            this.Directory0Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory0Refresh.Location = new System.Drawing.Point(3, 3);
            this.Directory0Refresh.Name = "Directory0Refresh";
            this.Directory0Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory0Refresh.TabIndex = 220;
            this.Directory0Refresh.Click += new System.EventHandler(this.RefreshDirectory_Click);
            // 
            // Directory1Refresh
            // 
            this.Directory1Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory1Refresh.Location = new System.Drawing.Point(3, 3);
            this.Directory1Refresh.Name = "Directory1Refresh";
            this.Directory1Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory1Refresh.TabIndex = 221;
            this.Directory1Refresh.Click += new System.EventHandler(this.RefreshDirectory_Click);
            // 
            // Directory2Refresh
            // 
            this.Directory2Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory2Refresh.Location = new System.Drawing.Point(3, 3);
            this.Directory2Refresh.Name = "Directory2Refresh";
            this.Directory2Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory2Refresh.TabIndex = 222;
            this.Directory2Refresh.Click += new System.EventHandler(this.RefreshDirectory_Click);
            // 
            // timeLine
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.timeLine, 4);
            this.timeLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeLine.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLine.Location = new System.Drawing.Point(3, 583);
            this.timeLine.Name = "timeLine";
            this.timeLine.Size = new System.Drawing.Size(1354, 52);
            this.timeLine.TabIndex = 214;
            this.timeLine.TabStop = false;
            this.timeLine.Text = "timeLine1";
            this.timeLine.StartTimeChanged += new BAPSPresenter.TimeLineEventHandler(this.timeLine_StartTimeChanged);
            // 
            // bapsChannel1
            // 
            this.bapsChannel1.AutoSize = true;
            this.bapsChannel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bapsChannel1.Location = new System.Drawing.Point(249, 3);
            this.bapsChannel1.Name = "bapsChannel1";
            this.tableLayoutPanel1.SetRowSpan(this.bapsChannel1, 4);
            this.bapsChannel1.Size = new System.Drawing.Size(365, 574);
            this.bapsChannel1.TabIndex = 225;
            // 
            // bapsChannel2
            // 
            this.bapsChannel2.AutoSize = true;
            this.bapsChannel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bapsChannel2.Location = new System.Drawing.Point(620, 3);
            this.bapsChannel2.Name = "bapsChannel2";
            this.tableLayoutPanel1.SetRowSpan(this.bapsChannel2, 4);
            this.bapsChannel2.Size = new System.Drawing.Size(365, 574);
            this.bapsChannel2.TabIndex = 226;
            // 
            // bapsChannel3
            // 
            this.bapsChannel3.AutoSize = true;
            this.bapsChannel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bapsChannel3.Location = new System.Drawing.Point(991, 3);
            this.bapsChannel3.Name = "bapsChannel3";
            this.tableLayoutPanel1.SetRowSpan(this.bapsChannel3, 4);
            this.bapsChannel3.Size = new System.Drawing.Size(366, 574);
            this.bapsChannel3.TabIndex = 227;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.bapsChannel3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.MainTextDisplay, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.timeLine, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.bapsChannel2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.bapsChannel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.topButtonsPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.directory0Panel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.directory1Panel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.directory2Panel, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1360, 737);
            this.tableLayoutPanel1.TabIndex = 228;
            // 
            // topButtonsPanel
            // 
            this.topButtonsPanel.AutoSize = true;
            this.topButtonsPanel.Controls.Add(this.loadShowButton);
            this.topButtonsPanel.Controls.Add(this.bapsButton1);
            this.topButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topButtonsPanel.Location = new System.Drawing.Point(3, 3);
            this.topButtonsPanel.Name = "topButtonsPanel";
            this.topButtonsPanel.Size = new System.Drawing.Size(240, 70);
            this.topButtonsPanel.TabIndex = 229;
            this.topButtonsPanel.WrapContents = false;
            // 
            // directory0Panel
            // 
            this.directory0Panel.Controls.Add(this.Directory0Refresh);
            this.directory0Panel.Controls.Add(this.Directory0);
            this.directory0Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directory0Panel.Location = new System.Drawing.Point(3, 79);
            this.directory0Panel.Name = "directory0Panel";
            this.directory0Panel.Size = new System.Drawing.Size(240, 162);
            this.directory0Panel.TabIndex = 228;
            // 
            // directory1Panel
            // 
            this.directory1Panel.Controls.Add(this.Directory1Refresh);
            this.directory1Panel.Controls.Add(this.Directory1);
            this.directory1Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directory1Panel.Location = new System.Drawing.Point(3, 247);
            this.directory1Panel.Name = "directory1Panel";
            this.directory1Panel.Size = new System.Drawing.Size(240, 162);
            this.directory1Panel.TabIndex = 230;
            // 
            // directory2Panel
            // 
            this.directory2Panel.Controls.Add(this.Directory2Refresh);
            this.directory2Panel.Controls.Add(this.Directory2);
            this.directory2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directory2Panel.Location = new System.Drawing.Point(3, 415);
            this.directory2Panel.Name = "directory2Panel";
            this.directory2Panel.Size = new System.Drawing.Size(240, 162);
            this.directory2Panel.TabIndex = 231;
            // 
            // MainTextDisplay
            // 
            this.MainTextDisplay.AcceptsReturn = true;
            this.MainTextDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.MainTextDisplay, 4);
            this.MainTextDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTextDisplay.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTextDisplay.Location = new System.Drawing.Point(3, 641);
            this.MainTextDisplay.Multiline = true;
            this.MainTextDisplay.Name = "MainTextDisplay";
            this.MainTextDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MainTextDisplay.Size = new System.Drawing.Size(1354, 93);
            this.MainTextDisplay.TabIndex = 29;
            this.MainTextDisplay.Text = "<You can type notes here>";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1360, 737);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BAPS Presenter";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BAPSPresenterMain_KeyDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.topButtonsPanel.ResumeLayout(false);
            this.directory0Panel.ResumeLayout(false);
            this.directory1Panel.ResumeLayout(false);
            this.directory2Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
#endregion

        private BAPSChannel bapsChannel1;
        private BAPSChannel bapsChannel2;
        private BAPSChannel bapsChannel3;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel topButtonsPanel;
        private FlowLayoutPanel directory0Panel;
        private FlowLayoutPanel directory1Panel;
        private FlowLayoutPanel directory2Panel;
        private TextBox MainTextDisplay;
    }
}
