using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.Command;
using URY.BAPS.Client.Wpf.Properties;

namespace URY.BAPS.Client.Wpf.Converters
{
    /// <summary>
    ///     Converts a <see cref="SelectionChangedEventArgs" /> over a
    ///     <see cref="Selector" /> into an integer representing the index of any selected item.
    /// </summary>
    public class SelectionChangedEventArgsToIndexConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            if (!(value is SelectionChangedEventArgs args))
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    string.Format(
                        Resources.SelectionChangedEventArgsToIndexConverter_Convert_NotSelectionChangedEventArgs,
                        nameof(SelectionChangedEventArgs)));
            if (!(args.Source is Selector sel))
                throw new ArgumentOutOfRangeException(nameof(args.Source), args.Source,
                    string.Format(Resources.SelectionChangedEventArgsToIndexConverter_Convert_SourceNotSelector,
                        nameof(Selector))
                );
            return sel.SelectedIndex;
        }
    }
}