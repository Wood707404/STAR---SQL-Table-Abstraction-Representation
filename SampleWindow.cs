using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows.Media;

namespace WPFSamples
{
    public class SampleWindow : MetroWindow
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof (Uri), typeof (SampleWindow), new PropertyMetadata(default(Uri)));

        public SampleWindow()
        {
            // It's a non standard practice to set UI properties in code. 
            // This is only to workaround the limitation described in
            // http://stackoverflow.com/questions/8604086/setting-a-local-implicit-style-different-from-theme-style-alternative-to-based
            this.RightWindowCommands = Application.Current.FindResource("SampleWindowCommands") as WindowCommands;
            this.OpenSourceUrlCommand = new Command(() => Process.Start(this.Source.AbsoluteUri));
            this.BorderThickness = new Thickness(0);
            this.GlowBrush = Brushes.Black;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public Uri Source
        {
            get { return (Uri) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public ICommand OpenSourceUrlCommand { get; private set; }
    }
}