namespace URY.BAPS.Client.Common.ClientConfig
{
    /// <summary>
    ///     Data structure containing configuration for a BAPS client.
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        ///     The configured set of known and default BAPS servers.
        /// </summary>
        public ServerPoolConfig Servers { get; set; } = new ServerPoolConfig();
       
        /// <summary>
        ///     The default username to use when connecting to a BAPS server.
        /// </summary>
        public string DefaultUsername { get; set; } = "";
    }
}
