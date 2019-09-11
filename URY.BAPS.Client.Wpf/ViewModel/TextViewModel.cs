using System;
using GalaSoft.MvvmLight;
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
    internal class TextViewModel : ViewModelBase, ITextViewModel
    {
        [NotNull] private readonly SystemController _controller;

        private const int MinimumFontScale = 50;
        private const int MaximumFontScale = 200;

        private int _fontScale = 100;

        private string _text = "";

        /// <summary>
        ///     Constructs a <see cref="TextViewModel" />.
        /// </summary>
        /// <param name="controller">
        ///     The <see cref="SystemController" /> used to translate text-panel
        ///     actions into server requests.
        /// </param>
        /// <param name="client">
        ///     The <see cref="ClientCore" /> to which this view model
        ///     subscribes for text-property updates.
        /// </param>
        public TextViewModel(SystemController? controller, ClientCore? client)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            SubscribeToServerUpdates(client?.EventFeed);
        }

        /// <summary>
        ///     The font scale, in percent.
        /// </summary>
        public int FontScale
        {
            get => _fontScale;
            private set
            {
                if (_fontScale.Equals(value)) return;
                if (_fontScale < MinimumFontScale) return;
                if (MaximumFontScale <= _fontScale) return;
                _fontScale = value;
                RaisePropertyChanged(nameof(FontScale));
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

        private void SubscribeToServerUpdates(IFullEventFeed? updater)
        {
            updater?.ObserveTrackLoad?.Subscribe(OnTrackLoad);
            updater?.ObserveTextSetting?.Subscribe(OnTextSetting);
        }

        private void AdjustFontSize(TextSettingDirection direction)
        {
            var delta = direction == TextSettingDirection.Up ? 1 : -1;
            DispatcherHelper.CheckBeginInvokeOnUI(() => FontScale += delta);
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