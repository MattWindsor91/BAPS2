using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;

namespace URY.BAPS.Client.Wpf.Converters
{
    public class MouseButtonEventArgsToIndexConverter : IEventArgsConverter
    {
        // https://stackoverflow.com/questions/6961963/wpf-listbox-indexfrompoint

        private static int IndexFromPoint(MouseButtonEventArgs e, ListBox listBox)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
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
        
        public object Convert(object value, object parameter)
        {
            var args = (MouseButtonEventArgs) value;
            var list = (ListBox) parameter;
            return IndexFromPoint(args, list);
        }
    }
}