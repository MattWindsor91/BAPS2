using Microsoft.Win32;
using System;

namespace BAPSClientWindows
{
    /// <summary>
    /// A local config manager that uses the Win32 registry.
    /// <para>
    /// This is mostly a port of the analogous C++ class in the original version of BAPS2.
    /// </para>
    /// </summary>
    public class ConfigManager : IDisposable
    {
        public void Dispose()
        {
            // Don't use the properties here: they'll open/create the registry
            // keys if they weren't already opened!
            _bapsClient?.Close();
            _ury?.Close();
            _software?.Close();
        }

        public T GetValue<T>(string name) => (T)BapsClient.GetValue(name);

        public T GetValue<T>(string name, T defaultValue) => (T)BapsClient.GetValue(name, defaultValue);

        public void SetValue<T>(string name, T value)
        {
            BapsClient.SetValue(name, value);
        }

        private RegistryKey GetOrCreateSubkey(RegistryKey parent, string name)
        {
            var subkey = parent.OpenSubKey(name, writable: true);
            if (subkey != null) return subkey;
            subkey = parent.CreateSubKey(name, writable: true);
            if (subkey != null) return subkey;
            throw new Exception($"Create of key {name} failed");
        }

        private RegistryKey Software => _software ?? (_software = GetOrCreateSubkey(Registry.CurrentUser, "software"));
        private RegistryKey _software = null;
        private RegistryKey Ury => _ury ?? (_ury = GetOrCreateSubkey(Software, "URY"));
        private RegistryKey _ury = null;
        private RegistryKey BapsClient => _bapsClient ?? (_bapsClient = GetOrCreateSubkey(Ury, "BAPSClient"));
        private RegistryKey _bapsClient = null;
    }
}
