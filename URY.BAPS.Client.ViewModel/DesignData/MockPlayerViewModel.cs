namespace URY.BAPS.Client.ViewModel.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IPlayerViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to player controls in design mode.
    ///     </para>
    /// </summary>
    public sealed class MockPlayerViewModel : IPlayerViewModel
    {
        public IPlayerTransportViewModel Transport { get; } = new MockPlayerTransportViewModel();
        public IPlayerMarkerViewModel Markers { get; } = new MockPlayerMarkerViewModel();
        public IPlayerTrackViewModel Track { get; } = new MockPlayerTrackViewModel();

        public void Dispose()
        {
            Transport?.Dispose();
            Markers?.Dispose();
        }
    }
}