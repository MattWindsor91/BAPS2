using System.Windows.Forms;

namespace BAPSPresenter2
{
    /// <summary>
    /// Keyboard shortcuts used in BAPS Presenter.
    /// </summary>
    public static class KeyShortcuts
    {
        public const Keys About = Keys.Control | Keys.Alt | Keys.A;
        public const Keys AlterWindow = Keys.Control | Keys.Alt | Keys.W;
        public const Keys Config = Keys.Control | Keys.O;
        public const Keys LocalConfig = Keys.Control | Keys.Shift | Keys.O;
        public const Keys Security = Keys.Control | Keys.S;
        public const Keys Shutdown = Keys.Control | Keys.Alt | Keys.Q;
        public const Keys TextMaximised = Keys.Control | Keys.Shift | Keys.N;
        public const Keys Text = Keys.Control | Keys.N;
    }
}
