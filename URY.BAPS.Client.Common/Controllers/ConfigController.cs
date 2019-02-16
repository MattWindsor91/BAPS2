using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Controller for requesting server config changes through BapsNet.
    /// </summary>
    public class ConfigController : BapsNetControllerBase
    {
        /// <summary>
        ///     The config cache, used for looking up details about configuration options.
        /// </summary>
        [NotNull] private readonly ConfigCache _cache;

        public ConfigController([CanBeNull] IClientCore core, [CanBeNull] ConfigCache cache) : base(core)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        ///     Sends a BAPSNet message to set a Boolean ('Yes'/'No') option to one of its choices.
        /// </summary>
        /// <param name="optionKey">The key of the option to set.</param>
        /// <param name="flag">The Boolean equivalent of the new choice..</param>
        /// <param name="index">If present and valid, the index of the option to set.</param>
        public void SetFlag(OptionKey optionKey, bool flag, int index = ConfigCache.NoIndex)
        {
            SetChoice(optionKey, ChoiceKeys.BooleanToChoice(flag), index);
        }

        /// <summary>
        ///     Sends a BAPSNet message to set an option to one of its choices.
        /// </summary>
        /// <param name="optionKey">The key of the option to set.</param>
        /// <param name="choiceKey">The choice to use.</param>
        /// <param name="index">If present and valid, the index of the option to set.</param>
        public void SetChoice(OptionKey optionKey, string choiceKey, int index = ConfigCache.NoIndex)
        {
            var optionId = (uint) optionKey;
            var choiceIndex = _cache.ChoiceIndexFor(optionId, choiceKey);
            var cmd = Command.Config | Command.SetConfigValue;
            if (index != ConfigCache.NoIndex) cmd |= Command.ConfigUseValueMask | (Command) index;
            SendAsync(new Message(cmd).Add(optionId).Add((uint) ConfigType.Choice).Add((uint) choiceIndex));
        }


        /// <summary>
        ///     Asks the server to retrieve an integer config key, producing a task that waits until either
        ///     the config cache receives it or the given timeout period elapses.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="timeout">A time-span that, will be used as the timeout for this poll.</param>
        /// <param name="fallback">A fallback value to use if the timeout elapses.</param>
        /// <param name="index">If given, the specific index to retrieve.</param>
        /// <returns>A task that completes with the retrieved integer value.</returns>
        public int PollInt(OptionKey key, TimeSpan timeout, int fallback = -1, int index = ConfigCache.NoIndex)
        {
            var mainObservable = Observable.FromEventPattern<ConfigCache.IntChangeEventArgs>(
                ev =>
                {
                    _cache.IntChanged += ev;
                    // This is a strange place to put this, but necessary;
                    // the BapsNet conversation that results in receiving the config setting has to
                    // take place within the time window that 'ev' is registered.
                    SendAsync(new Message(Command.Config | Command.GetConfigSetting).Add((uint) key));
                },
                ev => _cache.IntChanged -= ev
            ).FirstAsync(
                ev => ev.EventArgs.Key == key && ev.EventArgs.Index == index
            ).Select(
                ev => ev.EventArgs.Value
            );
            var fallbackObservable = Observable.Return(fallback).Delay(timeout);
            return mainObservable.Amb(fallbackObservable).Wait();
        }
    }
}