using System;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Model.MessageEvents;
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

        /// <summary>
        ///     Observable that reports when the server responds to a
        ///     configuration change request.
        /// </summary>
        IObservable<ConfigResultArgs> ObserveConfigResult { get; }
        
        /// <summary>
        ///     Observable that reports when the server declares an IP restriction record.
        /// </summary>
        IObservable<IpRestrictionArgs> ObserveIpRestriction { get; }
        
        /// <summary>
        ///     Observable that reports when the server declares a permission record.
        /// </summary>
        IObservable<PermissionArgs> ObservePermission { get; }
        
        /// <summary>
        ///     Observable that reports when the server declares a user record.
        /// </summary>
        IObservable<UserArgs> ObserveUser { get; }
        
        /// <summary>
        ///     Observable that reports when the server responds to a
        ///     user change request.
        /// </summary>
        IObservable<UserResultArgs> ObserveUserResult { get; }
    }
}