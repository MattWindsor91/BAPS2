using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace URY.BAPS.Client.Wpf
{
    /// <summary>
    ///     Attached property for adding the ability to open a context menu on left-click (rather than the usual
    ///     right-click) to a UI element.
    ///     <para>
    ///         Based on <a href="https://stackoverflow.com/a/29123964">a StackOverflow answer</a>.
    ///     </para>
    /// </summary>
    public static class ContextMenuLeftClickBehavior
    {
        public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty.RegisterAttached(
            "IsLeftClickEnabled",
            typeof(bool),
            typeof(ContextMenuLeftClickBehavior),
            new UIPropertyMetadata(false, OnIsLeftClickEnabledChanged));

        public static bool GetIsLeftClickEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsLeftClickEnabledProperty);
        }

        public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLeftClickEnabledProperty, value);
        }

        private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UIElement uiElement)) return;

            var isEnabled = e.NewValue is bool value && value;
            if (isEnabled)
            {
                if (uiElement is ButtonBase button)
                    button.Click += OnMouseLeftButtonUp;
                else
                    uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
            }
            else
            {
                if (uiElement is ButtonBase button)
                    button.Click -= OnMouseLeftButtonUp;
                else
                    uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;
            if (fe.ContextMenu == null) return;
            if (fe.ContextMenu.DataContext == null)
                fe.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding {Source = fe.DataContext});

            fe.ContextMenu.IsOpen = true;
        }
    }
}