namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Interface for text-pane view models.
    /// </summary>
    public interface ITextViewModel
    {
        /// <summary>
        ///     The font scale, in percent.
        /// </summary>
        int FontScale { get; }

        /// <summary>
        ///     The text stored in the text panel.
        /// </summary>
        string Text { get; set; }
    }
}