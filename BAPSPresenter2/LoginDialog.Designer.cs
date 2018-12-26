using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

namespace BAPSPresenter2
{
    /**
        The LoginDialog is the entry point of the Client Application and deals with
        obtaining server connection details and login details from the client
    **/
    partial class LoginDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Label passwordLabel;
        private TextBox passwordText;
        private TextBox usernameText;
        private Label usernameLabel;

        private Label portLabel;
        private Label label1;
        private TextBox portText;

        private HelpProvider helpProvider1;
        private Button loginButton;
        private Button cancelButton;
        private ComboBox serverText;

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

        void loginButton_Click(object sender, System.EventArgs e)
        {
            /** Blank usernames are not permitted **/
            if (usernameText.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("You must enter a username", "Incomplete login:", System.Windows.Forms.MessageBoxButtons.OK);
            }
            else
            {
                if (serverText.Text.Equals("studio1")) serverText.Text = "144.32.64.181";
                else if (serverText.Text.Equals("studio2")) serverText.Text = "144.32.64.184";
                else if (serverText.Text.Equals("production")) serverText.Text = "144.32.64.178";
                else if (serverText.Text.Equals("localhost")) serverText.Text = "127.0.0.1";

                /** Increment the attempt counter so the first attempt is the only
                    one that causes a connection to the server to be made, (unless
                    the server details are altered
                **/
                loginAttempt++;
                /** Set OK as the result as cancel will terminate the client **/
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }
        void cancelButton_Click(object sender, System.EventArgs e)
        {
            /** This will in effect end the application **/
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
        void LoginDialog_Load(object sender, System.EventArgs e)
        {
            /** On showing the form we reset the flag so that we dont reconnect next attempt **/
            connectionChanged = false;
            serverText.Focus();
        }
        void connectionText_TextChanged(object sender, System.EventArgs e)
        {
            /** If the server or port change then the connection must be dropped the the current server **/
            connectionChanged = true;
            /** Validate the port number **/
            try
            {
                int port = Port;
                if (port < 1 || port > 65535)
                {
                    throw new System.Exception("Port out of range");
                }
            }
            catch (System.Exception)
            {
                System.Windows.Forms.MessageBox.Show("You must enter a number (1-65535).", "Port error:", System.Windows.Forms.MessageBoxButtons.OK);
                portText.Text = "1350";
            }
        }
        void Textbox_Enter(object sender, System.EventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.passwordLabel = (new System.Windows.Forms.Label());
            this.passwordText = (new System.Windows.Forms.TextBox());
            this.usernameText = (new System.Windows.Forms.TextBox());
            this.usernameLabel = (new System.Windows.Forms.Label());
            this.portLabel = (new System.Windows.Forms.Label());
            this.label1 = (new System.Windows.Forms.Label());
            this.portText = (new System.Windows.Forms.TextBox());
            this.helpProvider1 = (new System.Windows.Forms.HelpProvider());
            this.loginButton = (new System.Windows.Forms.Button());
            this.cancelButton = (new System.Windows.Forms.Button());
            this.serverText = (new System.Windows.Forms.ComboBox());
            this.SuspendLayout();
            // 
            // passwordLabel
            // 
            this.passwordLabel.BackColor = System.Drawing.Color.Transparent;
            this.passwordLabel.Location = new System.Drawing.Point(12, 40);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(64, 16);
            this.passwordLabel.TabIndex = 16;
            this.passwordLabel.Text = "Password:";
            // 
            // passwordText
            // 
            this.passwordText.BackColor = System.Drawing.SystemColors.Window;
            this.passwordText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordText.Location = new System.Drawing.Point(82, 38);
            this.passwordText.Name = "passwordText";
            this.passwordText.PasswordChar = '*';
            this.passwordText.Size = new System.Drawing.Size(125, 21);
            this.passwordText.TabIndex = 1;
            this.passwordText.UseSystemPasswordChar = true;
            this.passwordText.Enter += connectionText_TextChanged;
            // 
            // usernameText
            // 
            this.usernameText.BackColor = System.Drawing.SystemColors.Window;
            this.usernameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usernameText.Location = new System.Drawing.Point(82, 12);
            this.usernameText.Name = "usernameText";
            this.usernameText.Size = new System.Drawing.Size(125, 21);
            this.usernameText.TabIndex = 0;
            this.usernameText.Enter += Textbox_Enter;
            // 
            // usernameLabel
            // 
            this.usernameLabel.BackColor = System.Drawing.Color.Transparent;
            this.usernameLabel.Location = new System.Drawing.Point(12, 14);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(64, 16);
            this.usernameLabel.TabIndex = 17;
            this.usernameLabel.Text = "Username:";
            // 
            // portLabel
            // 
            this.portLabel.BackColor = System.Drawing.Color.Transparent;
            this.portLabel.Location = new System.Drawing.Point(12, 92);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(64, 18);
            this.portLabel.TabIndex = 13;
            this.portLabel.Text = "Port:";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Server:";
            // 
            // portText
            // 
            this.portText.BackColor = System.Drawing.SystemColors.Window;
            this.portText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portText.Location = new System.Drawing.Point(82, 90);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(125, 21);
            this.portText.TabIndex = 7;
            this.portText.Text = "1350";
            this.portText.TextChanged += connectionText_TextChanged;
            this.portText.Enter += Textbox_Enter;
            // 
            // loginButton
            // 
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Location = new System.Drawing.Point(51, 117);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 23);
            this.loginButton.TabIndex = 18;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += loginButton_Click;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Location = new System.Drawing.Point(132, 117);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += cancelButton_Click;
            // 
            // serverText
            // 
            this.serverText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.serverText.FormattingEnabled = true;
            this.serverText.Items.AddRange(new object[] { "localhost", "studio1", "studio2", "production" });
            this.serverText.Location = new System.Drawing.Point(82, 63);
            this.serverText.Name = "serverText";
            this.serverText.Size = new System.Drawing.Size(125, 21);
            this.serverText.TabIndex = 20;
            // 
            // LoginDialog
            // 
            this.AcceptButton = this.loginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(219, 152);
            this.ControlBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.serverText);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.passwordText);
            this.Controls.Add(this.usernameText);
            this.Controls.Add(this.usernameLabel);
            this.Font = (new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = Properties.Resources.Icon;
            this.Name = "LoginDialog";
            this.Text = "BAPS Presenter: Please Login";
            this.Load += LoginDialog_Load;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}