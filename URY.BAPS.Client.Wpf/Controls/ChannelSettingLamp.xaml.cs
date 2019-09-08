using System.Windows;
using FontAwesome5;

namespace URY.BAPS.Client.Wpf.Controls
{
    /// <summary>
    ///     A user control that gives a read-only projection of some
    ///     setting on a BAPS channel by displaying an icon with a 'lit' or
    ///     'not lit' visual effect.
    /// </summary>
    public partial class ChannelSettingLamp
    {
        public ChannelSettingLamp()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The default icon to show if the user doesn't bind the icon.
        /// </summary>
        private const EFontAwesomeIcon DefaultIcon = EFontAwesomeIcon.Solid_QuestionCircle;

        /// <summary>
        ///     The icon to display on the lamp.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(EFontAwesomeIcon), typeof(ChannelSettingLamp), new PropertyMetadata(DefaultIcon));

        /// <summary>
        ///     Code-level property for <see cref="IconProperty"/>.
        /// </summary>
        public EFontAwesomeIcon Icon
        {
            get => (EFontAwesomeIcon) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        ///     Whether the lamp is enabled (lit) or not.
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(ChannelSettingLamp), new PropertyMetadata(default(bool)));

        /// <summary>
        ///     Code-level property for <see cref="IsHighlightedProperty"/>.
        /// </summary>
        public bool IsHighlighted
        {
            get => (bool) GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        /// <summary>
        ///     Whether the lamp should show an alternative status when lit.
        ///     <para>
        ///         Visually, this should make the lamp glow a different
        ///         colour.
        ///     </para>
        /// </summary>
        public static readonly DependencyProperty AlternativeModeProperty = DependencyProperty.Register(
            "AlternativeMode", typeof(bool), typeof(ChannelSettingLamp), new PropertyMetadata(default(bool)));

        /// <summary>
        ///     Code-level property for <see cref="AlternativeModeProperty"/>.
        /// </summary>
        public bool AlternativeMode
        {
            get => (bool) GetValue(AlternativeModeProperty);
            set => SetValue(AlternativeModeProperty, value);
        }
    }
}
