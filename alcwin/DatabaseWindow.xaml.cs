using alclib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class DatabaseWindow : Window
    {
        public ObservableCollection<DbEntry> Data { get; set; }

        //public static readonly RoutedUICommand EditCommand;
        public static readonly DependencyProperty EditCommandProperty;

        public event Action<ChannelParameter> ChannelSetEvent;

        private class Edit : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private DatabaseWindow Parent;

            public Edit(DatabaseWindow p) => Parent = p;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter) => Parent.EditItem(parameter as DbEntry);
        }


        static DatabaseWindow()
        {
            //EditCommand = new RoutedUICommand("Edit", "Edit", typeof(DatabaseWindow));
            EditCommandProperty = DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(DatabaseWindow));
        }

        public DatabaseWindow()
        {
            InitializeComponent();
            EditCommand = new Edit(this);
            DbList.DataContext = this;
        }

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DbList.ItemsSource = Data;
        }

        public async void EditItem(DbEntry selection)
        {
            if (selection != null)
            {
                int idx = Data.IndexOf(selection);
                var d = new DatabaseEditDialog();
                d.Owner = this;
                d.Parameter = selection;
                if (d.ShowDialog() ?? false)
                {
                    var e = d.Parameter;
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        e = await Task.Run(delegate (){
                            var con = App.ConnectionHolder.GetConnection(true);
                            try
                            {
                                return LogConvert.Parse<DbEntry>(con.Cmd(e.GetBytes()));
                            }
                            finally
                            {
                                App.ConnectionHolder.ReleaseConnection();
                            }
                        });
                        Data[idx] = e;
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
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            int channel = Convert.ToInt32(((FrameworkElement)sender).Tag);
            EditItem(DbList.SelectedItem as DbEntry);
        }

        private async void ToChannel_Click(object sender, RoutedEventArgs e)
        {
            int channel = Convert.ToInt32(((FrameworkElement)sender).Tag);
            var selection = DbList.SelectedItem as DbEntry;
            if (selection!=null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    var q = await Task.Run(delegate {
                        var con = App.ConnectionHolder.GetConnection(true);
                        try
                        {
                            var p = new ChannelParameter();
                            p.Channel = channel;
                            p.BatIndex = 40;// selection.Index;
                            p.BatType = selection.BatType;
                            p.Cells = selection.Cells;
                            p.DisCurrent = selection.DisCurrent;
                            p.ChargeCurrent = selection.ChargeCurrent;
                            p.Capacity = selection.Capacity;
                            p.Program = 1;
                            p.FormingCurrent = selection.ChargeCurrent;
                            p.Pause = selection.Pause;
                            p.Flags = selection.Flags;
                            p.ChargeFactor = selection.ChargeFactor;
                            return LogConvert.Parse<ChannelParameter>(con.Cmd(p.GetBytes()));
                        }
                        finally
                        {
                            App.ConnectionHolder.ReleaseConnection();
                        }
                    });

                    if (q != null && ChannelSetEvent != null)
                        ChannelSetEvent(q);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
    }
}
