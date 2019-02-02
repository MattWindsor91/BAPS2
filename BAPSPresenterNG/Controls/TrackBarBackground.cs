using System.Windows;

namespace BAPSPresenterNG.Controls
{
    public class TrackBarBackground : FrameworkElement
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration", typeof(uint), typeof(TrackBarBackground), new PropertyMetadata(default(uint)));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(uint), typeof(TrackBarBackground), new PropertyMetadata(default(uint)));

        public uint Duration
        {
            get => (uint) GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public uint Position
        {
            get => (uint) GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }
    }
}