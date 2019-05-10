namespace URY.BAPS.Model.Track
{
    /// <summary>
    ///     A track that contains a block of text.
    /// </summary>
    public class TextTrack : NonAudioTrack
    {
        public TextTrack(string description, string text) : base(description)
        {
            Text = text;
        }

        public override bool IsLoading => false;
        public override bool IsError => false;
        public override bool IsTextItem => true;
        public override string Text { get; }
    }
}