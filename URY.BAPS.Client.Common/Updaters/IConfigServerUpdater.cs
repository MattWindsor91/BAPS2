using System;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Protocol.V2.Commands;

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
        IObservable<ConfigChoiceArgs> ObserveConfigChoice { get; }

        /// <summary>
        ///     Observable that reports when the server declares a config option.
        /// </summary>
        IObservable<ConfigOptionArgs> ObserveConfigOption { get; }

        /// <summary>
        ///     Event raised when the server declares that a setting on a
        ///     config option has changed.
        /// </summary>
        IObservable<ConfigSettingArgs> ObserveConfigSetting { get; }

        IObservable<ConfigResultArgs> ObserveConfigResult { get; }
    }
}