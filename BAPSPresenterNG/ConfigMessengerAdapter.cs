using System;
using BAPSClientCommon.ServerConfig;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to events from a config cache
    ///     and forwards them onto the messenger bus.
    /// </summary>
    public class ConfigMessengerAdapter : EventMessengerAdapterBase
    {
        [NotNull] private readonly ConfigCache _configCache;

        /// <summary>
        ///     Constructs a <see cref="ConfigMessengerAdapter" />.
        ///     <para>
        ///         This constructor registers the appropriate event handlers
        ///         on <paramref name="configCache" />.
        ///     </para>
        /// </summary>
        /// <param name="configCache">The server config cache to listen to.</param>
        /// <param name="messenger">The messenger bus to forward onto.</param>
        public ConfigMessengerAdapter([CanBeNull] ConfigCache configCache, IMessenger messenger) : base(messenger)
        {
            _configCache = configCache ?? throw new ArgumentNullException(nameof(configCache));
            Register();
        }

        private void Register()
        {
            _configCache.IntChanged += Relay;
            _configCache.StringChanged += Relay;
            _configCache.ChoiceChanged += Relay;
        }
    }
}