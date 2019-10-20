using System;
using System.ComponentModel.Design;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     A view model that represents the text panel, and its various
    ///     configurable aspects.
    /// </summary>
    public class TextViewModel : ViewModelBase, ITextViewModel
    {
        private const int MinimumFontScale = 50;
        private const int MaximumFontScale = 200;

        private int _fontScale = 100;

        /// <summary>
        ///     Constructs a <see cref="TextViewModel" />.
        /// </summary>
        /// <param name="eventFeed">
        ///     The <see cref="IFullEventFeed" /> to which this view model
        ///     subscribes for text-property updates.
        /// </param>
        public TextViewModel(IFullEventFeed? eventFeed = null)
        {
            DecreaseFontScale = ReactiveCommand.Create(DecreaseFontScaleImpl, CanDecreaseTextSize);
            IncreaseFontScale = ReactiveCommand.Create(IncreaseFontScaleImpl, CanIncreaseTextSize);

            if (! (eventFeed is null)) SubscribeToServerUpdates(eventFeed);
        }

        /// <summary>
        ///     The font scale, in percent.
        /// </summary>
        public int FontScale
        {
            get => _fontScale;
            private set => this.RaiseAndSetIfChanged(ref _fontScale, value);
        }

        private ObservableAsPropertyHelper<string> _text;

        /// <summary>
        ///     The text stored in the text panel.
        ///     <para>
        ///         This may be changed by the user, but will be overridden if
        ///         the server loads a text item.
        ///     </para>
        /// </summary>
        public string Text => _text.Value;

        private void SubscribeToServerUpdates(IFullEventFeed updater)
        {
            var textLoads =
                from t in updater.ObserveTrackLoad
                where t.Track.IsTextItem
                select t.Track.Text;
            _text = textLoads.ToProperty(this, x => x.Text, "");

            var fontSizeChanges =
                updater.ObserveTextSetting.Where(x => x.Setting == TextSetting.FontSize);
            AddSubscription(fontSizeChanges.Where(x => x.Direction == TextSettingDirection.Up)
                .InvokeCommand(IncreaseFontScale));
            AddSubscription(fontSizeChanges.Where(x => x.Direction == TextSettingDirection.Down)
                    .InvokeCommand(DecreaseFontScale));
        }

        #region Commands

        // These commands deliberately don't go through the system controller.
        // This is because the server doesn't actually understand text setting
        // changes; it can only send them, and only does so if a connected
        // BAPS paddle etc. requests it.

        /// <summary>
        ///     A command that, when invoked, increases the text size.
        /// </summary>
        public ReactiveCommand<Unit, Unit> IncreaseFontScale { get; }

        /// <summary>
        ///     A command that, when invoked, increases the text size.
        /// </summary>
        public ReactiveCommand<Unit, Unit> DecreaseFontScale { get; }

        private void IncreaseFontScaleImpl()
        {
            FontScale += 10;
        }

        private void DecreaseFontScaleImpl()
        {
            FontScale -= 10;
        }

        private IObservable<bool> CanIncreaseTextSize =>
            this.WhenAnyValue(x => x.FontScale, scale => scale < MaximumFontScale);

        private IObservable<bool> CanDecreaseTextSize =>
            this.WhenAnyValue(x => x.FontScale, scale => MinimumFontScale < scale);

        #endregion Commands

        public override void Dispose()
        {
            _text.Dispose();
            DecreaseFontScale.Dispose();
            IncreaseFontScale.Dispose();
            base.Dispose();
        }
    }
}