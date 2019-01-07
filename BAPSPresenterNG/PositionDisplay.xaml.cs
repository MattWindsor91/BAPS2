﻿
using System.Windows;
using System.Windows.Controls;


namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for PositionDisplay.xaml
    /// </summary>
    public partial class PositionDisplay : UserControl
    {
        public PositionDisplay()
        {
            InitializeComponent();
        }

        public string SubText
        {
            get { return (string)GetValue(SubTextProperty); }
            set { SetValue(SubTextProperty, value); }
        }

        public string MainText
        {
            get { return (string)GetValue(MainTextProperty); }
            set { SetValue(MainTextProperty, value); }
        }

        public static DependencyProperty SubTextProperty =
           DependencyProperty.Register("SubText", typeof(string), typeof(PositionDisplay));

        public static DependencyProperty MainTextProperty =
           DependencyProperty.Register("MainText", typeof(string), typeof(PositionDisplay));
    }
}