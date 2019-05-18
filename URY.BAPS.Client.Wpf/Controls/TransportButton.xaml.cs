namespace URY.BAPS.Client.Wpf.Controls
{
    /// <summary>
    ///     Interaction logic for TransportButton.xaml
    /// </summary>
    public partial class TransportButton
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(TransportButton), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(TransportButton), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(TransportButton),
            new PropertyMetadata(SystemColors.HighlightBrush));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(FontAwesomeIcon), typeof(TransportButton), new PropertyMetadata(FontAwesomeIcon.Question));

        public TransportButton()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public bool IsHighlighted
        {
            get => (bool) GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        public Brush HighlightBrush
        {
            get => (Brush) GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public FontAwesomeIcon Icon
        {
            get => (FontAwesomeIcon) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}