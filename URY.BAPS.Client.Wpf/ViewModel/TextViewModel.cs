using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     A view model that represents the text panel, and its various
    ///     configurable aspects.
    /// </summary>
    class TextViewModel : ViewModelBase, ITextViewModel
    {
        [NotNull] private readonly SystemController _controller;

        /// <summary>
        ///     Constructs a <see cref="TextViewModel"/>.
        /// </summary>
        /// <param name="controller">
        ///     The <see cref="SystemController"/> used to translate text-panel
        ///     actions into server requests.
        /// </param>
        /// <param name="updater">
        ///     The <see cref="ISystemServerUpdater"/> to which this view model
        ///     subscribes for text-property updates.
        /// </param>
        public TextViewModel([CanBeNull] SystemController controller, [CanBeNull] IClientCore updater)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            SubscribeToServerUpdates(updater);
        }

        private string _text;

        private double _fontSize;

        /// <summary>
        ///     The font size.
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            private set
            {
                if (_fontSize.Equals(value)) return;
                if (_fontSize < 0) return;
                _fontSize = value;
                RaisePropertyChanged(nameof(FontSize));
            }
        }

        /// <summary>
        ///     The text stored in the text panel.
        ///     <para>
        ///         This may be changed by the user, but will be overridden if
        ///         the server loads a text item.
        ///     </para>
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        private void SubscribeToServerUpdates(IServerUpdater updater)
        {
            updater.ObserveTrackLoad.Subscribe(OnTrackLoad);
            updater.ObserveTextSetting.Subscribe(OnTextSetting);
        }

        private void AdjustFontSize(UpDown direction)
        {
            var delta = direction == UpDown.Up ? 1 : -1;
            DispatcherHelper.CheckBeginInvokeOnUI(() => FontSize += delta);
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
        private void OnTextSetting(TextSettingEventArgs args)
        {
            switch (args.Setting)
            {
                case TextSetting.FontSize:
                    AdjustFontSize(args.Direction);
                    break;
                case TextSetting.Scroll:
                    // TODO(@MattWindsor91): handle scroll somehow.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
