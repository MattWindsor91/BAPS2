using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Event interface for classes that contain BapsNet system event feeds.
    /// </summary>
    public interface ISystemEventFeed : IEventFeed
    {
        IObservable<ErrorEventArgs> ObserveError { get; }
        IObservable<ServerQuitArgs> ObserveServerQuit { get; }

        /// <summary>
        ///     Observable that reports when the server announces its
        ///     version.
        /// </summary>
        IObservable<ServerVersionArgs> ObserveServerVersion { get; }

        /// <summary>
        ///     Observable that reports when the server requests a change on
        ///     its connected clients' text windows.
        ///     <para>
        ///         This sort of request comes from the BAPS controller and
        ///         is routed through the BAPS server.  The server itself has
        ///         no logic for tracking text windows and, as such, this
        ///         particular event is one-sided and (presumably) rare.
        ///     </para>
        /// </summary>
        IObservable<TextSettingArgs> ObserveTextSetting { get; }
    }
}