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
using System.Windows.Shapes;

namespace alcwin
{
    /// <summary>
    /// Interaktionslogik für LogSelectDialog.xaml
    /// </summary>
    public partial class LogSelectDialog : Window
    {
        public (int,int)[] Info;
        public int Selection;
        public int Channel;

        public class LogItem
        {
            public int Index { get; set; }
            public int Position { get; set; }
            public int Records { get; set; }
            public string Time
            {
                get
                {
                    int t = Records * 5;
                    int h = t / 3600;
                    int m = t / 60 % 60;
                    int s = t % 60;
                    return String.Format("{0:d}:{1:d2}:{2:d2}", h, m, s);
                }
            }
        }

        public LogSelectDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Info == null)
                return;

            var items = new LogItem[Info.Length];
            for (int i=0; i<Info.Length; i++)
            {
                items[i] = new LogItem
                {
                    Index = i + 1,
                    Position = Info[i].Item1,
                    Records = Info[i].Item2 - Info[i].Item1 - 3
                };
            }
            LogList.ItemsSource = items;

            if (items.Length > 0)
                LogList.SelectedIndex = items.Length - 1;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var idx = LogList.SelectedIndex;

            if (idx>=0 && idx<Info.Length)
            {
                Selection = idx;
                DialogResult = true;
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(delegate {
                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        con.DeleteLog(Channel);
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                });
                DialogResult = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
