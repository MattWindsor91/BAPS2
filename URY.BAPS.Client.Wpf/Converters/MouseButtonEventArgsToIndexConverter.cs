namespace URY.BAPS.Client.Wpf.Converters
{
    /// <summary>
    ///     Converts a <see cref="MouseButtonEventArgs" /> over a
    ///     <see cref="ListBox" /> into an integer representing the index of any item under the mouse
    ///     at the time of the button event.
    /// </summary>
    public class MouseButtonEventArgsToIndexConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var args = (MouseButtonEventArgs) value;
            var list = (ListBox) parameter;
            return IndexFromPoint(args, list);
        }
        // https://stackoverflow.com/questions/6961963/wpf-listbox-indexfrompoint

        private static int IndexFromPoint(MouseButtonEventArgs e, ListBox listBox)
        {
            for (var i = 0; i < listBox.Items.Count; i++)
            {
                if (!(listBox.ItemContainerGenerator.ContainerFromIndex(i) is ListBoxItem lbi)) continue;
                if (IsMouseOverTarget(lbi, e.GetPosition(lbi))) return i;
            }

            return -1;
        }

        private static bool IsMouseOverTarget(Visual target, Point point)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(point);
        }
    }
}