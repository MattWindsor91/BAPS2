using System;
using Microsoft.Win32;

namespace URY.BAPS.Client.Windows
{
    /// <inheritdoc />
    /// <summary>
    ///     A local config manager that uses the Win32 registry.
    ///     <para>
    ///         This is mostly a port of the analogous C++ class in the original version of BAPS2.
    ///     </para>
    /// </summary>
    public class ConfigManager : IDisposable
    {
        private RegistryKey _bapsClient;
        private RegistryKey _software;
        private RegistryKey _ury;

        private RegistryKey Software => _software ?? (_software = GetOrCreateSubKey(Registry.CurrentUser, "software"));
        private RegistryKey Ury => _ury ?? (_ury = GetOrCreateSubKey(Software, "URY"));
        private RegistryKey BapsClient => _bapsClient ?? (_bapsClient = GetOrCreateSubKey(Ury, "BAPSClient"));

        public void Dispose()
        {
            // Don't use the properties here: they'll open/create the registry
            // keys if they weren't already opened!
            _bapsClient?.Close();
            _ury?.Close();
            _software?.Close();
        }

        public T GetValue<T>(string name)
        {
            return (T) BapsClient.GetValue(name);
        }

        public T GetValue<T>(string name, T defaultValue)
        {
            return (T) BapsClient.GetValue(name, defaultValue);
        }

        public void SetValue<T>(string name, T value)
        {
            BapsClient.SetValue(name, value);
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