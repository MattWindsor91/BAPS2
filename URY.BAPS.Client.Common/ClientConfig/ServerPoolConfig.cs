using System.Linq;
using URY.BAPS.Client.Common.ServerSelect;

namespace URY.BAPS.Client.Common.ClientConfig
{
    /// <summary>
    ///     Represents the server-pool configuration in a BAPS client
    ///     configuration tree.
    ///     <para>
    ///         As with all such objects, this class is designed to have
    ///         configuration bound to it.
    ///     </para>
    /// </summary>
    public class ServerPoolConfig
    {
        /// <summary>
        ///     The name of the default server.
        /// </summary>
        public string Default { get; set; } = "";

        /// <summary>
        ///     The configured server records.
        /// </summary>
        public ServerRecord[] Records { get; set; } = { };

        /// <summary>
        ///     Tries to resolve <see cref="Default"/> against <see cref="Records"/>, producing the first matching
        ///     record if one exists or an invalid record if not.
        /// </summary>
        public ServerRecord DefaultRecord =>
            Records.FirstOrDefault(s => s.Name == Default) ?? ServerRecord.Empty();
    }
}