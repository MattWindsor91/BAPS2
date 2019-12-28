namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    /// 
    ///     A pair of channel ID and channel playlist index.
    /// </summary>
    public struct TrackIndex
    {
        /// <summary>
        ///     Deconstructs a <see cref="TrackIndex"/> into its channel ID and index.
        /// </summary>
        /// <param name="channelId">The channel ID to output.</param>
        /// <param name="index">The index to output.</param>
        public void Deconstruct(out ushort channelId, out uint index)
        {
            channelId = ChannelId;
            index = Position;
        }
 
        public override string ToString()
        {
            return $"{ChannelId}:{Position}";
        }
 
        /// <summary>
        ///     The ID of the channel to which this index refers.
        /// </summary>
        public ushort ChannelId;
         
        /// <summary>
        ///     The position in the channel's track list to which this index refers.
        /// </summary>
        public uint Position;
    }
}