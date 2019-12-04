using System;
using System.Collections.Generic;
using System.Windows;

namespace OneCode.VsExtension.Windows
{
    /// <summary>
    /// Interaction logic for RepositoriesControl.xaml
    /// </summary>
    public partial class RepositoriesControl
    {
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(IEnumerable<string>), typeof(RepositoriesControl), new FrameworkPropertyMetadata(
                    Array.Empty<string>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValuesChangedCallback));

        public IEnumerable<string> Values {
            get => (IEnumerable<string>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }

        private static void OnValuesChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is RepositoriesControl control)
            {
                control.ListView.ItemsSource = args.NewValue as IEnumerable<string>;
            }
        }

        public RepositoriesControl()
        {
            InitializeComponent();
        }
    }
}
