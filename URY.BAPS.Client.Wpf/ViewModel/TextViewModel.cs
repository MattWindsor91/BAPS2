using System;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;
using ArgumentNullException = System.ArgumentNullException;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     A view model that represents the text panel, and its various
    ///     configurable aspects.
    /// </summary>
    public class TextViewModel : TextViewModelBase
    {
        private const int MinimumFontScale = 50;
        private const int MaximumFontScale = 200;

        private int _fontScale = 100;

        private string _text = "";

        /// <summary>
        ///     Constructs a <see cref="TextViewModel" />.
        /// </summary>
        /// <param name="eventFeed">
        ///     The <see cref="IFullEventFeed" /> to which this view model
        ///     subscribes for text-property updates.
        /// </param>
        public TextViewModel(IFullEventFeed? eventFeed)
        {
            SubscribeToServerUpdates(eventFeed ?? throw new ArgumentNullException(nameof(eventFeed)));
        }

        /// <summary>
        ///     The font scale, in percent.
        /// </summary>
        public override int FontScale => _fontScale;

        /// <summary>
        ///     The text stored in the text panel.
        ///     <para>
        ///         This may be changed by the user, but will be overridden if
        ///         the server loads a text item.
        ///     </para>
        /// </summary>
        public override string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        private void SubscribeToServerUpdates(IFullEventFeed updater)
        {
            SubscribeTo(updater.ObserveTrackLoad, OnTrackLoad);
            SubscribeTo(updater.ObserveTextSetting, OnTextSetting);
        }

        /// <summary>
        ///     Observes a track load from the server.
        ///     <para>
        ///         The text view model observes track loads so as to
        ///         handle text-track loads; it doesn't react to any other
        ///         track load.
        ///     </para>
        /// </summary>
        /// <param name="args">Information about the newly-loaded track.</param>
        private void OnTrackLoad(TrackLoadArgs args)
        {
            if (!args.Track.IsTextItem) return;
            Text = args.Track.Text;
        }

        /// <summary>
        ///     Observes a text setting change from the server.
        /// </summary>
        /// <param name="args">Information about the text setting change.</param>
        private void OnTextSetting(TextSettingArgs args)
        {
            switch (args.Setting)
            {
                case TextSetting.FontSize:
                    AdjustTextSize(args.Direction);
                    break;
                case TextSetting.Scroll:
                    // TODO(@MattWindsor91): handle scroll somehow.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Commands

        // These commands deliberately don't go through the system controller.
        // This is because the server doesn't actually understand text setting
        // changes; it can only send them, and only does so if a connected
        // BAPS paddle etc. requests it.

        protected override void AdjustTextSize(TextSettingDirection direction)
        {
            var delta = direction == TextSettingDirection.Up ? 10 : -10;
            DispatcherHelper.CheckBeginInvokeOnUI(() => SetFontScale(FontScale + delta));
        }

        private void SetFontScale(int value)
        {
            value = Math.Max(value, MinimumFontScale);
            value = Math.Min(value, MaximumFontScale);
            if (_fontScale.Equals(value)) return;
            _fontScale = value;
            RaisePropertyChanged(nameof(FontScale));
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IncreaseTextSizeCommand.RaiseCanExecuteChanged();
                DecreaseTextSizeCommand.RaiseCanExecuteChanged();
            });
        }

        protected override bool CanIncreaseTextSize()
        {
            return FontScale < MaximumFontScale;
        }

        protected override bool CanDecreaseTextSize()
        {
            return MinimumFontScale < FontScale;
        }

        #endregion Commands
    }
}