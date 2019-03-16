using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        IObservable<ErrorEventArgs> ObserveError { get; }
        IObservable<bool> ObserveServerQuit { get; }
        IObservable<Receiver.VersionInfo> ObserveVersion { get; }

        IObservable<TextSettingEventArgs> ObserveTextSetting { get; }
    }
}