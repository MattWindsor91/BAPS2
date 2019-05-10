namespace URY.BAPS.Model.MessageEvents
{
    /// <summary>
    ///     Interface for objects that can receive BAPS messages.
    /// </summary>
    public interface IMessageSink
    {
        /// <summary>
        ///     Sends a BAPS message event to this sink.
        /// </summary>
        /// <param name="e">The message to send.</param>
        void OnMessageReceived(MessageArgsBase e);
    }
}