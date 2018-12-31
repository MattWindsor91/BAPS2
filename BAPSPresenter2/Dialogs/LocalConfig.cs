using System;
using System.Windows.Forms;

namespace BAPSPresenter2.Dialogs
{
    public partial class LocalConfig : Form
    {
        /// <summary>
        /// A handle to the main window.
        /// </summary>
        private Main main;

        public LocalConfig(Main main)
        {
            this.main = main;

            InitializeComponent();

            serverText.Text = ConfigManager.getConfigValueString("ServerAddress", "127.0.0.1");
            portText.Text = ConfigManager.getConfigValueString("ServerPort", Properties.Resources.DefaultPort);
            usernameText.Text = ConfigManager.getConfigValueString("DefaultUsername", "");
            passwordText.Text = ConfigManager.getConfigValueString("DefaultPassword", "");
            enableTimersList.SelectedItem = ConfigManager.getConfigValueString("EnableTimers", "Yes");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (serverText.Text.Equals("studio1")) ConfigManager.setConfigValue("ServerAddress", "144.32.64.181");
            else if (serverText.Text.Equals("studio2")) ConfigManager.setConfigValue("ServerAddress", "144.32.64.184");
            else if (serverText.Text.Equals("production")) ConfigManager.setConfigValue("ServerAddress", "144.32.64.178");
            else if (serverText.Text.Equals("localhost")) ConfigManager.setConfigValue("ServerAddress", "127.0.0.1");
            else ConfigManager.setConfigValue("ServerAddress", serverText.Text);
            ConfigManager.setConfigValue("ServerPort", portText.Text);
            ConfigManager.setConfigValue("DefaultUsername", usernameText.Text);
            ConfigManager.setConfigValue("DefaultPassword", passwordText.Text);
            ConfigManager.setConfigValue("EnableTimers", (string)enableTimersList.SelectedItem);
            Close();
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LocalConfigDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == KeyShortcuts.LocalConfig) return;
            main.Invoke((KeyEventHandler)main.BAPSPresenterMain_KeyDown, sender, e);
        }
    }
}
