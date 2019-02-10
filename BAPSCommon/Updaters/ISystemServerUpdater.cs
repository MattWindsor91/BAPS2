using System;
using BAPSClientCommon.Events;

namespace BAPSClientCommon.Updaters
{
    public interface ISystemServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.ErrorEventArgs> ObserveError { get; }
        IObservable<bool> ObserveServerQuit { get; }
        IObservable<Receiver.VersionInfo> ObserveVersion { get; }
        
        IObservable<Updates.TextSettingEventArgs> ObserveTextSetting { get; }
    }
}