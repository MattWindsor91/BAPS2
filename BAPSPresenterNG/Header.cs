using System.Windows;
using System.Windows.Controls;
using FontAwesome.WPF;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     A control used for the headers of BAPS channels and directories.
    ///     <para>
    ///         The header control contains three elements:
    ///         <list type="bullet">
    ///             <item>a FontAwesome icon (<see cref="Icon" />);</item>
    ///             <item>a text legend (<see cref="Text" />);</item>
    ///             <item>
    ///                 a content section intended for buttons or icons
    ///                 (the control's body).
    ///             </item>
    ///         </list>
    ///     </para>
    /// </summary>
    public class Header : ContentControl
    {
        static Header()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Header), new FrameworkPropertyMetadata(typeof(Header)));
        }

        #region DependencyProperty Text

        /// <summary>
        ///     Registers a dependency property as backing store for the Text property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Header),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion DependencyProperty Text

        #region DependencyProperty Icon

        /// <summary>
        ///     Registers a dependency property as backing store for the Icon property
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(FontAwesomeIcon), typeof(Header),
                new FrameworkPropertyMetadata(FontAwesomeIcon.Question,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public FontAwesomeIcon Icon
        {
            get => (FontAwesomeIcon) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion DependencyProperty Icon
    }
}