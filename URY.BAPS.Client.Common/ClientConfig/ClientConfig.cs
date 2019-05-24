using System;
using System.Collections.Generic;
using System.Text;

namespace URY.BAPS.Client.Common.ClientConfig
{
    /// <summary>
    ///     Data structure containing configuration for a BAPS client.
    /// </summary>
    public class ClientConfig
    {
        public string ServerAddress { get; set; } = "localhost";

        public int ServerPort { get; set; } = 1350;

        public string DefaultUsername { get; set; } = "";
    }
}
