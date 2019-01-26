using System;
using System.Windows.Forms;
using BAPSClientCommon;
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
            if (disposing)
            {
                _core?.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /** Sub-form handles **/
        RecordLibrarySearch recordLibrarySearch = null;
        Dialogs.Config configDialog = null;
        LoadShowDialog loadShowDialog = null;
        Dialogs.Security securityDialog = null;
        Dialogs.About about = null;
        Dialogs.Text textDialog = null;
        AudioWall audioWall = null;

        private Timer countdownTimer;

        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem showAudioWallToolStripMenuItem;
        private BAPSFormControls.TimeLine timeLine;
        private Button loadShowButton;
        private Button bapsButton1;

#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showAudioWallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadShowButton = new System.Windows.Forms.Button();
            this.bapsButton1 = new System.Windows.Forms.Button();
            this.timeLine = new BAPSFormControls.TimeLine();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.directoryFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.bapsChannel3 = new BAPSPresenter2.BAPSChannel();
            this.MainTextDisplay = new System.Windows.Forms.TextBox();
            this.bapsChannel2 = new BAPSPresenter2.BAPSChannel();
            this.bapsChannel1 = new BAPSPresenter2.BAPSChannel();
            this.topButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.topButtonsPanel.SuspendLayout();
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
            // loadShowButton
            // 
            this.loadShowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.loadShowButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadShowButton.Location = new System.Drawing.Point(3, 3);
            this.loadShowButton.Name = "loadShowButton";
            this.loadShowButton.Size = new System.Drawing.Size(114, 64);
            this.loadShowButton.TabIndex = 218;
            this.loadShowButton.Text = "Load Show";
            this.loadShowButton.Click += new System.EventHandler(this.loadShow_Click);
            // 
            // bapsButton1
            // 
            this.bapsButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bapsButton1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bapsButton1.Location = new System.Drawing.Point(123, 3);
            this.bapsButton1.Name = "bapsButton1";
            this.bapsButton1.Size = new System.Drawing.Size(114, 64);
            this.bapsButton1.TabIndex = 219;
            this.bapsButton1.Text = "Search Library";
            this.bapsButton1.UseVisualStyleBackColor = true;
            this.bapsButton1.Click += new System.EventHandler(this.SearchRecordLib_Click);
            // 
            // timeLine
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.timeLine, 4);
            this.timeLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeLine.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLine.Location = new System.Drawing.Point(3, 583);
            this.timeLine.Name = "timeLine";
            this.timeLine.Size = new System.Drawing.Size(1354, 52);
            this.timeLine.TabIndex = 214;
            this.timeLine.TabStop = false;
            this.timeLine.Text = "timeLine1";
            this.timeLine.StartTimeChanged += new BAPSFormControls.TimeLine.TimeLineEventHandler(this.timeLine_StartTimeChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.directoryFlow, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bapsChannel3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.MainTextDisplay, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.timeLine, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bapsChannel2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.bapsChannel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.topButtonsPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1360, 737);
            this.tableLayoutPanel1.TabIndex = 228;
            // 
            // directoryFlow
            // 
            this.directoryFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.directoryFlow.Location = new System.Drawing.Point(3, 79);
            this.directoryFlow.Name = "directoryFlow";
            this.directoryFlow.Size = new System.Drawing.Size(240, 498);
            this.directoryFlow.TabIndex = 229;
            // 
            // bapsChannel3
            // 
            this.bapsChannel3.AutoSize = true;
            this.bapsChannel3.ChannelId = 2;
            this.bapsChannel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bapsChannel3.Location = new System.Drawing.Point(991, 3);
            this.bapsChannel3.Name = "bapsChannel3";
            this.tableLayoutPanel1.SetRowSpan(this.bapsChannel3, 2);
            this.bapsChannel3.Size = new System.Drawing.Size(366, 574);
            this.bapsChannel3.TabIndex = 227;
            this.bapsChannel3.Tag = "";
            // 
            // MainTextDisplay
            // 
            this.MainTextDisplay.AcceptsReturn = true;
            this.MainTextDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.MainTextDisplay, 4);
            this.MainTextDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTextDisplay.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTextDisplay.Location = new System.Drawing.Point(3, 641);
            this.MainTextDisplay.Multiline = true;
            this.MainTextDisplay.Name = "MainTextDisplay";
            this.MainTextDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MainTextDisplay.Size = new System.Drawing.Size(1354, 93);
            this.MainTextDisplay.TabIndex = 29;
            this.MainTextDisplay.Text = "<You can type notes here>";
            // 
            // bapsChannel2
            // 
            this.bapsChannel2.AutoSize = true;
            this.bapsChannel2.ChannelId = 1;
            this.bapsChannel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bapsChannel2.Location = new System.Drawing.Point(620, 3);
            this.bapsChannel2.Name = "bapsChannel2";
            this.tableLayoutPanel1.SetRowSpan(this.bapsChannel2, 2);
            this.bapsChannel2.Size = new System.Drawing.Size(365, 574);
            this.bapsChannel2.TabIndex = 226;
            this.bapsChannel2.Tag = "";
            // 
            // bapsChannel1
            // 
            this.bapsChannel1.AutoSize = true;
            this.bapsChannel1.ChannelId = 0;
            this.bapsChannel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bapsChannel1.Location = new System.Drawing.Point(249, 3);
            this.bapsChannel1.Name = "bapsChannel1";
            this.tableLayoutPanel1.SetRowSpan(this.bapsChannel1, 2);
            this.bapsChannel1.Size = new System.Drawing.Size(365, 574);
            this.bapsChannel1.TabIndex = 225;
            this.bapsChannel1.Tag = "";
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1360, 737);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.ResumeLayout(false);

        }
#endregion

        private BAPSChannel bapsChannel1;
        private BAPSChannel bapsChannel2;
        private BAPSChannel bapsChannel3;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel topButtonsPanel;
        private TextBox MainTextDisplay;
        private FlowLayoutPanel directoryFlow;
    }
}
