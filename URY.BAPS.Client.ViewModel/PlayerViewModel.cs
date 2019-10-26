using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     View model governing the player head of a channel.
    /// </summary>
    public class PlayerViewModel : ViewModelBase, IPlayerViewModel
    {
        public PlayerViewModel(IPlayerTransportViewModel transport, IPlayerMarkerViewModel markers, IPlayerTrackViewModel track)
        {
            Markers = markers;
            Transport = transport;
            Track = track;
        }

        public IPlayerMarkerViewModel Markers { get; }

        public IPlayerTransportViewModel Transport { get; }

        public IPlayerTrackViewModel Track { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Markers.Dispose();
                Transport.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}