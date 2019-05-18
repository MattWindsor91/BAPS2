namespace URY.BAPS.Client.Wpf.Converters
{
    internal class SliderValueChangedEventArgsToUintConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            if (!(value is RoutedPropertyChangedEventArgs<double> args)) return 0;
            return (uint) args.NewValue;
        }
    }
}