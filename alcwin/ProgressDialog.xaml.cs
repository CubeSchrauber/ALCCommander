using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace alcwin
{
    /// <summary>
    /// Interaktionslogik für ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public bool CanClose;
        public readonly ManualResetEvent LoadCompleted;

        public static readonly DependencyProperty ProgressProperty;
        public static readonly DependencyProperty ImageSourceProperty;

        static ProgressDialog()
        {
            ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(ProgressDialog));
            ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ProgressDialog));
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public ProgressDialog()
        {
            LoadCompleted = new ManualResetEvent(false);
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CanClose;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCompleted.Set();
        }

        public void ReportProgress(double value)
        {
            Dispatcher.Invoke(delegate { this.Progress = value; });
        }
    }
}
