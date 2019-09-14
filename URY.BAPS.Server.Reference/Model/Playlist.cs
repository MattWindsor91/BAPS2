using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Server.Model
{
    public sealed class Playlist
    {
        // Ported from the original BAPS2 server.


        /// <summary>
        ///     The channel number associated with this playlist.
        /// </summary>
        private readonly byte _channelId;

        /// <summary>
        ///     The current array of tracks.
        /// </summary>
        private readonly List<PlaylistItem> _entries = new List<PlaylistItem>();

        /// <summary>
        ///     The loaded text item, if any.
        /// </summary>
        private IPlaylistItem _loadedTextItem = new NullPlaylistItem();

        public Playlist(byte channelId)
        {
            _channelId = channelId;
        }

        public int Count => _entries.Count;

        /// <summary>
        ///     Finds the next audio entry in the list, given a start index.
        /// </summary>
        /// <param name="currentIndex">
        ///     The index to start the search from, inclusive.
        /// </param>
        /// <returns>
        ///     The index of the next audio entry, or return -1 if there are no more
        ///     such entries.
        /// </returns>
        public int GetNextPlayable(int currentIndex)
        {
            return _entries.FindIndex(currentIndex, item => item.IsAudioItem);
        }

        public ITrack? Get(int index)
        {
            return IsInRange(index) ? _entries[index] : null;
        }

        #region Event interface

        public event EventHandler<TrackAddArgs>? TrackAdd;
        public event EventHandler<TrackDeleteArgs>? TrackDelete;
        public event EventHandler<TrackMoveArgs>? TrackMove;
        public event EventHandler<PlaylistResetArgs>? PlaylistReset;

        #endregion Event interface

        #region Observable interface

        /// <summary>
        ///     An observable that mirrors <see cref="TrackAdd" />.
        /// </summary>
        public IObservable<TrackAddArgs> ObserveTrackAdd =>
            _observeTrackAdd ??=
                Observable.FromEventPattern<TrackAddArgs>(
                    ev => TrackAdd += ev,
                    ev => TrackAdd -= ev
                ).Select(x => x.EventArgs);

        private IObservable<TrackAddArgs>? _observeTrackAdd;


        /// <summary>
        ///     An observable that mirrors <see cref="TrackDelete" />.
        /// </summary>
        public IObservable<TrackDeleteArgs> ObserveTrackDelete =>
            _observeTrackDelete ??=
                Observable.FromEventPattern<TrackDeleteArgs>(
                    ev => TrackDelete += ev,
                    ev => TrackDelete -= ev
                ).Select(x => x.EventArgs);

        private IObservable<TrackDeleteArgs>? _observeTrackDelete;

        /// <summary>
        ///     An observable that mirrors <see cref="TrackMove" />.
        /// </summary>
        public IObservable<TrackMoveArgs> ObserveTrackMove =>
            _observeTrackMove ??=
                Observable.FromEventPattern<TrackMoveArgs>(
                    ev => TrackMove += ev,
                    ev => TrackMove -= ev
                ).Select(x => x.EventArgs);

        private IObservable<TrackMoveArgs>? _observeTrackMove;

        /// <summary>
        ///     An observable that mirrors <see cref="PlaylistReset" />.
        /// </summary>
        public IObservable<PlaylistResetArgs> ObservePlaylistReset =>
            _observePlaylistReset ??=
                Observable.FromEventPattern<PlaylistResetArgs>(
                    ev => PlaylistReset += ev,
                    ev => PlaylistReset -= ev
                ).Select(x => x.EventArgs);

        private IObservable<PlaylistResetArgs>? _observePlaylistReset;

        #endregion Observable interface

        #region Event invocators

        private void OnTrackAdd(TrackAddArgs e)
        {
            TrackAdd?.Invoke(this, e);
        }

        private void OnTrackDelete(TrackDeleteArgs e)
        {
            TrackDelete?.Invoke(this, e);
        }

        private void OnTrackMove(TrackMoveArgs e)
        {
            TrackMove?.Invoke(this, e);
        }

        private void OnPlaylistReset(PlaylistResetArgs e)
        {
            PlaylistReset?.Invoke(this, e);
        }

        #endregion Event invocators

        #region Pre-flight checks

        public bool CanRemove(int index)
        {
            return IsInRange(index) && _entries[index].IsSafeToRemove;
        }

        public bool CanMove(int fromIndex, int toIndex)
        {
            return IsInRange(fromIndex) && IsInRange(toIndex);
        }

        private bool IsInRange(int index)
        {
            return 0 <= index && index < Count;
        }

        #endregion Pre-flight checks

        #region Adding

        public void Add(ITrack track)
        {
            var item = new PlaylistItem(track, (uint) Count);
            _entries.Add(item);
            NotifyAdd(item);
        }

        private void NotifyAdd(PlaylistItem item)
        {
            var args = new TrackAddArgs(_channelId, item.Index, item);
            OnTrackAdd(args);
        }

        #endregion Adding

        #region Removing

        public void Remove(int index)
        {
            if (!CanRemove(index))
                throw new ArgumentException("entry not safe to remove", nameof(index));

            RemoveDirectly(index);
            RefreshIndices(index, _entries.Count);
            NotifyRemove((uint) index);
        }

        private void RemoveDirectly(int index)
        {
            _entries.RemoveAt(index);
            if (_loadedTextItem.HasIndex((uint) index)) UnlinkLoadedTextItem();
        }

        private void UnlinkLoadedTextItem()
        {
            _loadedTextItem = new NullPlaylistItem();
        }

        private void NotifyRemove(uint index)
        {
            var args = new TrackDeleteArgs(_channelId, index);
            OnTrackDelete(args);
        }

        #endregion Removing

        #region Moving

        public void Move(int fromIndex, int toIndex)
        {
            if (!IsInRange(fromIndex))
                throw new ArgumentOutOfRangeException(nameof(fromIndex));
            if (!IsInRange(toIndex))
                throw new ArgumentOutOfRangeException(nameof(toIndex));
            if (fromIndex == toIndex) return;

            MoveDirectly(fromIndex, toIndex);
            RefreshIndicesAfterMove(fromIndex, toIndex);

            NotifyMove((uint) fromIndex, (uint) toIndex);
        }

        private void MoveDirectly(int oldIndex, int newIndex)
        {
            var item = _entries[oldIndex];
            _entries.RemoveAt(oldIndex);
            _entries.Insert(newIndex, item);
        }

        private void NotifyMove(uint oldIndex, uint newIndex)
        {
            var args = new TrackMoveArgs(_channelId, oldIndex, newIndex);
            OnTrackMove(args);
        }

        #endregion Moving

        #region Resetting

        public void Reset()
        {
            ResetDirectly();
            RefreshIndices(0, _entries.Count);
            NotifyReset();
        }

        private void ResetDirectly()
        {
            UnlinkLoadedTextItem();
            _entries.RemoveAll(e => e.IsSafeToRemove);
        }

        /// <summary>
        ///     Notifies anything observing playlist resets that the playlist has
        ///     been reset.
        ///     <para>
        ///         This happens in two stages.  First, we send a general reset
        ///         notification.  Then, for any item that wasn't safe to
        ///         delete, we send an add notification.
        ///     </para>
        /// </summary>
        private void NotifyReset()
        {
            OnPlaylistReset(new PlaylistResetArgs(_channelId));
            _entries.ForEach(NotifyAdd);
        }

        #endregion Resetting

        #region Index book-keeping

        private void RefreshIndicesAfterMove(int oldIndex, int newIndex)
        {
            var start = Math.Min(oldIndex, newIndex);
            var end = Math.Max(oldIndex, newIndex);
            RefreshIndices(start, end);
        }

        /// <summary>
        ///     Updates the entry-side indices for each entry between
        ///     <paramref name="start" /> (inclusive) and <paramref name="end" /> (exclusive).
        /// </summary>
        /// <param name="start">The first index to refresh.</param>
        /// <param name="end">One past the last index to refresh.</param>
        private void RefreshIndices(int start, int end)
        {
            for (var i = start; i < end; i++) RefreshIndex(i);
        }

        private void RefreshIndex(int at)
        {
            _entries[at].Index = (uint) at;
        }

        #endregion Index book-keeping
    }
}