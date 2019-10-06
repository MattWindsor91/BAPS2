using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

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

        /// <summary>
        ///     A command that, when invoked, increases the text size.
        /// </summary>
        ReactiveCommand<Unit, Unit> IncreaseFontScale { get; }

        /// <summary>
        ///     A command that, when invoked, decreases the text size.
        /// </summary>
        ReactiveCommand<Unit, Unit> DecreaseFontScale { get; }
    }
}