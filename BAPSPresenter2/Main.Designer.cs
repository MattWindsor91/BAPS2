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

#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
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
            this.timeLine = new BAPSPresenter.TimeLine();
            this.bapsChannel1 = new BAPSPresenter2.BAPSChannel();
            this.bapsChannel2 = new BAPSPresenter2.BAPSChannel();
            this.bapsChannel3 = new BAPSPresenter2.BAPSChannel();
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
            // bapsChannel1
            // 
            this.bapsChannel1.AutoSize = true;
            this.bapsChannel1.Location = new System.Drawing.Point(252, 12);
            this.bapsChannel1.Name = "bapsChannel1";
            this.bapsChannel1.Size = new System.Drawing.Size(363, 577);
            this.bapsChannel1.TabIndex = 225;
            // 
            // bapsChannel2
            // 
            this.bapsChannel2.AutoSize = true;
            this.bapsChannel2.Location = new System.Drawing.Point(621, 12);
            this.bapsChannel2.Name = "bapsChannel2";
            this.bapsChannel2.Size = new System.Drawing.Size(363, 577);
            this.bapsChannel2.TabIndex = 226;
            // 
            // bapsChannel3
            // 
            this.bapsChannel3.AutoSize = true;
            this.bapsChannel3.Location = new System.Drawing.Point(990, 12);
            this.bapsChannel3.Name = "bapsChannel3";
            this.bapsChannel3.Size = new System.Drawing.Size(363, 577);
            this.bapsChannel3.TabIndex = 227;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1360, 737);
            this.ControlBox = false;
            this.Controls.Add(this.bapsChannel3);
            this.Controls.Add(this.bapsChannel2);
            this.Controls.Add(this.bapsChannel1);
            this.Controls.Add(this.loadShowButton);
            this.Controls.Add(this.Directory0Refresh);
            this.Controls.Add(this.bapsButton1);
            this.Controls.Add(this.Directory2Refresh);
            this.Controls.Add(this.Directory1Refresh);
            this.Controls.Add(this.timeLine);
            this.Controls.Add(this.MainTextDisplay);
            this.Controls.Add(this.Directory2);
            this.Controls.Add(this.Directory1);
            this.Controls.Add(this.Directory0);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }
#endregion

        private void trackList0_DragDrop(object sender, DragEventArgs e)
        {
        }

        private BAPSChannel bapsChannel1;
        private BAPSChannel bapsChannel2;
        private BAPSChannel bapsChannel3;
    }
}