using System;
using Microsoft.Win32;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.Windows
{
    /// <inheritdoc />
    /// <summary>
    ///     A local config manager that uses the Win32 registry.
    ///     <para>
    ///         This is mostly a port of the analogous C++ class in the original version of BAPS2.
    ///     </para>
    /// </summary>
    public class RegistryConfigManager : IDisposable
    {
        private RegistryKey _bapsClient;
        private RegistryKey _software;
        private RegistryKey _ury;

        /// <summary>
        ///     The <c>HKEY_CURRENT_USER\software</c> registry key.
        /// </summary>
        private RegistryKey Software => _software ??= GetOrCreateSubKey(Registry.CurrentUser, "software");

        /// <summary>
        ///     The <c>HKEY_CURRENT_USER\software\URY</c> registry key.
        /// </summary>
        private RegistryKey Ury => _ury ??= GetOrCreateSubKey(Software, "URY");

        /// <summary>
        ///     The <c>HKEY_CURRENT_USER\software\URY\BAPSClient</c> registry key.
        /// </summary>
        private RegistryKey BapsClient => _bapsClient ??= GetOrCreateSubKey(Ury, "BAPSClient");

        public void Dispose()
        {
            // Don't use the properties here: they'll open/create the registry
            // keys if they weren't already opened!
            _bapsClient?.Close();
            _ury?.Close();
            _software?.Close();
        }

        private T GetValue<T>(string name, T defaultValue)
        {
            return (T) BapsClient.GetValue(name, defaultValue);
        }

        private int GetIntFromStringValue(string name, int defaultValue)
        {
            var value = defaultValue;
            if (BapsClient.GetValue(name) is string s) int.TryParse(s, out value);
            return value;
        }

        public ClientConfig MakeConfig()
        {
            var config = new ClientConfig();
            config.ServerAddress = GetValue("ServerAddress", config.ServerAddress);
            config.ServerPort = GetIntFromStringValue("ServerPort", config.ServerPort);
            config.DefaultUsername = GetValue("DefaultUsername", config.DefaultUsername);
            return config;
        }

        private static RegistryKey GetOrCreateSubKey(RegistryKey parent, string name)
        {
            var subKey = parent.OpenSubKey(name, true);
            if (subKey != null) return subKey;
            subKey = parent.CreateSubKey(name, true);
            if (subKey != null) return subKey;
            throw new Exception($"Create of key {name} failed");
        }
    }
}