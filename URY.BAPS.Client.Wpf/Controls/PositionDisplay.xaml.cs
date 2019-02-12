using System.Windows;
using System.Windows.Controls;

namespace URY.BAPS.Client.Wpf.Controls
{
    /// <summary>
    ///     Interaction logic for PositionDisplay.xaml
    /// </summary>
    public partial class PositionDisplay
    {
        public static DependencyProperty SubTextProperty =
            DependencyProperty.Register("SubText", typeof(string), typeof(PositionDisplay));

        public static DependencyProperty MainTextProperty =
            DependencyProperty.Register("MainText", typeof(string), typeof(PositionDisplay));

        public PositionDisplay()
        {
            InitializeComponent();
        }

        public string SubText
        {
            get => (string) GetValue(SubTextProperty);
            set => SetValue(SubTextProperty, value);
        }

        public string MainText
        {
            get => (string) GetValue(MainTextProperty);
            set => SetValue(MainTextProperty, value);
        }
    }
}