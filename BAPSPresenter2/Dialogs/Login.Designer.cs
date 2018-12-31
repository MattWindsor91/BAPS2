using System.Windows.Forms;

namespace BAPSPresenter2.Dialogs
{
    /**
        The LoginDialog is the entry point of the Client Application and deals with
        obtaining server connection details and login details from the client
    **/
    partial class Login
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
                PortText = Properties.Resources.DefaultPort;
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
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordText = new System.Windows.Forms.TextBox();
            this.usernameText = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.portText = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.loginButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.serverText = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // passwordLabel
            // 
            this.passwordLabel.BackColor = System.Drawing.Color.Transparent;
            this.passwordLabel.Location = new System.Drawing.Point(3, 30);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(3);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(64, 16);
            this.passwordLabel.TabIndex = 16;
            this.passwordLabel.Text = "Password:";
            // 
            // passwordText
            // 
            this.passwordText.BackColor = System.Drawing.SystemColors.Window;
            this.passwordText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordText.Location = new System.Drawing.Point(73, 30);
            this.passwordText.Name = "passwordText";
            this.passwordText.PasswordChar = '*';
            this.passwordText.Size = new System.Drawing.Size(133, 21);
            this.passwordText.TabIndex = 1;
            this.passwordText.UseSystemPasswordChar = true;
            this.passwordText.Enter += new System.EventHandler(this.Textbox_Enter);
            // 
            // usernameText
            // 
            this.usernameText.BackColor = System.Drawing.SystemColors.Window;
            this.usernameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usernameText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usernameText.Location = new System.Drawing.Point(73, 3);
            this.usernameText.Name = "usernameText";
            this.usernameText.Size = new System.Drawing.Size(133, 21);
            this.usernameText.TabIndex = 0;
            this.usernameText.Enter += new System.EventHandler(this.Textbox_Enter);
            // 
            // usernameLabel
            // 
            this.usernameLabel.BackColor = System.Drawing.Color.Transparent;
            this.usernameLabel.Location = new System.Drawing.Point(3, 3);
            this.usernameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(64, 16);
            this.usernameLabel.TabIndex = 17;
            this.usernameLabel.Text = "Username:";
            // 
            // portLabel
            // 
            this.portLabel.BackColor = System.Drawing.Color.Transparent;
            this.portLabel.Location = new System.Drawing.Point(3, 84);
            this.portLabel.Margin = new System.Windows.Forms.Padding(3);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(64, 18);
            this.portLabel.TabIndex = 13;
            this.portLabel.Text = "Port:";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(3, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Server:";
            // 
            // portText
            // 
            this.portText.BackColor = System.Drawing.SystemColors.Window;
            this.portText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portText.Location = new System.Drawing.Point(73, 84);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(133, 21);
            this.portText.TabIndex = 7;
            this.portText.TextChanged += new System.EventHandler(this.connectionText_TextChanged);
            this.portText.Enter += new System.EventHandler(this.Textbox_Enter);
            // 
            // loginButton
            // 
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Location = new System.Drawing.Point(84, 3);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 23);
            this.loginButton.TabIndex = 18;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Location = new System.Drawing.Point(3, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
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
            this.serverText.Location = new System.Drawing.Point(73, 57);
            this.serverText.Name = "serverText";
            this.serverText.Size = new System.Drawing.Size(133, 21);
            this.serverText.TabIndex = 20;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.usernameText, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.passwordText, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.serverText, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.portLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.portText, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(209, 133);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.loginButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(44, 111);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 29);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // LoginDialog
            // 
            this.AcceptButton = this.loginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(219, 143);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BAPS Presenter: Please Login";
            this.Load += new System.EventHandler(this.LoginDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}