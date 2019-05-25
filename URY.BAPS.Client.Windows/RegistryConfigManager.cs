using System;
using Microsoft.Win32;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.Windows
{
    /// <inheritdoc />
    /// <summary>
    ///     A client config manager that uses the Win32 registry.
    ///     <para>
    ///         This is mostly a port of the analogous C++ class in the original version of BAPS2.
    ///     </para>
    /// </summary>
    public class RegistryConfigManager : IDisposable, IClientConfigManager
    {
        private RegistryKey _bapsClient;
        private RegistryKey _software;
        private RegistryKey _ury;

        private const string ServerAddressKey = "ServerAddress";
        private const string ServerPortKey = "ServerPort";
        private const string DefaultUsernameKey = "DefaultUsername";

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

        private static RegistryKey GetOrCreateSubKey(RegistryKey parent, string name)
        {
            var subKey = parent.OpenSubKey(name, true);
            if (subKey != null) return subKey;
            subKey = parent.CreateSubKey(name, true);
            if (subKey != null) return subKey;
            throw new Exception($"Create of key {name} failed");
        }

        public ClientConfig LoadConfig()
        {
            var config = new ClientConfig();

            config.ServerAddress = GetString(ServerAddressKey, config.ServerAddress);

            config.ServerPort = GetIntFromString(ServerPortKey, config.ServerPort);
            config.DefaultUsername = GetString(DefaultUsernameKey, config.DefaultUsername);
            return config;
        }

        /// <summary>
        ///     Retrieves a string from the registry.
        /// </summary>
        /// <param name="key">The registry key to get.</param>
        /// <param name="defaultValue">The default value to use if the string doesn't exist.</param>
        /// <returns>
        ///     Either the string stored at registry
        ///     key <paramref name="key"/>, or <paramref name="defaultValue"/>.
        /// </returns>
        private string GetString(string key, string defaultValue)
        {
            return (string) BapsClient.GetValue(key, defaultValue);
        }

        /// <summary>
        ///     Retrieves a string from the registry, then parses it as an int.
        ///     <para>
        ///         BAPS2 stores the server port this way.
        ///     </para>
        /// </summary>
        /// <param name="key">The registry key to get.</param>
        /// <param name="defaultValue">The default value to use if the string doesn't exist.</param>
        /// <returns>
        ///     Either the integer parsed from the string stored at registry
        ///     key <paramref name="key"/>, or <paramref name="defaultValue"/>.
        /// </returns>
        private int GetIntFromString(string key, int defaultValue)
        {
            var value = defaultValue;
            if (BapsClient.GetValue(key) is string s) int.TryParse(s, out value);
            return value;
        }

        public void SaveConfig(ClientConfig config)
        {
            SetString(ServerAddressKey, config.ServerAddress);
            SetString(ServerPortKey, config.ServerPort.ToString());
            SetString(DefaultUsernameKey, config.DefaultUsername);
        }

        private void SetString(string key, string value)
        {
            BapsClient.SetValue(key, value, RegistryValueKind.String);
        }
    }
}