using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.ServerConfig;
using JetBrains.Annotations;

namespace BAPSClientCommon.Controllers
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
    }
}