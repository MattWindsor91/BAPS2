using System;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Updaters
{
    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        IObservable<ErrorEventArgs> ObserveError { get; }
        IObservable<ServerQuitArgs> ObserveServerQuit { get; }
        IObservable<ServerVersionArgs> ObserveServerVersion { get; }

        IObservable<TextSettingArgs> ObserveTextSetting { get; }
    }
}