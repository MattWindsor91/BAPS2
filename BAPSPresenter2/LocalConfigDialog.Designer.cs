using System.Windows.Forms;

namespace BAPSPresenter2
{
    partial class LocalConfigDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serverLabel = new System.Windows.Forms.Label();
            this.portText = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.usernameText = new System.Windows.Forms.TextBox();
            this.passwordText = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.enableTimersLabel = new System.Windows.Forms.Label();
            this.enableTimersList = new System.Windows.Forms.ListBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.serverText = new System.Windows.Forms.ComboBox();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.layoutPanel.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverLabel
            // 
            this.serverLabel.BackColor = System.Drawing.Color.Transparent;
            this.serverLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.serverLabel.Location = new System.Drawing.Point(3, 3);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(3);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(88, 20);
            this.serverLabel.TabIndex = 11;
            this.serverLabel.Text = "Server Address:";
            // 
            // portText
            // 
            this.portText.BackColor = System.Drawing.SystemColors.Window;
            this.portText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portText.Location = new System.Drawing.Point(111, 30);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(193, 21);
            this.portText.TabIndex = 1;
            this.portText.Text = "<not set>";
            // 
            // portLabel
            // 
            this.portLabel.BackColor = System.Drawing.Color.Transparent;
            this.portLabel.Location = new System.Drawing.Point(3, 30);
            this.portLabel.Margin = new System.Windows.Forms.Padding(3);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(88, 20);
            this.portLabel.TabIndex = 10;
            this.portLabel.Text = "Server Port:";
            // 
            // passwordLabel
            // 
            this.passwordLabel.BackColor = System.Drawing.Color.Transparent;
            this.passwordLabel.Location = new System.Drawing.Point(3, 84);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(3);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(102, 20);
            this.passwordLabel.TabIndex = 10;
            this.passwordLabel.Text = "Default Password:";
            // 
            // usernameText
            // 
            this.usernameText.BackColor = System.Drawing.SystemColors.Window;
            this.usernameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usernameText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usernameText.Location = new System.Drawing.Point(111, 57);
            this.usernameText.Name = "usernameText";
            this.usernameText.Size = new System.Drawing.Size(193, 21);
            this.usernameText.TabIndex = 2;
            this.usernameText.Text = "<not set>";
            // 
            // passwordText
            // 
            this.passwordText.BackColor = System.Drawing.SystemColors.Window;
            this.passwordText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordText.Location = new System.Drawing.Point(111, 84);
            this.passwordText.Name = "passwordText";
            this.passwordText.Size = new System.Drawing.Size(193, 21);
            this.passwordText.TabIndex = 3;
            this.passwordText.Text = "<not set>";
            // 
            // usernameLabel
            // 
            this.usernameLabel.BackColor = System.Drawing.Color.Transparent;
            this.usernameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.usernameLabel.Location = new System.Drawing.Point(3, 57);
            this.usernameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(102, 20);
            this.usernameLabel.TabIndex = 11;
            this.usernameLabel.Text = "Default Username:";
            // 
            // enableTimersLabel
            // 
            this.enableTimersLabel.BackColor = System.Drawing.Color.Transparent;
            this.enableTimersLabel.Location = new System.Drawing.Point(3, 111);
            this.enableTimersLabel.Margin = new System.Windows.Forms.Padding(3);
            this.enableTimersLabel.Name = "enableTimersLabel";
            this.enableTimersLabel.Size = new System.Drawing.Size(88, 20);
            this.enableTimersLabel.TabIndex = 9;
            this.enableTimersLabel.Text = "Enable Timers:";
            // 
            // enableTimersList
            // 
            this.enableTimersList.BackColor = System.Drawing.SystemColors.Window;
            this.enableTimersList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.enableTimersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enableTimersList.FormattingEnabled = true;
            this.enableTimersList.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.enableTimersList.Location = new System.Drawing.Point(111, 111);
            this.enableTimersList.Name = "enableTimersList";
            this.enableTimersList.Size = new System.Drawing.Size(193, 28);
            this.enableTimersList.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Location = new System.Drawing.Point(3, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 12;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // serverText
            // 
            this.serverText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.serverText.FormattingEnabled = true;
            this.serverText.Items.AddRange(new object[] {
            "localhost",
            "studio1",
            "studio2",
            "production"});
            this.serverText.Location = new System.Drawing.Point(111, 3);
            this.serverText.Name = "serverText";
            this.serverText.Size = new System.Drawing.Size(193, 21);
            this.serverText.TabIndex = 21;
            this.serverText.Text = "<not set>";
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoSize = true;
            this.layoutPanel.ColumnCount = 2;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutPanel.Controls.Add(this.buttonsPanel, 0, 5);
            this.layoutPanel.Controls.Add(this.serverLabel, 0, 0);
            this.layoutPanel.Controls.Add(this.serverText, 1, 0);
            this.layoutPanel.Controls.Add(this.portLabel, 0, 1);
            this.layoutPanel.Controls.Add(this.enableTimersList, 1, 4);
            this.layoutPanel.Controls.Add(this.portText, 1, 1);
            this.layoutPanel.Controls.Add(this.usernameLabel, 0, 2);
            this.layoutPanel.Controls.Add(this.enableTimersLabel, 0, 4);
            this.layoutPanel.Controls.Add(this.passwordText, 1, 3);
            this.layoutPanel.Controls.Add(this.passwordLabel, 0, 3);
            this.layoutPanel.Controls.Add(this.usernameText, 1, 2);
            this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanel.Location = new System.Drawing.Point(5, 5);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 6;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.Size = new System.Drawing.Size(307, 165);
            this.layoutPanel.TabIndex = 22;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Location = new System.Drawing.Point(84, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsPanel.AutoSize = true;
            this.layoutPanel.SetColumnSpan(this.buttonsPanel, 2);
            this.buttonsPanel.Controls.Add(this.cancelButton);
            this.buttonsPanel.Controls.Add(this.saveButton);
            this.buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonsPanel.Location = new System.Drawing.Point(142, 145);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(162, 29);
            this.buttonsPanel.TabIndex = 23;
            // 
            // LocalConfigDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(317, 175);
            this.ControlBox = false;
            this.Controls.Add(this.layoutPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocalConfigDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LocalConfigDialog_KeyDown);
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.buttonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label enableTimersLabel;
    	private ListBox enableTimersList;
    	private Button saveButton;
    	private ComboBox serverText;
    	private Label serverLabel;
    	private TextBox portText;
    	private Label portLabel;
    	private Label passwordLabel;
    	private TextBox usernameText;
    	private TextBox passwordText;
    	private Label usernameLabel;
        private TableLayoutPanel layoutPanel;
        private Button cancelButton;
        private FlowLayoutPanel buttonsPanel;
    }
}