using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A mock-up view model for the text pane.
    /// </summary>
    [UsedImplicitly]
    public class MockTextViewModel : ITextViewModel
    {
        private const string ExampleText =
            "It was a dark and stormy night; the rain fell in torrents — except at occasional intervals," +
            " when it was checked by a violent gust of wind which swept up the streets " +
            "(for it is in London that our scene lies), rattling along the housetops, " +
            "and fiercely agitating the scanty flame of the lamps that struggled against the darkness.";

        public MockTextViewModel(double fontSize, string text)
        {
            FontSize = fontSize;
            Text = text;
        }

        public MockTextViewModel() : this(14.0, ExampleText)
        {
        }

        public double FontSize { get; set; }
        public string Text { get; set; }
    }
}