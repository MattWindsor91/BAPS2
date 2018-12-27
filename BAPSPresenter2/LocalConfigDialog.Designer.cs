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
            this.label1 = new System.Windows.Forms.Label();
            this.portText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.usernameText = new System.Windows.Forms.TextBox();
            this.passwordText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.enableTimersLabel = new System.Windows.Forms.Label();
            this.enableTimersList = new System.Windows.Forms.ListBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.serverText = new System.Windows.Forms.ComboBox();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.layoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "Server Address:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // portText
            // 
            this.portText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.portText.BackColor = System.Drawing.SystemColors.Window;
            this.portText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layoutPanel.SetColumnSpan(this.portText, 2);
            this.portText.Location = new System.Drawing.Point(178, 30);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(125, 21);
            this.portText.TabIndex = 1;
            this.portText.Text = "<not set>";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(3, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Server Port:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(3, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Default Password:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // usernameText
            // 
            this.usernameText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.usernameText.BackColor = System.Drawing.SystemColors.Window;
            this.usernameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layoutPanel.SetColumnSpan(this.usernameText, 2);
            this.usernameText.Location = new System.Drawing.Point(178, 57);
            this.usernameText.Name = "usernameText";
            this.usernameText.Size = new System.Drawing.Size(125, 21);
            this.usernameText.TabIndex = 2;
            this.usernameText.Text = "<not set>";
            // 
            // passwordText
            // 
            this.passwordText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.passwordText.BackColor = System.Drawing.SystemColors.Window;
            this.passwordText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layoutPanel.SetColumnSpan(this.passwordText, 2);
            this.passwordText.Location = new System.Drawing.Point(178, 84);
            this.passwordText.Name = "passwordText";
            this.passwordText.Size = new System.Drawing.Size(125, 21);
            this.passwordText.TabIndex = 3;
            this.passwordText.Text = "<not set>";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(3, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Default Username:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // enableTimersLabel
            // 
            this.enableTimersLabel.BackColor = System.Drawing.Color.Transparent;
            this.enableTimersLabel.Location = new System.Drawing.Point(3, 108);
            this.enableTimersLabel.Name = "enableTimersLabel";
            this.enableTimersLabel.Size = new System.Drawing.Size(88, 20);
            this.enableTimersLabel.TabIndex = 9;
            this.enableTimersLabel.Text = "Enable Timers:";
            this.enableTimersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // enableTimersList
            // 
            this.enableTimersList.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.enableTimersList.BackColor = System.Drawing.SystemColors.Window;
            this.enableTimersList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layoutPanel.SetColumnSpan(this.enableTimersList, 2);
            this.enableTimersList.FormattingEnabled = true;
            this.enableTimersList.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.enableTimersList.Location = new System.Drawing.Point(178, 111);
            this.enableTimersList.Name = "enableTimersList";
            this.enableTimersList.Size = new System.Drawing.Size(125, 28);
            this.enableTimersList.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Location = new System.Drawing.Point(141, 155);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 12;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Location = new System.Drawing.Point(228, 155);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // serverText
            // 
            this.serverText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.layoutPanel.SetColumnSpan(this.serverText, 2);
            this.serverText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.serverText.FormattingEnabled = true;
            this.serverText.Items.AddRange(new object[] {
            "localhost",
            "studio1",
            "studio2",
            "production"});
            this.serverText.Location = new System.Drawing.Point(178, 3);
            this.serverText.Name = "serverText";
            this.serverText.Size = new System.Drawing.Size(125, 21);
            this.serverText.TabIndex = 21;
            this.serverText.Text = "<not set>";
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoSize = true;
            this.layoutPanel.ColumnCount = 3;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutPanel.Controls.Add(this.label1, 0, 0);
            this.layoutPanel.Controls.Add(this.cancelButton, 2, 5);
            this.layoutPanel.Controls.Add(this.serverText, 1, 0);
            this.layoutPanel.Controls.Add(this.saveButton, 1, 5);
            this.layoutPanel.Controls.Add(this.label2, 0, 1);
            this.layoutPanel.Controls.Add(this.enableTimersList, 1, 4);
            this.layoutPanel.Controls.Add(this.portText, 1, 1);
            this.layoutPanel.Controls.Add(this.label5, 0, 2);
            this.layoutPanel.Controls.Add(this.enableTimersLabel, 0, 4);
            this.layoutPanel.Controls.Add(this.passwordText, 1, 3);
            this.layoutPanel.Controls.Add(this.label4, 0, 3);
            this.layoutPanel.Controls.Add(this.usernameText, 1, 2);
            this.layoutPanel.Location = new System.Drawing.Point(9, 1);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 6;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutPanel.Size = new System.Drawing.Size(306, 181);
            this.layoutPanel.TabIndex = 22;
            // 
            // LocalConfigDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(317, 182);
            this.ControlBox = false;
            this.Controls.Add(this.layoutPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocalConfigDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LocalConfigDialog_KeyDown);
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label enableTimersLabel;
    	private ListBox enableTimersList;
    	private Button saveButton;
    	private Button cancelButton;
    	private ComboBox serverText;
    	private Label label1;
    	private TextBox portText;
    	private Label label2;
    	private Label label4;
    	private TextBox usernameText;
    	private TextBox passwordText;
    	private Label label5;
        private TableLayoutPanel layoutPanel;
    }
}