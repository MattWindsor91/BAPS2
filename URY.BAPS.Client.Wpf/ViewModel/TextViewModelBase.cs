using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public abstract class TextViewModelBase : SubscribingViewModel, ITextViewModel
    {
        private RelayCommand? _increaseTextSizeCommand;
        private RelayCommand? _decreaseTextSizeCommand;

        /// <summary>
        ///     The font scale, in percent.
        /// </summary>
        public abstract int FontScale { get; }

        /// <summary>
        ///     The text stored in the text panel.
        ///     <para>
        ///         This may be changed by the user, but will be overridden if
        ///         the server loads a text item.
        ///     </para>
        /// </summary>
        public abstract string Text { get; set; }

        /// <summary>
        ///     A command that, when invoked, increases the text size.
        /// </summary>
        public RelayCommand IncreaseTextSizeCommand => _increaseTextSizeCommand ??= new RelayCommand(
            IncreaseTextSize,
            CanIncreaseTextSize);

        /// <summary>
        ///     A command that, when invoked, increases the text size.
        /// </summary>
        public RelayCommand DecreaseTextSizeCommand => _decreaseTextSizeCommand ??= new RelayCommand(
            DecreaseTextSize,
            CanDecreaseTextSize);

        protected abstract void AdjustTextSize(TextSettingDirection direction);
        protected void IncreaseTextSize() => AdjustTextSize(TextSettingDirection.Up);
        protected abstract bool CanIncreaseTextSize();
        protected void DecreaseTextSize() => AdjustTextSize(TextSettingDirection.Down);
        protected abstract bool CanDecreaseTextSize();
    }
}