using alclib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using static alclib.AlcConnection;

namespace alcwin
{
    /// <summary>
    /// Interaktionslogik für ChannelLogWindow.xaml
    /// </summary>
    public partial class ChannelLogWindow : Window
    {
        public LogData Log;

        public static readonly DependencyProperty ChannelProperty;

        public static readonly RoutedUICommand GraphCommand;
        public static readonly RoutedUICommand TextCommand;

        public class ListRecord
        {
            public int Index { get; set; }
            public int Voltage { get; set; }
            public int Current { get; set; }
            public int Capacity { get; set; }
        }

        static ChannelLogWindow()
        {
            ChannelProperty = DependencyProperty.Register("Channel", typeof(int), typeof(ChannelLogWindow));
            GraphCommand = new RoutedUICommand("Graph", "Graph", typeof(ChannelLogWindow));
            TextCommand = new RoutedUICommand("Text", "Text", typeof(ChannelLogWindow));
        }

        public int Channel
        {
            get { return (int)GetValue(ChannelProperty); }
            set { SetValue(ChannelProperty, value); }
        }

        public ChannelLogWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Chart.SetData(Log);

            Program.Text = DisplayConvert.Function(Log.Header.Program);
            BatType.Text = DisplayConvert.BatType(Log.Header.BatType);
            Voltage.Text = DisplayConvert.Voltage(Log.Header.BatType, Log.Header.Cells);
            Capacity.Text = DisplayConvert.Capacity(Log.Header.Capacity);
            ChargeCurrent.Text = DisplayConvert.Current(Log.Header.ChargeCurrent);
            FormingCurrent.Text = DisplayConvert.Current(Log.Header.FormingCurrent);
            DischargeCurrent.Text = DisplayConvert.Current(Log.Header.DischargeCurrent);
            Pause.Text = LogConvert.Pause(Log.Header.Pause);

            var l = new List<ListRecord>();
            foreach(var s in Log.Sections)
            {
                int idx = 0;
                foreach(var r in s.Data)
                {
                    var lr = new ListRecord();
                    lr.Index = idx++;
                    lr.Voltage = r.Voltage;
                    lr.Current = r.Current;
                    lr.Capacity = r.Capacity;
                    l.Add(lr);
                }
            }

            Records.ItemsSource = l;
        }

        private void Graph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Records.Visibility = Visibility.Hidden;
            Chart.Visibility = Visibility.Visible;
        }

        private void Text_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Chart.Visibility = Visibility.Hidden;
            Records.Visibility = Visibility.Visible;
        }
    }
}
