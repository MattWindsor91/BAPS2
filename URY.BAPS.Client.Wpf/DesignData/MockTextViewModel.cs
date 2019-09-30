﻿using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A mock-up view model for the text pane.
    /// </summary>
    [UsedImplicitly]
    public sealed class MockTextViewModel : TextViewModelBase
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
        }

        public MockTextViewModel() : this(100, ExampleText)
        {
        }

        public override int FontScale { get; }

        public override string Text { get; set; }

        protected override void AdjustTextSize(TextSettingDirection delta)
        {
        }

        protected override bool CanIncreaseTextSize()
        {
            return false;
        }

        protected override bool CanDecreaseTextSize()
        {
            return true;
        }
    }
}