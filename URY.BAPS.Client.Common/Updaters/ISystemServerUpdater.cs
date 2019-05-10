using System;
using URY.BAPS.Model.MessageEvents;

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