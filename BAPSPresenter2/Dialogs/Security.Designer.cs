namespace BAPSPresenter2.Dialogs
{
    partial class Security
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
            this.addToAllowButton = (new System.Windows.Forms.Button());
            this.ipAddressText = (new System.Windows.Forms.TextBox());
            this.label1 = (new System.Windows.Forms.Label());
            this.deniedIPList = (new System.Windows.Forms.ListBox());
            this.allowedIPList = (new System.Windows.Forms.ListBox());
            this.permissionList = (new System.Windows.Forms.ListBox());
            this.userList = (new System.Windows.Forms.ListBox());
            this.selectedUserLabel = (new System.Windows.Forms.Label());
            this.confirmNewPasswordLabel = (new System.Windows.Forms.Label());
            this.grantButton = (new System.Windows.Forms.Button());
            this.label2 = (new System.Windows.Forms.Label());
            this.maskText = (new System.Windows.Forms.TextBox());
            this.addToDenyButton = (new System.Windows.Forms.Button());
            this.removeFromAllowButton = (new System.Windows.Forms.Button());
            this.connectionManagerPage = (new System.Windows.Forms.TabPage());
            this.removeFromDenyButton = (new System.Windows.Forms.Button());
            this.confirmNewPasswordText = (new System.Windows.Forms.TextBox());
            this.availablePermissionList = (new System.Windows.Forms.ListBox());
            this.availablePermissionLabel = (new System.Windows.Forms.Label());
            this.refreshButton = (new System.Windows.Forms.Button());
            this.deleteUserButton = (new System.Windows.Forms.Button());
            this.userManagerPage = (new System.Windows.Forms.TabPage());
            this.selectedUserLabelLabel = (new System.Windows.Forms.Label());
            this.newUserBox = (new System.Windows.Forms.GroupBox());
            this.addUserButton = (new System.Windows.Forms.Button());
            this.passwordLabel = (new System.Windows.Forms.Label());
            this.newUsernameLabel = (new System.Windows.Forms.Label());
            this.newUsernameText = (new System.Windows.Forms.TextBox());
            this.passwordText = (new System.Windows.Forms.TextBox());
            this.confirmPasswordText = (new System.Windows.Forms.TextBox());
            this.confirmPasswordLabel = (new System.Windows.Forms.Label());
            this.setPasswordButton = (new System.Windows.Forms.Button());
            this.newPasswordLabel = (new System.Windows.Forms.Label());
            this.newPasswordText = (new System.Windows.Forms.TextBox());
            this.permissionLabel = (new System.Windows.Forms.Label());
            this.selectUserLabel = (new System.Windows.Forms.Label());
            this.revokeButton = (new System.Windows.Forms.Button());
            this.securityPageControl = (new System.Windows.Forms.TabControl());
            this.connectionManagerPage.SuspendLayout();
            this.userManagerPage.SuspendLayout();
            this.newUserBox.SuspendLayout();
            this.securityPageControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // addToAllowButton
            // 
            this.addToAllowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addToAllowButton.Location = new System.Drawing.Point(288, 294);
            this.addToAllowButton.Name = "addToAllowButton";
            this.addToAllowButton.Size = new System.Drawing.Size(80, 22);
            this.addToAllowButton.TabIndex = 4;
            this.addToAllowButton.Text = "Add to Allow";
            this.addToAllowButton.Click += new System.EventHandler(this.alterRestrictionButton_Click);
            // 
            // ipAddressText
            // 
            this.ipAddressText.Location = new System.Drawing.Point(16, 294);
            this.ipAddressText.Name = "ipAddressText";
            this.ipAddressText.Size = new System.Drawing.Size(170, 21);
            this.ipAddressText.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Font = (new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                0));
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Allowed Networks";
            // 
            // deniedIPList
            // 
            this.deniedIPList.Location = new System.Drawing.Point(288, 32);
            this.deniedIPList.Name = "deniedIPList";
            this.deniedIPList.Size = new System.Drawing.Size(232, 251);
            this.deniedIPList.TabIndex = 1;
            // 
            // allowedIPList
            // 
            this.allowedIPList.Location = new System.Drawing.Point(16, 32);
            this.allowedIPList.Name = "allowedIPList";
            this.allowedIPList.Size = new System.Drawing.Size(232, 251);
            this.allowedIPList.TabIndex = 0;
            // 
            // permissionList
            // 
            this.permissionList.Location = new System.Drawing.Point(216, 64);
            this.permissionList.Name = "permissionList";
            this.permissionList.Size = new System.Drawing.Size(296, 56);
            this.permissionList.TabIndex = 18;
            this.permissionList.SelectedIndexChanged += new System.EventHandler(this.permissionList_SelectedIndexChanged);
            // 
            // userList
            // 
            this.userList.Location = new System.Drawing.Point(16, 24);
            this.userList.Name = "userList";
            this.userList.Size = new System.Drawing.Size(176, 251);
            this.userList.TabIndex = 6;
            this.userList.SelectedIndexChanged += new System.EventHandler(this.userList_SelectedIndexChanged);
            // 
            // selectedUserLabel
            // 
            this.selectedUserLabel.Font = (new System.Drawing.Font("Segoe UI", 15, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.selectedUserLabel.Location = new System.Drawing.Point(280, 16);
            this.selectedUserLabel.Name = "selectedUserLabel";
            this.selectedUserLabel.Size = new System.Drawing.Size(176, 24);
            this.selectedUserLabel.TabIndex = 11;
            this.selectedUserLabel.Text = "<select user>";
            // 
            // confirmNewPasswordLabel
            // 
            this.confirmNewPasswordLabel.Location = new System.Drawing.Point(307, 237);
            this.confirmNewPasswordLabel.Name = "confirmNewPasswordLabel";
            this.confirmNewPasswordLabel.Size = new System.Drawing.Size(104, 16);
            this.confirmNewPasswordLabel.TabIndex = 16;
            this.confirmNewPasswordLabel.Text = "Confirm Password";
            // 
            // grantButton
            // 
            this.grantButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grantButton.Font = (new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.grantButton.Location = new System.Drawing.Point(448, 150);
            this.grantButton.Name = "grantButton";
            this.grantButton.Size = new System.Drawing.Size(64, 21);
            this.grantButton.TabIndex = 9;
            this.grantButton.Text = "Grant";
            this.grantButton.Click += new System.EventHandler(this.grantButton_Click);
            // 
            // label2
            // 
            this.label2.Font = (new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                0));
            this.label2.Location = new System.Drawing.Point(284, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Denied Networks";
            // 
            // maskText
            // 
            this.maskText.Location = new System.Drawing.Point(192, 294);
            this.maskText.Name = "maskText";
            this.maskText.Size = new System.Drawing.Size(56, 21);
            this.maskText.TabIndex = 3;
            // 
            // addToDenyButton
            // 
            this.addToDenyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addToDenyButton.Location = new System.Drawing.Point(288, 322);
            this.addToDenyButton.Name = "addToDenyButton";
            this.addToDenyButton.Size = new System.Drawing.Size(80, 22);
            this.addToDenyButton.TabIndex = 4;
            this.addToDenyButton.Text = "Add to Deny";
            this.addToDenyButton.Click += new System.EventHandler(this.alterRestrictionButton_Click);
            // 
            // removeFromAllowButton
            // 
            this.removeFromAllowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeFromAllowButton.Location = new System.Drawing.Point(374, 294);
            this.removeFromAllowButton.Name = "removeFromAllowButton";
            this.removeFromAllowButton.Size = new System.Drawing.Size(112, 22);
            this.removeFromAllowButton.TabIndex = 4;
            this.removeFromAllowButton.Text = "Remove from Allow";
            this.removeFromAllowButton.Click += new System.EventHandler(this.alterRestrictionButton_Click);
            // 
            // connectionManagerPage
            // 
            this.connectionManagerPage.Controls.Add(this.addToAllowButton);
            this.connectionManagerPage.Controls.Add(this.ipAddressText);
            this.connectionManagerPage.Controls.Add(this.label1);
            this.connectionManagerPage.Controls.Add(this.deniedIPList);
            this.connectionManagerPage.Controls.Add(this.allowedIPList);
            this.connectionManagerPage.Controls.Add(this.label2);
            this.connectionManagerPage.Controls.Add(this.maskText);
            this.connectionManagerPage.Controls.Add(this.addToDenyButton);
            this.connectionManagerPage.Controls.Add(this.removeFromAllowButton);
            this.connectionManagerPage.Controls.Add(this.removeFromDenyButton);
            this.connectionManagerPage.Location = new System.Drawing.Point(4, 25);
            this.connectionManagerPage.Name = "connectionManagerPage";
            this.connectionManagerPage.Size = new System.Drawing.Size(531, 349);
            this.connectionManagerPage.TabIndex = 1;
            this.connectionManagerPage.Text = "Connection Manager";
            // 
            // removeFromDenyButton
            // 
            this.removeFromDenyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeFromDenyButton.Location = new System.Drawing.Point(374, 322);
            this.removeFromDenyButton.Name = "removeFromDenyButton";
            this.removeFromDenyButton.Size = new System.Drawing.Size(112, 22);
            this.removeFromDenyButton.TabIndex = 4;
            this.removeFromDenyButton.Text = "Remove from Deny";
            this.removeFromDenyButton.Click += new System.EventHandler(this.alterRestrictionButton_Click);
            // 
            // confirmNewPasswordText
            // 
            this.confirmNewPasswordText.Location = new System.Drawing.Point(310, 256);
            this.confirmNewPasswordText.Name = "confirmNewPasswordText";
            this.confirmNewPasswordText.PasswordChar = '*';
            this.confirmNewPasswordText.Size = new System.Drawing.Size(88, 21);
            this.confirmNewPasswordText.TabIndex = 15;
            this.confirmNewPasswordText.Text = "12345";
            this.confirmNewPasswordText.UseSystemPasswordChar = true;
            this.confirmNewPasswordText.TextChanged += new System.EventHandler(this.changePassword_TextChanged);
            // 
            // availablePermissionList
            // 
            this.availablePermissionList.Location = new System.Drawing.Point(216, 177);
            this.availablePermissionList.Name = "availablePermissionList";
            this.availablePermissionList.Size = new System.Drawing.Size(296, 56);
            this.availablePermissionList.TabIndex = 18;
            this.availablePermissionList.SelectedIndexChanged += new System.EventHandler(this.availablePermissionList_SelectedIndexChanged);
            // 
            // availablePermissionLabel
            // 
            this.availablePermissionLabel.Location = new System.Drawing.Point(213, 158);
            this.availablePermissionLabel.Name = "availablePermissionLabel";
            this.availablePermissionLabel.Size = new System.Drawing.Size(136, 16);
            this.availablePermissionLabel.TabIndex = 14;
            this.availablePermissionLabel.Text = "Permissions (available):";
            // 
            // refreshButton
            // 
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.refreshButton.Font = (new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.refreshButton.Location = new System.Drawing.Point(448, 8);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(66, 22);
            this.refreshButton.TabIndex = 21;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // deleteUserButton
            // 
            this.deleteUserButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deleteUserButton.Font = (new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.deleteUserButton.Location = new System.Drawing.Point(448, 36);
            this.deleteUserButton.Name = "deleteUserButton";
            this.deleteUserButton.Size = new System.Drawing.Size(64, 22);
            this.deleteUserButton.TabIndex = 22;
            this.deleteUserButton.Text = "Delete User";
            this.deleteUserButton.Click += new System.EventHandler(this.deleteUserButton_Click);
            // 
            // userManagerPage
            // 
            this.userManagerPage.Controls.Add(this.deleteUserButton);
            this.userManagerPage.Controls.Add(this.refreshButton);
            this.userManagerPage.Controls.Add(this.selectedUserLabelLabel);
            this.userManagerPage.Controls.Add(this.newUserBox);
            this.userManagerPage.Controls.Add(this.setPasswordButton);
            this.userManagerPage.Controls.Add(this.newPasswordLabel);
            this.userManagerPage.Controls.Add(this.newPasswordText);
            this.userManagerPage.Controls.Add(this.permissionLabel);
            this.userManagerPage.Controls.Add(this.selectUserLabel);
            this.userManagerPage.Controls.Add(this.revokeButton);
            this.userManagerPage.Controls.Add(this.grantButton);
            this.userManagerPage.Controls.Add(this.permissionList);
            this.userManagerPage.Controls.Add(this.userList);
            this.userManagerPage.Controls.Add(this.selectedUserLabel);
            this.userManagerPage.Controls.Add(this.confirmNewPasswordLabel);
            this.userManagerPage.Controls.Add(this.confirmNewPasswordText);
            this.userManagerPage.Controls.Add(this.availablePermissionList);
            this.userManagerPage.Controls.Add(this.availablePermissionLabel);
            this.userManagerPage.Location = new System.Drawing.Point(4, 25);
            this.userManagerPage.Name = "userManagerPage";
            this.userManagerPage.Size = new System.Drawing.Size(531, 349);
            this.userManagerPage.TabIndex = 0;
            this.userManagerPage.Text = "User Manager";
            // 
            // selectedUserLabelLabel
            // 
            this.selectedUserLabelLabel.Font = (new System.Drawing.Font("Segoe UI", 15, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.selectedUserLabelLabel.Location = new System.Drawing.Point(216, 16);
            this.selectedUserLabelLabel.Name = "selectedUserLabelLabel";
            this.selectedUserLabelLabel.Size = new System.Drawing.Size(64, 24);
            this.selectedUserLabelLabel.TabIndex = 20;
            this.selectedUserLabelLabel.Text = "User:";
            // 
            // newUserBox
            // 
            this.newUserBox.Controls.Add(this.addUserButton);
            this.newUserBox.Controls.Add(this.passwordLabel);
            this.newUserBox.Controls.Add(this.newUsernameLabel);
            this.newUserBox.Controls.Add(this.newUsernameText);
            this.newUserBox.Controls.Add(this.passwordText);
            this.newUserBox.Controls.Add(this.confirmPasswordText);
            this.newUserBox.Controls.Add(this.confirmPasswordLabel);
            this.newUserBox.Location = new System.Drawing.Point(8, 280);
            this.newUserBox.Name = "newUserBox";
            this.newUserBox.Size = new System.Drawing.Size(520, 64);
            this.newUserBox.TabIndex = 19;
            this.newUserBox.TabStop = false;
            this.newUserBox.Text = "Add User";
            // 
            // addUserButton
            // 
            this.addUserButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addUserButton.Font = (new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.addUserButton.Location = new System.Drawing.Point(416, 27);
            this.addUserButton.Name = "addUserButton";
            this.addUserButton.Size = new System.Drawing.Size(88, 26);
            this.addUserButton.TabIndex = 4;
            this.addUserButton.Text = "Add User";
            this.addUserButton.Click += new System.EventHandler(this.addUserButton_Click);
            // 
            // passwordLabel
            // 
            this.passwordLabel.Location = new System.Drawing.Point(176, 16);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(88, 16);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "Password";
            // 
            // newUsernameLabel
            // 
            this.newUsernameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.newUsernameLabel.Location = new System.Drawing.Point(40, 16);
            this.newUsernameLabel.Name = "newUsernameLabel";
            this.newUsernameLabel.Size = new System.Drawing.Size(112, 16);
            this.newUsernameLabel.TabIndex = 1;
            this.newUsernameLabel.Text = "Username";
            // 
            // newUsernameText
            // 
            this.newUsernameText.Location = new System.Drawing.Point(40, 32);
            this.newUsernameText.Name = "newUsernameText";
            this.newUsernameText.Size = new System.Drawing.Size(112, 21);
            this.newUsernameText.TabIndex = 0;
            this.newUsernameText.TextChanged += new System.EventHandler(this.newUserText_Leave);
            // 
            // passwordText
            // 
            this.passwordText.Location = new System.Drawing.Point(176, 32);
            this.passwordText.Name = "passwordText";
            this.passwordText.PasswordChar = '*';
            this.passwordText.Size = new System.Drawing.Size(88, 21);
            this.passwordText.TabIndex = 0;
            this.passwordText.UseSystemPasswordChar = true;
            this.passwordText.TextChanged += new System.EventHandler(this.newUserText_Leave);
            // 
            // confirmPasswordText
            // 
            this.confirmPasswordText.Location = new System.Drawing.Point(272, 32);
            this.confirmPasswordText.Name = "confirmPasswordText";
            this.confirmPasswordText.PasswordChar = '*';
            this.confirmPasswordText.Size = new System.Drawing.Size(88, 21);
            this.confirmPasswordText.TabIndex = 0;
            this.confirmPasswordText.UseSystemPasswordChar = true;
            this.confirmPasswordText.TextChanged += new System.EventHandler(this.newUserText_Leave);
            // 
            // confirmPasswordLabel
            // 
            this.confirmPasswordLabel.Location = new System.Drawing.Point(272, 16);
            this.confirmPasswordLabel.Name = "confirmPasswordLabel";
            this.confirmPasswordLabel.Size = new System.Drawing.Size(104, 16);
            this.confirmPasswordLabel.TabIndex = 2;
            this.confirmPasswordLabel.Text = "Confirm Password";
            // 
            // setPasswordButton
            // 
            this.setPasswordButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.setPasswordButton.Font = (new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.setPasswordButton.Location = new System.Drawing.Point(424, 256);
            this.setPasswordButton.Name = "setPasswordButton";
            this.setPasswordButton.Size = new System.Drawing.Size(88, 21);
            this.setPasswordButton.TabIndex = 17;
            this.setPasswordButton.Text = "Set Password";
            this.setPasswordButton.Click += new System.EventHandler(this.setPasswordButton_Click);
            // 
            // newPasswordLabel
            // 
            this.newPasswordLabel.Location = new System.Drawing.Point(213, 237);
            this.newPasswordLabel.Name = "newPasswordLabel";
            this.newPasswordLabel.Size = new System.Drawing.Size(88, 16);
            this.newPasswordLabel.TabIndex = 16;
            this.newPasswordLabel.Text = "New Password";
            // 
            // newPasswordText
            // 
            this.newPasswordText.Location = new System.Drawing.Point(216, 256);
            this.newPasswordText.Name = "newPasswordText";
            this.newPasswordText.PasswordChar = '*';
            this.newPasswordText.Size = new System.Drawing.Size(88, 21);
            this.newPasswordText.TabIndex = 15;
            this.newPasswordText.Text = "12345";
            this.newPasswordText.UseSystemPasswordChar = true;
            this.newPasswordText.TextChanged += new System.EventHandler(this.changePassword_TextChanged);
            // 
            // permissionLabel
            // 
            this.permissionLabel.Location = new System.Drawing.Point(213, 45);
            this.permissionLabel.Name = "permissionLabel";
            this.permissionLabel.Size = new System.Drawing.Size(136, 16);
            this.permissionLabel.TabIndex = 14;
            this.permissionLabel.Text = "Permissions (granted):";
            // 
            // selectUserLabel
            // 
            this.selectUserLabel.Font = (new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.selectUserLabel.Location = new System.Drawing.Point(16, 8);
            this.selectUserLabel.Name = "selectUserLabel";
            this.selectUserLabel.Size = new System.Drawing.Size(168, 16);
            this.selectUserLabel.TabIndex = 13;
            this.selectUserLabel.Text = "Select User:";
            // 
            // revokeButton
            // 
            this.revokeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.revokeButton.Font = (new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.revokeButton.Location = new System.Drawing.Point(448, 126);
            this.revokeButton.Name = "revokeButton";
            this.revokeButton.Size = new System.Drawing.Size(64, 21);
            this.revokeButton.TabIndex = 10;
            this.revokeButton.Text = "Revoke";
            this.revokeButton.Click += new System.EventHandler(this.revokeButton_Click);
            // 
            // securityPageControl
            // 
            this.securityPageControl.Appearance = System.Windows.Forms.TabAppearance.Normal;
            this.securityPageControl.Controls.Add(this.userManagerPage);
            this.securityPageControl.Controls.Add(this.connectionManagerPage);
            this.securityPageControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.securityPageControl.Font = (new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                0));
            this.securityPageControl.Location = new System.Drawing.Point(0, 0);
            this.securityPageControl.Name = "securityPageControl";
            this.securityPageControl.SelectedIndex = 0;
            this.securityPageControl.Size = new System.Drawing.Size(539, 378);
            this.securityPageControl.TabIndex = 1;
            this.securityPageControl.SelectedIndexChanged += new System.EventHandler(this.securityPageControl_SelectedIndexChanged);
            // 
            // SecurityDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 378);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.securityPageControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SecurityDialog";
            this.Text = "Security Manager";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SecurityDialog_KeyDown);
            this.connectionManagerPage.ResumeLayout(false);
            this.connectionManagerPage.PerformLayout();
            this.userManagerPage.ResumeLayout(false);
            this.userManagerPage.PerformLayout();
            this.newUserBox.ResumeLayout(false);
            this.newUserBox.PerformLayout();
            this.securityPageControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button addToAllowButton;
	    private System.Windows.Forms.TextBox ipAddressText;
	    private System.Windows.Forms.Label label1;
	    private System.Windows.Forms.ListBox deniedIPList;
	    private System.Windows.Forms.ListBox allowedIPList;
	    private System.Windows.Forms.ListBox permissionList;
	    private System.Windows.Forms.ListBox userList;
	    private System.Windows.Forms.Label selectedUserLabel;
	    private System.Windows.Forms.Label confirmNewPasswordLabel;
	    private System.Windows.Forms.Button grantButton;
	    private System.Windows.Forms.Label label2;
	    private System.Windows.Forms.TextBox maskText;
	    private System.Windows.Forms.Button addToDenyButton;
	    private System.Windows.Forms.Button removeFromAllowButton;
	    private System.Windows.Forms.TabPage connectionManagerPage;
	    private System.Windows.Forms.Button removeFromDenyButton;
	    private System.Windows.Forms.TextBox confirmNewPasswordText;
	    private System.Windows.Forms.ListBox availablePermissionList;
	    private System.Windows.Forms.Label availablePermissionLabel;
	    private System.Windows.Forms.Button refreshButton;
	    private System.Windows.Forms.Button deleteUserButton;
	    private System.Windows.Forms.TabPage userManagerPage;
	    private System.Windows.Forms.Label selectedUserLabelLabel;
	    private System.Windows.Forms.GroupBox newUserBox;
	    private System.Windows.Forms.Button addUserButton;
	    private System.Windows.Forms.Label passwordLabel;
	    private System.Windows.Forms.Label newUsernameLabel;
	    private System.Windows.Forms.TextBox newUsernameText;
	    private System.Windows.Forms.TextBox passwordText;
	    private System.Windows.Forms.TextBox confirmPasswordText;
	    private System.Windows.Forms.Label confirmPasswordLabel;
	    private System.Windows.Forms.Button setPasswordButton;
	    private System.Windows.Forms.Label newPasswordLabel;
	    private System.Windows.Forms.TextBox newPasswordText;
	    private System.Windows.Forms.Label permissionLabel;
	    private System.Windows.Forms.Label selectUserLabel;
	    private System.Windows.Forms.Button revokeButton;
	    private System.Windows.Forms.TabControl securityPageControl;
    }
}