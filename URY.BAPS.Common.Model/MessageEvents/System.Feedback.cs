namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     Event structure representing an attempt by the client to send
    ///     feedback on BAPS to the server.
    /// </summary>
    public class FeedbackRequestArgs : MessageArgsBase
    {
        /// <summary>
        ///     The feedback sent by the client, as a string.
        /// </summary>
        public string Feedback { get; }

        /// <summary>
        ///     Constructs a feedback request.
        /// </summary>
        /// <param name="feedback">
        ///     The feedback sent by the client, as a string.
        /// </param>
        public FeedbackRequestArgs(string feedback)
        {
            Feedback = feedback;
        }
    }

    /// <summary>
    ///     Event structure representing the response of the server to a
    ///     <see cref="FeedbackRequestArgs"/> request.
    /// </summary>
    public class FeedbackResponseArgs : MessageArgsBase
    {
        /// <summary>
        ///     Whether the client's feedback has been passed on.
        /// </summary>
        public bool WasSent { get; }

        /// <summary>
        ///     Constructs a feedback response.
        /// </summary>
        /// <param name="wasSent">
        ///     Whether the client's feedback has been passed on.
        /// </param>
        public FeedbackResponseArgs(bool wasSent)
        {
            WasSent = wasSent;
        }
    }
}