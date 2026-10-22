using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WPFSamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ListBox.SelectionChanged += (sender, args) =>
            {
                this.Flyout.IsOpen = this.ListBox.SelectedItem != null;
                this.TopFlyout.IsOpen = false;
            };

            this.LoadSamples();
        }

        private async void LoadSamples()
        {
            this.DataContext = await Sample.LoadFromCurrentAssemblyAsync();

            await Task.Delay(50);

            this.ListBox.SelectedItem = null;

            this.TopFlyout.IsOpen = true;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var hyperLink = sender as Hyperlink;
            Process.Start(hyperLink.NavigateUri.AbsoluteUri);
        }

        private void LaunchSample_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var sample = button.DataContext as Sample;
    
            var window = Activator.CreateInstance(sample.WindowType) as SampleWindow;
            if (window != null)
            {
                if (!string.IsNullOrEmpty(sample.RelatedUrl))
                {
                    window.Source = new Uri(sample.RelatedUrl);
                }
                    
                window.Show();
            }
            
        }
    }
}
