using System;
using System.Reactive;
using ReactiveUI;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface for text-pane view models.
    ///
    ///     <para>
    ///         The 'text view model' contains data and commands for the part
    ///         of a BAPS client that shows text messages (either loaded from
    ///         text playlist items, or typed out by the user).  The view model
    ///         tracks two pieces of information: the text itself, and the
    ///         amount to which its font has been scaled up or down.
    ///     </para>
    ///     <para>
    ///         The text view model is unusual among view models in that its
    ///         state doesn't fully correspond to the state of part of the BAPS
    ///         server.  The text pane is effectively a client-specific piece
    ///         of state, and the BAPS server's direct interactions with it are
    ///         only the effects of a BAPS paddle asking it to relay commands
    ///         like 'scroll up' or 'decrease text size'.
    ///     </para>
    /// </summary>
    public interface ITextViewModel : IDisposable
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