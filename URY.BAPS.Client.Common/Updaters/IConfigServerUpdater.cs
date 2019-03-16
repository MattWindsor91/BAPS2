using System;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server config updates.
    /// </summary>
    public interface IConfigServerUpdater : IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that reports when the server declares a config choice.
        /// </summary>
        IObservable<ConfigChoiceEventArgs> ObserveConfigChoice { get; }

        /// <summary>
        ///     Observable that reports when the server declares a config option.
        /// </summary>
        IObservable<ConfigOptionEventArgs> ObserveConfigOption { get; }

        /// <summary>
        ///     Event raised when the server declares that a setting on a
        ///     config option has changed.
        /// </summary>
        IObservable<ConfigSettingEventArgs> ObserveConfigSetting { get; }

        IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult { get; }
    }
}