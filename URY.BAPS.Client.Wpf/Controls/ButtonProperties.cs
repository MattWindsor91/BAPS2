using System;
using System.Collections.Generic;
using System.Windows;
using FontAwesome5;

namespace URY.BAPS.Client.Wpf.Controls
{
    /// <summary>
    ///     Attached properties used to attach icons and other such things to
    ///     buttons.
    /// </summary>
    public static class ButtonProperties
    {
        #region Icon property

        public static EFontAwesomeIcon GetIcon(DependencyObject obj)
        {
            return (EFontAwesomeIcon) obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, EFontAwesomeIcon value)
        {
            obj.SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(EFontAwesomeIcon), typeof(ButtonProperties),
                new UIPropertyMetadata(EFontAwesomeIcon.Solid_QuestionCircle));

        #endregion Icon property
    }
}