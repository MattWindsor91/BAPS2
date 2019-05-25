namespace URY.BAPS.Client.Common.ClientConfig
{
    /// <summary>
    ///     Interface for classes that allow for the retrieval of client
    ///     configuration.
    ///     <para>
    ///         Unlike 'classic' BAPS2, we don't support saving client
    ///         configuration directly from the client.
    ///     </para>
    /// </summary>
    public interface IClientConfigManager
    {
        /// <summary>
        ///     Loads the current client config.
        /// </summary>
        /// <returns>
        ///     The currently-saved client config.
        /// </returns>
        ClientConfig LoadConfig();
    }
}