using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace URY.BAPS.Client.Wpf.Converters
{
    class SliderValueChangedEventArgsToUintConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            if (!(value is RoutedPropertyChangedEventArgs<double> args)) return 0;
            return (uint) args.NewValue;
        }
    }
}
