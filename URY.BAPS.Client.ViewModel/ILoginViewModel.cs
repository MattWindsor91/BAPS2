namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface used for login dialog box view models.
    ///
    ///     <para>
    ///         This interface does not specify a means for getting to the
    ///         password.  This is because of the fact that WPF doesn't support
    ///         directly binding a password box to a property, for security
    ///         reasons.  Instead, any code reading off the fields of a
    ///         <see cref="ILoginViewModel"/> will need to scrape the password
    ///         out of the dialog.
    ///     </para>
    /// </summary>
    public interface ILoginViewModel
    {
        /// <summary>
        ///     The username that will be used to log into the BAPS server.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        ///     The hostname of the custom server.
        /// </summary>
        string Server { get; set; }

        /// <summary>
        ///     The port of the custom server.
        /// </summary>
        int Port { get; set; }
    }
}