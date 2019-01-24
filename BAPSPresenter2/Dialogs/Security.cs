using BAPSCommon;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BAPSPresenter2.Dialogs
{
    public partial class Security : Form
    {
        private class PermissionInfo
        {
            public uint permissionCode;
            public string description;
            public override string ToString() => description;
        }

        private class UserInfo
        {
            public string username;
            public uint permissions;
            public override string ToString() => username;
        }

        private class IPRestriction
        {
            public string ipaddress;
            public uint mask;
            public override string ToString() => string.Concat(ipaddress, "/", mask.ToString());
        }

        /// <summary>
        /// Forwards key-down events that aren't handled by this dialog.
        /// </summary>
        public event KeyEventHandler KeyDownForward;

        public Security(System.Collections.Concurrent.BlockingCollection<BAPSCommon.Message> msgQueue)
        {
            this.msgQueue = msgQueue;

            InitializeComponent();

            userList.Enabled = false;
            permissionList.Enabled = false;
            revokeButton.Enabled = false;
            grantButton.Enabled = false;
            availablePermissionList.Enabled = false;
            newPasswordText.Enabled = false;
            confirmNewPasswordText.Enabled = false;
            setPasswordButton.Enabled = false;
            addUserButton.Enabled = false;
            deleteUserButton.Enabled = false;
            addToAllowButton.Tag = 0;
            addToDenyButton.Tag = Command.ConfigUseValueMask;
            removeFromAllowButton.Tag = Command.ConfigModeMask;
            removeFromDenyButton.Tag = Command.ConfigUseValueMask | Command.ConfigModeMask;
        }

        private enum SecurityStatus
        {
            AWAITING_INIT = 0,
            DORMANT,
            ADDUSER,
            GETUSERS,
            REVOKEPERMISSION,
            GRANTPERMISSION,
            SETPASSWORD,
            REMOVEUSER,
            GETIPRESTRICTIONS,
            GETIPRESTRICTIONS2,
            ALTERIPRESTRICTION
        };

        public void receiveUserCount(uint count)
        {
            grantButton.Enabled = false;
            revokeButton.Enabled = false;
            availablePermissionList.Enabled = false;
            permissionList.Enabled = false;
            newPasswordText.Enabled = false;
            newPasswordText.Text = "12345";
            confirmNewPasswordText.Enabled = false;
            confirmNewPasswordText.Text = "12345";
            setPasswordButton.Enabled = false;
            deleteUserButton.Enabled = false;
            selectedUserLabel.Text = "<Select User>";
            permissionList.Items.Clear();
            availablePermissionList.Items.Clear();

            userList.Enabled = false;
            userList.Items.Clear();
            userCount = count;
            if (userCount == 0)
            {
                status = SecurityStatus.DORMANT;
                userList.Enabled = true;
            }
        }

        public void receiveUserInfo(string username, uint permissions)
        {
            userList.Items.Add(new UserInfo { username = username, permissions = permissions });
            userCount--;
            if (userCount == 0)
            {
                status = SecurityStatus.DORMANT;
                userList.Enabled = true;
            }
        }

        public void receivePermissionCount(uint count)
        {
            permissionCount = count;
            permissionInfo = new PermissionInfo[permissionCount];
        }

        public void receivePermissionInfo(uint permissionCode, string description)
        {
            permissionInfo[permissionInfo.Length - permissionCount] = new PermissionInfo { permissionCode = permissionCode, description = description };
            permissionCount--;

            if (permissionCount == 0)
            {
                status = SecurityStatus.GETUSERS;
                var cmd = Command.Config | Command.GetUsers;
                msgQueue.Add(new BAPSCommon.Message(cmd));
            }
        }

        public void receiveUserResult(uint resultCode, string description)
        {
            var tempStatus = SecurityStatus.DORMANT;
            switch (status)
            {
                case SecurityStatus.AWAITING_INIT:
                case SecurityStatus.GETUSERS:
                    MessageBox.Show(string.Concat("Cannot open Security Manager, reason: ", description), "Error:", MessageBoxButtons.OK);
                    Close();
                    break;
                case SecurityStatus.DORMANT:
                    MessageBox.Show("A recoverable error has occurred, this dialog will be closed. (Result rcvd but no cmd issued)", "Error:", MessageBoxButtons.OK);
                    Close();
                    break;
                case SecurityStatus.ADDUSER:
                    if (resultCode == 0)
                    {
                        newUsernameText.Text = "";
                        passwordText.Text = "";
                        confirmPasswordText.Text = "";
                        addUserButton.Enabled = true;
                        tempStatus = SecurityStatus.GETUSERS;
                        status = SecurityStatus.GETUSERS;
                        var cmd = Command.Config | Command.GetUsers;
                        msgQueue.Add(new BAPSCommon.Message(cmd));
                    }
                    else
                    {
                MessageBox.Show(string.Concat("Cannot add user, reason: ", description), "Error:", MessageBoxButtons.OK);
                    }
                    break;
                case SecurityStatus.SETPASSWORD:
                    if (resultCode == 0)
                    {
                        newPasswordText.Text = "12345";
                        confirmNewPasswordText.Text = "12345";
                        setPasswordButton.Enabled = true;
                    }
                    else
                    {
                MessageBox.Show(string.Concat("Cannot set password, reason: ", description), "Error:", MessageBoxButtons.OK);
                    }
                    break;
                case SecurityStatus.REMOVEUSER:
                    if (resultCode == 0)
                    {
                        tempStatus = SecurityStatus.GETUSERS;
                        status = SecurityStatus.GETUSERS;
                        Command cmd = Command.Config | Command.GetUsers;
                        msgQueue.Add(new BAPSCommon.Message(cmd));
                    }
                    else
                    {
                MessageBox.Show(string.Concat("Cannot delete user, reason: ", description), "Error:", MessageBoxButtons.OK);
                    }
                    break;
                case SecurityStatus.REVOKEPERMISSION:
                    if (resultCode == 0)
                    {
                        tempStatus = SecurityStatus.GETUSERS;
                        status = SecurityStatus.GETUSERS;
                        Command cmd = Command.Config | Command.GetUsers;
                        msgQueue.Add(new BAPSCommon.Message(cmd));
                    }
                    else
                    {
                MessageBox.Show(string.Concat("Cannot revoke permission, reason: ", description), "Error:", MessageBoxButtons.OK);
                    }
                    break;
                case SecurityStatus.GRANTPERMISSION:
                    if (resultCode == 0)
                    {
                        tempStatus = SecurityStatus.GETUSERS;
                        status = SecurityStatus.GETUSERS;
                        Command cmd = Command.Config | Command.GetUsers;
                        msgQueue.Add(new BAPSCommon.Message(cmd));
                    }
                    else
                    {
                MessageBox.Show(string.Concat("Cannot grant permission, reason: ", description), "Error:", MessageBoxButtons.OK);
                    }
                    break;
            }
            status = tempStatus;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                addUserButton.Enabled = false;
                status = SecurityStatus.GETUSERS;
                Command cmd = Command.Config | Command.GetUsers;
                msgQueue.Add(new BAPSCommon.Message(cmd));
            }
            else
            {
                MessageBox.Show("A command is still being processed please wait and try again", "Notice:", MessageBoxButtons.OK);
            }
        }

        private void deleteUserButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                DialogResult dr;
                dr = MessageBox.Show("Are you sure you wish to delete the selcted user?", "Notice:", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    status = SecurityStatus.REMOVEUSER;
                    Command cmd = Command.Config | Command.RemoveUser;
                    msgQueue.Add(new BAPSCommon.Message(cmd).Add(selectedUserLabel.Text));
                }
            }
            else
            {
                MessageBox.Show("A command is still being processed please wait and try again", "Notice:", MessageBoxButtons.OK);
            }
        }
        private void newUserText_Leave(object sender, EventArgs e)
        {
            bool valid = (newUsernameText.Text.Length > 0 &&
                          string.Compare(passwordText.Text, confirmPasswordText.Text) == 0);
            addUserButton.Enabled = valid;
            if (!valid)
            {
                newUsernameLabel.ForeColor = Color.Red;
                confirmPasswordLabel.ForeColor = Color.Red;
            }
            else
            {
                newUsernameLabel.ForeColor = SystemColors.ControlText;
                confirmPasswordLabel.ForeColor = SystemColors.ControlText;
            }
        }
        void changePassword_TextChanged(object sender, EventArgs e)
        {
            bool valid = (string.Compare(newPasswordText.Text, confirmNewPasswordText.Text) == 0);
            setPasswordButton.Enabled = valid;
            if (!valid)
            {
                confirmNewPasswordLabel.ForeColor = Color.Red;
            }
            else
            {
                confirmNewPasswordLabel.ForeColor = SystemColors.ControlText;
            }
        }
        void addUserButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                addUserButton.Enabled = false;
                status = SecurityStatus.ADDUSER;
                Command cmd = Command.Config | Command.AddUser;
                msgQueue.Add(new BAPSCommon.Message(cmd).Add(newUsernameText.Text).Add(passwordText.Text));
            }
            else
            {
        MessageBox.Show("A command is still being processed please wait and try again", "Notice:", MessageBoxButtons.OK);
            }
        }
        void setPasswordButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                setPasswordButton.Enabled = false;
                status = SecurityStatus.SETPASSWORD;
                Command cmd = Command.Config | Command.SetPassword;
                msgQueue.Add(new BAPSCommon.Message(cmd).Add(selectedUserLabel.Text).Add(newPasswordText.Text));
            }
            else
            {
                MessageBox.Show("A command is still being processed please wait and try again", "Notice:", MessageBoxButtons.OK);
            }
        }

        void userList_SelectedIndexChanged(object sender, EventArgs e)
        {
            grantButton.Enabled = false;
            revokeButton.Enabled = false;
            availablePermissionList.Enabled = false;
            permissionList.Enabled = false;
            newPasswordText.Enabled = false;
            newPasswordText.Text = "12345";
            confirmNewPasswordText.Enabled = false;
            confirmNewPasswordText.Text = "12345";
            setPasswordButton.Enabled = false;
            deleteUserButton.Enabled = false;

            UserInfo userInfo;
            if (userList.SelectedIndex != -1)
            {
                userInfo = (UserInfo)(userList.Items[userList.SelectedIndex]);
            }
            else
            {
                userInfo = new UserInfo { username = "<None>", permissions = 0 };
            }
            selectedUserLabel.Text = userInfo.username;
            var permissions = userInfo.permissions;
            permissionList.Items.Clear();
            availablePermissionList.Items.Clear();
            foreach (var perm in permissionInfo)
            {
                if ((permissions & perm.permissionCode) == perm.permissionCode)
                {
                    permissionList.Items.Add(perm);
                }
                else
                {
                    availablePermissionList.Items.Add(perm);
                }
            }
            availablePermissionList.Enabled = true;
            permissionList.Enabled = true;
            newPasswordText.Enabled = true;
            confirmNewPasswordText.Enabled = true;
            deleteUserButton.Enabled = true;
        }

        void permissionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            revokeButton.Enabled = (permissionList.SelectedIndex != -1);
        }
        void availablePermissionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            grantButton.Enabled = (availablePermissionList.SelectedIndex != -1);
        }
        void revokeButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                status = SecurityStatus.REVOKEPERMISSION;
                var permission = ((PermissionInfo) permissionList.Items[permissionList.SelectedIndex]).permissionCode;
                Command cmd = Command.Config | Command.RevokePermission;
                msgQueue.Add(new BAPSCommon.Message(cmd).Add(selectedUserLabel.Text).Add(permission));
            }
            else
            {
                MessageBox.Show("A command is still being processed please wait and try again", "Notice:", MessageBoxButtons.OK);
            }
        }
        void grantButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                status = SecurityStatus.GRANTPERMISSION;
                var permission = ((PermissionInfo) availablePermissionList.Items[availablePermissionList.SelectedIndex]).permissionCode;
                Command cmd = Command.Config | Command.GrantPermission;
                msgQueue.Add(new BAPSCommon.Message(cmd).Add(selectedUserLabel.Text).Add(permission));
            }
            else
            {
                MessageBox.Show("A command is still being processed please wait and try again", "Notice:", MessageBoxButtons.OK);
            }
        }

        public void receiveIPDenyCount(uint denyCount)
        {
            deniedIPList.Enabled = false;
            deniedIPList.Items.Clear();
            if (denyCount == 0)
            {
                if (status == SecurityStatus.GETIPRESTRICTIONS)
                {
                    status = SecurityStatus.GETIPRESTRICTIONS2;
                }
                else
                {
                    status = SecurityStatus.DORMANT;
                }
                deniedIPList.Enabled = true;
            }
        }

        public void receiveIPAllowCount(uint allowCount)
        {
            allowedIPList.Enabled = false;
            allowedIPList.Items.Clear();
            if (allowCount == 0)
            {
                if (status == SecurityStatus.GETIPRESTRICTIONS)
                {
                    status = SecurityStatus.GETIPRESTRICTIONS2;
                }
                else
                {
                    status = SecurityStatus.DORMANT;
                }
                allowedIPList.Enabled = true;
            }
        }

        public void receiveIPDeny(string ipaddress, uint mask)
        {
            deniedIPList.Items.Add(new IPRestriction { ipaddress = ipaddress, mask = mask });
            denyCount--;
            if (denyCount == 0)
            {
                if (status == SecurityStatus.GETIPRESTRICTIONS)
                {
                    status = SecurityStatus.GETIPRESTRICTIONS2;
                }
                else
                {
                    status = SecurityStatus.DORMANT;
                }
                deniedIPList.Enabled = true;
            }
        }

        public void receiveIPAllow(string ipaddress, uint mask)
        {
            allowedIPList.Items.Add(new IPRestriction { ipaddress = ipaddress, mask = mask });
            allowCount--;
            if (allowCount == 0)
            {
                if (status == SecurityStatus.GETIPRESTRICTIONS)
                {
                    status = SecurityStatus.GETIPRESTRICTIONS2;
                }
                else
                {
                    status = SecurityStatus.DORMANT;
                }
                allowedIPList.Enabled = true;
            }
        }

        public void securityPageControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (securityPageControl.SelectedTab == userManagerPage)
            {
                if (status == SecurityStatus.DORMANT)
                {
                    status = SecurityStatus.GETUSERS;
                    Command cmd = Command.Config | Command.GetUsers;
                    msgQueue.Add(new BAPSCommon.Message(cmd));
                }
                else
                {
                    MessageBox.Show("A command is still being processed please wait and use the refresh button to update the form.", "Notice:", MessageBoxButtons.OK);
                }
            }
            else if (securityPageControl.SelectedTab == connectionManagerPage)
            {
                if (status == SecurityStatus.DORMANT)
                {
                    status = SecurityStatus.GETIPRESTRICTIONS;
                    Command cmd = Command.Config | Command.GetIpRestrictions;
                    msgQueue.Add(new BAPSCommon.Message(cmd));
                }
                else
                {
                     MessageBox.Show("A command is still being processed please wait and use the refresh button to update the form.", "Notice:", MessageBoxButtons.OK);
                }
            }
        }

        public void receiveConfigError(uint resultCode, string description)
        {
            if (resultCode != 0)
            {
                MessageBox.Show(string.Concat("Unable to alter IP list, reason: ", description), "Error:", MessageBoxButtons.OK);
            }
            status = SecurityStatus.GETIPRESTRICTIONS;
            Command cmd = Command.Config | Command.GetIpRestrictions;
            msgQueue.Add(new BAPSCommon.Message(cmd));
        }

        private void alterRestrictionButton_Click(object sender, EventArgs e)
        {
            if (status == SecurityStatus.DORMANT)
            {
                try
                {
                    int cmdMask = (int)((Button)sender).Tag;
                    status = SecurityStatus.ALTERIPRESTRICTION;
                    var cmd = Command.Config | Command.AlterIpRestriction | (Command)cmdMask;
                    msgQueue.Add(new BAPSCommon.Message(cmd).Add(ipAddressText.Text).Add(Convert.ToUInt32(maskText.Text)));
                }
                catch (FormatException)
        		{
                    MessageBox.Show("The mask value must be an integer.", "Error:", MessageBoxButtons.OK);
                    status = SecurityStatus.DORMANT;
                }
            }
	        else
	        {
                MessageBox.Show("A command is still being processed please wait and use the refresh button to update the form.", "Notice:", MessageBoxButtons.OK);
            }
        }

        void SecurityDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == KeyShortcuts.Security) return;
            KeyDownForward?.Invoke(sender, e);
        }

        private PermissionInfo[] permissionInfo = null;

        private System.Collections.Concurrent.BlockingCollection<BAPSCommon.Message> msgQueue;
		private SecurityStatus status = SecurityStatus.AWAITING_INIT;
        private uint userCount = 0;
        private uint permissionCount = 0;
        private uint allowCount = 0;
        private uint denyCount = 0;
    }
}
