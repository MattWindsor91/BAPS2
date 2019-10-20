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
        private const int InitialFontScale = 100;
        private const int MaximumFontScale = 200;

        private event EventHandler<TextSettingDirection> ChangeFontSize;
        
        /// <summary>
        ///     Constructs a <see cref="TextViewModel" />.
        /// </summary>
        /// <param name="eventFeed">
        ///     The <see cref="IFullEventFeed" /> to which this view model
        ///     subscribes for text-property updates.
        /// </param>
        public TextViewModel(IFullEventFeed? eventFeed = null)
        {
            eventFeed ??= new EmptyEventFeed();


            var textLoads =
                from t in eventFeed.ObserveTrackLoad
                where t.Track.IsTextItem
                select t.Track.Text;
            _text = textLoads.ToProperty(this, x => x.Text, "");

            var externalFontSizeChanges =
                from x in eventFeed.ObserveTextSetting
                where x.Setting == TextSetting.FontSize
                select x.Direction;

            var internalFontSizeChanges =
                from x in Observable.FromEventPattern<TextSettingDirection>(x => ChangeFontSize += x,
                    x => ChangeFontSize -= x)
                select x.EventArgs;

            var fontSizeChanges = internalFontSizeChanges.Merge(externalFontSizeChanges);

            _fontScale =
                fontSizeChanges.Scan(InitialFontScale, UpdateFontSize).ToProperty(this, x => x.FontScale, InitialFontScale);
            
            DecreaseFontScale = ReactiveCommand.Create(DecreaseFontScaleImpl, CanDecreaseTextSize);
            IncreaseFontScale = ReactiveCommand.Create(IncreaseFontScaleImpl, CanIncreaseTextSize);
        }

        private ObservableAsPropertyHelper<int> _fontScale;

        /// <summary>
        ///     The font scale, in percent.
        /// </summary>
        public int FontScale => _fontScale.Value;

        private ObservableAsPropertyHelper<string> _text;

        /// <summary>
        ///     The text stored in the text panel.
        ///     <para>
        ///         This may be changed by the user, but will be overridden if
        ///         the server loads a text item.
        ///     </para>
        /// </summary>
        public string Text => _text.Value;

        private static int UpdateFontSize(int current, TextSettingDirection direction)
        {
            return direction switch
            {
                TextSettingDirection.Down => Math.Max(MinimumFontScale, current - 10),
                TextSettingDirection.Up => Math.Min(MaximumFontScale, current + 10),
                _ => current
            };
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
            ChangeFontSize?.Invoke(this, TextSettingDirection.Up);
        }

        private void DecreaseFontScaleImpl()
        {
            ChangeFontSize?.Invoke(this, TextSettingDirection.Down);
        }

        private IObservable<bool> CanIncreaseTextSize =>
            this.WhenAnyValue(x => x.FontScale, scale => scale < MaximumFontScale);

        private IObservable<bool> CanDecreaseTextSize =>
            this.WhenAnyValue(x => x.FontScale, scale => MinimumFontScale < scale);

        #endregion Commands

        public override void Dispose()
        {
            _fontScale.Dispose();
            _text.Dispose();
            DecreaseFontScale.Dispose();
            IncreaseFontScale.Dispose();
            base.Dispose();
        }
    }
}