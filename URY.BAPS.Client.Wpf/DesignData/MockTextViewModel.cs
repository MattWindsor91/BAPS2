using System;
using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A mock-up view model for the text pane.
    /// </summary>
    [UsedImplicitly]
    public sealed class MockTextViewModel : ITextViewModel
    {
        private const string ExampleText =
            "It was a dark and stormy night; the rain fell in torrents — except at occasional intervals," +
            " when it was checked by a violent gust of wind which swept up the streets " +
            "(for it is in London that our scene lies), rattling along the housetops, " +
            "and fiercely agitating the scanty flame of the lamps that struggled against the darkness.";

        public MockTextViewModel(int fontScale, string text)
        {
            FontScale = fontScale;
            Text = text;

            DecreaseFontScale = ReactiveCommand.Create(() => { }, Observable.Return(false));
            IncreaseFontScale = ReactiveCommand.Create(() => { }, Observable.Return(true));
        }

        public MockTextViewModel() : this(100, ExampleText)
        {
        }

        public int FontScale { get; }

        public string Text { get; set; }
        public ReactiveCommand<Unit, Unit> IncreaseFontScale { get; }
        public ReactiveCommand<Unit, Unit> DecreaseFontScale { get; }
    }
}