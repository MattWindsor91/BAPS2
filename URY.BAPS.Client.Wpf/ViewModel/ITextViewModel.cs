﻿namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Interface for text-pane view models.
    /// </summary>
    public interface ITextViewModel
    {
        /// <summary>
        ///     The font size.
        /// </summary>
        double FontSize { get; }

        /// <summary>
        ///     The text stored in the text panel.
        /// </summary>
        string Text { get; set; }
    }
}