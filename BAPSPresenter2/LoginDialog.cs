using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class LoginDialog : Form
    {
        /** If this is the first time we have attempted to connect or if
			the user has altered the server details, the connection must be
			recreated.
		**/
        public bool needsToConnect()
        {
            return (loginAttempt == 1) || connectionChanged;
        }
        /** Accessor for server text **/
        public string Server { get => serverText.Text; private set => serverText.Text = value; }
        /** Accessor for port number **/
        public int Port => int.Parse(portText.Text);
        /** Accessor for port number (as string) **/
        private string PortText { get => portText.Text; set => portText.Text = value; }
        /** Accessor for username **/
        public string Username { get => usernameText.Text; private set => usernameText.Text = value; }
        /** Accessor for password **/
        public string Password { get => passwordText.Text; private set => passwordText.Text = value; }

        /** flag to determine if the connection details have been changed
            since the last login/connection attempt
        **/
        bool connectionChanged = false;

        /** A counter to determine how many times we have attempted connection
            Currently used just to indicate that the first attempt needs
            to connect to the server even if the server details are not altered
        **/
        int loginAttempt = 0;

        public LoginDialog()
        {
            InitializeComponent();

            loginAttempt = 0;
            /** Get the default server and port from the config system **/
            Server = ConfigManager.getConfigValueString("ServerAddress", "localhost");
            PortText = ConfigManager.getConfigValueString("ServerPort", Properties.Resources.DefaultPort);
            /** Get the default username and password from the config system **/
            Username = ConfigManager.getConfigValueString("DefaultUsername", "");
            Password = ConfigManager.getConfigValueString("DefaultPassword", "");
        }
    }
}
