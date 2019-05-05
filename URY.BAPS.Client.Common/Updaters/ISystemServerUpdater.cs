using System;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Updaters
{
    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        IObservable<ErrorEventArgs> ObserveError { get; }
        IObservable<bool> ObserveServerQuit { get; }
        IObservable<ServerVersion> ObserveServerVersion { get; }

        IObservable<TextSettingEventArgs> ObserveTextSetting { get; }
    }
}