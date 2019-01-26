using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for TransportButton.xaml
    /// </summary>
    public partial class TransportButton
    {
        public TransportButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(TransportButton), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(TransportButton), new PropertyMetadata(default(bool)));

        public bool IsHighlighted
        {
            get => (bool) GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(TransportButton), new PropertyMetadata(SystemColors.HighlightBrush));

        public Brush HighlightBrush
        {
            get => (Brush) GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(FontAwesomeIcon), typeof(TransportButton), new PropertyMetadata(FontAwesomeIcon.Question));

        public FontAwesomeIcon Icon
        {
            get => (FontAwesomeIcon) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
