namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Base class for view models that refer to part or all of a channel, and therefore have an associated
    ///     channel ID.
    /// </summary>
    public abstract class ChannelComponentViewModelBase : ViewModelBase
    {
        protected ChannelComponentViewModelBase(ushort channelId)
        {
            ChannelId = channelId;
        }

        /// <summary>
        ///     The ID of the channel this view model concerns.
        /// </summary>
        public ushort ChannelId { get; }
    }
}