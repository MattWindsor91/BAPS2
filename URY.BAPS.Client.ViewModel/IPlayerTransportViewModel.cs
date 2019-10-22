using System;
using System.Reactive;
using ReactiveUI;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface for player transport view models.
    /// </summary>
    public interface IPlayerTransportViewModel : IDisposable
    {
        /// <summary>
        ///     Whether this channel is playing, according to the server.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        ///     Whether this channel is paused, according to the server.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        ///     Whether this channel is stopped, according to the server.
        /// </summary>
        bool IsStopped { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to start playing
        ///     on this channel.
        /// </summary>
        ReactiveCommand<Unit, Unit> Play { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to pause
        ///     this channel.
        /// </summary>
        ReactiveCommand<Unit, Unit> Pause { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to stop
        ///     this channel.
        /// </summary>
        ReactiveCommand<Unit, Unit> Stop { get; }
    }
}