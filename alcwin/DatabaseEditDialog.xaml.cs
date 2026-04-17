using alclib;
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
    /// Interaktionslogik für DatabaseEditDialog.xaml
    /// </summary>
    public partial class DatabaseEditDialog : Window
    {
        public DatabaseEditDialog()
        {
            InitializeComponent();
        }

        public DbEntry Parameter
        {
            get { return ParameterControl.GetDbEntry(); }
            set { ParameterControl.SetDbEntry(value); }
        }

        private async void FromChannel_Click(object sender, RoutedEventArgs e)
        {
            int channel = Convert.ToInt32(((FrameworkElement)sender).Tag);
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var p = await Task.Run(delegate {
                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        return LogConvert.Parse<ChannelParameter>(con.GetParameter(channel));
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                });

                ParameterControl.Import(p);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
