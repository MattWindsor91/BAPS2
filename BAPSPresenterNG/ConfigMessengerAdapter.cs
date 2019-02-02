using BAPSClientCommon.ServerConfig;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to events from a config cache
    ///     and forwards them onto the messenger bus.
    /// </summary>
    public class ConfigMessengerAdapter : EventMessengerAdapterBase
    {
        private readonly Cache _cache;

        /// <summary>
        ///     Constructs a <see cref="ConfigMessengerAdapter" />.
        ///     <para>
        ///         This constructor registers the appropriate event handlers
        ///         on <paramref name="cache" />.
        ///     </para>
        /// </summary>
        /// <param name="cache">The server config cache to listen to.</param>
        /// <param name="messenger">The messenger bus to forward onto.</param>
        public ConfigMessengerAdapter(Cache cache, IMessenger messenger) : base(messenger)
        {
            _cache = cache;
            Register();
        }

        private void Register()
        {
            _cache.IntChanged += Relay;
            _cache.StringChanged += Relay;
            _cache.ChoiceChanged += Relay;
        }
    }
}