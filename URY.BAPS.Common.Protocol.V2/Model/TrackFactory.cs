using System;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Common.Protocol.V2.Model
{
    /// <summary>
    ///     Creates instances of tracks given information taken from a BapsNet message.
    /// </summary>
    public static class TrackFactory
    {
        /// <summary>
        ///     Creates a <see cref="TrackBase" /> with the given type and description.
        /// </summary>
        /// <param name="type">The BapsNet type of the entry.</param>
        /// <param name="description">The description of the entry.</param>
        /// <param name="duration">If given, the duration to use for any constructed audio items.</param>
        /// <param name="text">If given, the text to use for any constructed text items.</param>
        /// <returns>
        ///     A <see cref="TrackBase" /> with the correct type
        ///     and description.
        /// </returns>
        public static ITrack Create(TrackType type, string description, uint duration = 0, string text = "")
        {
            return type switch
                {
                TrackType.File =>
                (ITrack) new FileTrack(description, duration),
                TrackType.Text =>
                new TextTrack(description, text),
                TrackType.Library =>
                new LibraryTrack(description, duration),
                TrackType.Void =>
                CreateSpecialTrack(description),
                _ =>
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unrecognised track type.")
                };
        }

        private static ITrack CreateSpecialTrack(string description)
        {
            return description switch
                {
                SpecialTrackDescriptions.LoadFailed =>
                (ITrack) new ErrorTrack(),
                SpecialTrackDescriptions.Loading =>
                new LoadingTrack(),
                _ =>
                new NullTrack(description)
                };
        }
    }
}