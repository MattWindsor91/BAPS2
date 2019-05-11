using System;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Client.Common.Controllers
{
    public interface IConfigController
    {
        /// <summary>
        ///     Sends a BAPSNet message to set a Boolean ('Yes'/'No') option to one of its choices.
        /// </summary>
        /// <param name="optionKey">The key of the option to set.</param>
        /// <param name="flag">The Boolean equivalent of the new choice..</param>
        /// <param name="index">If present and valid, the index of the option to set.</param>
        void SetFlag(OptionKey optionKey, bool flag, int index = ConfigCache.NoIndex);

        /// <summary>
        ///     Sends a BAPSNet message to set an option to one of its choices.
        /// </summary>
        /// <param name="optionKey">The key of the option to set.</param>
        /// <param name="choiceKey">The choice to use.</param>
        /// <param name="index">If present and valid, the index of the option to set.</param>
        void SetChoice(OptionKey optionKey, string choiceKey, int index = ConfigCache.NoIndex);

        /// <summary>
        ///     Asks the server to retrieve an integer config key, producing a task that waits until either
        ///     the config cache receives it or the given timeout period elapses.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="timeout">A time-span that, will be used as the timeout for this poll.</param>
        /// <param name="fallback">A fallback value to use if the timeout elapses.</param>
        /// <param name="index">If given, the specific index to retrieve.</param>
        /// <returns>A task that completes with the retrieved integer value.</returns>
        int PollInt(OptionKey key, TimeSpan timeout, int fallback = -1, int index = ConfigCache.NoIndex);
    }
}