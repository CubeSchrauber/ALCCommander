using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Shapes;

namespace alcwin
{
    public partial class SerialDialog : Window
    {
        public bool Network { get; private set; }

        public SerialDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            PortSelection.ItemsSource = ports;
            if ((ports?.Length ?? 0) > 0)
                PortSelection.SelectedIndex = 0;

            if (!String.IsNullOrWhiteSpace(App.Settings.Hostname))
                Hostname.Text = App.Settings.Hostname;
        }

        private void Serial_Click(object sender, RoutedEventArgs e)
        {
            Network = false;
            DialogResult = true;
        }

        public string Port
        {
            get
            {
                return Network ? Hostname.Text : PortSelection.SelectedItem.ToString();
            }
        }

        private void Network_Click(object sender, RoutedEventArgs e)
        {
            Network = true;
            DialogResult = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Settings.Hostname = Hostname.Text;
        }
    }
}
