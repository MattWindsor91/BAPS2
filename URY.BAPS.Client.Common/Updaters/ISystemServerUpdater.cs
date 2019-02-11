using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.ErrorEventArgs> ObserveError { get; }
        IObservable<bool> ObserveServerQuit { get; }
        IObservable<Receiver.VersionInfo> ObserveVersion { get; }

        IObservable<Updates.TextSettingEventArgs> ObserveTextSetting { get; }
    }
}