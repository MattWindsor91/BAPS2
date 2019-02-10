using System;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server config updates.
    /// </summary>
    public interface IConfigServerUpdater : IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that reports when the server declares a config choice.
        /// </summary>
        IObservable<Updates.ConfigChoiceEventArgs> ObserveConfigChoice { get; }

        /// <summary>
        ///     Observable that reports when the server declares a config option.
        /// </summary>
        IObservable<Updates.ConfigOptionEventArgs> ObserveConfigOption { get; }

        /// <summary>
        ///     Event raised when the server declares that a setting on a
        ///     config option has changed.
        /// </summary>
        IObservable<Updates.ConfigSettingEventArgs> ObserveConfigSetting { get; }

        IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult { get; }
    }
}