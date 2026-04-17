using alclib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace alcwin
{
    public partial class MainWindow : Window
    {
        private ChannelInfo[] ChannelInfo;
        private ChannelControl[] ChannelControl;

        private DispatcherTimer Timer;
        private DatabaseWindow DatabaseWindow;

        public MainWindow()
        {
            InitializeComponent();
            ChannelControl = new ChannelControl[] { Channel1, Channel2, Channel3, Channel4 };
            Timer = new DispatcherTimer(TimeSpan.FromSeconds(5), DispatcherPriority.Normal, Timer_Tick, Dispatcher.CurrentDispatcher);
        }

        private Task<T> Run<T>(Func<IAlcConnection, T> action, bool wait = false) where T:class
        {
            return Task.Run(delegate {
                var con = App.ConnectionHolder.GetConnection(wait);
                if (con != null)
                {
                    try
                    {
                        return action(con);
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                }
                else return null;
            });
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                var result = await Run(con => GetChannelInfo(con));
                if (result != null)
                    SetChannelInfo(result);
            }
            catch
            {
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await Run(con => GetChannelInfo(con));
                if (result != null)
                    SetChannelInfo(result);
            }
            catch
            {
            }
        }

        private void SetChannelInfo(ChannelInfo[] result)
        {
            ChannelInfo = result;
            for (int i = 0; i < 4; i++)
                ChannelControl[i].SetInfo(result[i]);
        }

        private ChannelInfo[] GetChannelInfo(IAlcConnection Connection)
        {
            var info = new ChannelInfo[4];
            for (int i = 0; i < 4; i++)
            {
                info[i] = new ChannelInfo();
                info[i].Parameter = LogConvert.Parse<ChannelParameter>(Connection.GetParameter(i));
                info[i].ChargeState = LogConvert.Parse<ChargeState>(Connection.GetChargeState(i));
                info[i].Metric = LogConvert.Parse<Metric>(Connection.GetMetric(i));
            }
            return info;
        }

        private async void Channel_Edit(int channel)
        {
            if (ChannelInfo == null)
                return;

            var info = ChannelInfo[channel];

            var d = new ParameterDialog();
            d.Owner = this;
            d.SetParameter(info.Parameter);

            if (d.ShowDialog()??false)
            {
                var p = d.GetParameter();
                p.Channel = channel;

                try
                {
                    p = await Run(con => LogConvert.Parse<ChannelParameter>(con.Cmd(p.GetBytes())), true);
                }
                catch
                {
                }

                ChannelControl[channel].SetParameter(p);
                ChannelInfo[channel].Parameter = p;
            }
        }

        private async void Channel_Start(int channel)
        {
            try
            {
                await Run(con => con.Start(channel), true);
            }
            catch
            {
            }
        }

        private async void Channel_Stop(int channel)
        {
            try
            {
                await Run(con => con.Stop(channel), true);
            }
            catch
            {

            }
        }

        private (int, int)[] GetLogInfo(IAlcConnection con, int channel)
        {
            var par = LogConvert.Parse<ChannelParameter>(con.GetParameter(channel));
            var info = con.GetLogInfo(channel);
            return LogConvert.GetLogInfo(par.LogEnd, info, 2);
        }

        private async Task<LogData> LoadLog(ProgressDialog progress, int channel, int start, int end)
        {
            try
            {
                return await Task.Run(delegate
                {
                    progress.LoadCompleted.WaitOne();

                    int first_block = start / 100;
                    int last_block = (end + 99) / 100;
                    int block_size = 800;

                    byte[] data = new byte[(last_block - first_block + 1) * block_size];

                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        for (int block = first_block; block <= last_block; block++)
                        {
                            byte[] b = con.GetLogBlock(channel, block%650);
                            Array.Copy(b, 4, data, (block - first_block) * block_size, block_size);
                            progress.ReportProgress((double)(block - first_block) / (double)(last_block - first_block) * 100.0);
                        }
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }

                    var (header, records) = LogConvert.GetLog(new alclib.Buffer(data), start - (first_block * 100), end - (first_block * 100));
                    var log = new LogData(header, records);
                    return log;
                });
            }
            finally
            {
                progress.CanClose = true;
                progress.Close();
            }
        }

        private async void Channel_Log(int channel)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            (int, int)[] info;
            try
            {
                info = await Run(con => GetLogInfo(con, channel), true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            var d = new LogSelectDialog();
            d.Owner = this;
            d.Info = info;
            d.Channel = channel;
            if (d.ShowDialog()??false)
            {
                var progress = new ProgressDialog();
                progress.Owner = this;
                progress.ImageSource = (ImageSource)FindResource("LogDrawingImage");
                var task = LoadLog(progress, channel, info[d.Selection].Item1, info[d.Selection].Item2);
                progress.ShowDialog();
                try
                {
                    var log = await task;
                    if (log != null)
                    {
                        var w = new ChannelLogWindow();
                        w.Channel = channel + 1;
                        w.Log = log;
                        w.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Timer.Stop();
        }

        private async Task<DbEntry[]> LoadDatabase(ProgressDialog progress)
        {
            try
            {
                return await Task.Run(delegate {
                    progress.LoadCompleted.WaitOne();

                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        DbEntry[] result = new DbEntry[40];
                        for (int i=0; i<result.Length; i++)
                        {
                            var data = con.GetDbEntry(i);
                            var entry = new DbEntry();
                            int pos = 0;
                            entry.Init(data, ref pos);
                            result[i] = entry;
                            progress.ReportProgress((double)i/(double)result.Length * 100.0);
                        }
                        return result;
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                });
            }
            finally
            {
                progress.CanClose = true;
                progress.Close();
            }
        }

        private async void Database_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseWindow!=null)
            {
                DatabaseWindow.Activate();
                return;
            }

            var progress = new ProgressDialog();
            progress.Owner = this;
            progress.ImageSource = (ImageSource)FindResource("DatabaseDrawingImage");
            var task = LoadDatabase(progress);
            progress.ShowDialog();
            try
            {
                var db = await task;
                DatabaseWindow = new DatabaseWindow();
                DatabaseWindow.Closed += DatabaseWindow_Closed;
                DatabaseWindow.Data = new ObservableCollection<DbEntry>(db);
                DatabaseWindow.ChannelSetEvent += DatabaseWindow_ChannelSetEvent;
                DatabaseWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatabaseWindow_ChannelSetEvent(ChannelParameter p)
        {
            ChannelControl[p.Channel].SetParameter(p);
            ChannelInfo[p.Channel].Parameter = p;
        }

        private void DatabaseWindow_Closed(object sender, EventArgs e)
        {
            DatabaseWindow.ChannelSetEvent -= DatabaseWindow_ChannelSetEvent;
            DatabaseWindow = null;
        }

        ChannelParameter SetChannelFunction(IAlcConnection con, int channel, int function)
        {
            var p = LogConvert.Parse<ChannelParameter>(con.GetParameter(channel));
            p.Program = function;
            return LogConvert.Parse<ChannelParameter>(con.Cmd(p.GetBytes()));
        }

        private async void SetFunction(int channel, int function)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var p = await Task.Run(delegate {
                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        return SetChannelFunction(con, channel, function);
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                });

                ChannelControl[channel].SetParameter(p);
                ChannelInfo[channel].Parameter = p;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }
        private async void Device_Click(object sender, RoutedEventArgs e)
        {
            var d = new DeviceWindow();
            await d.LoadParameters();
            d.Show();

#if false
            DeviceParameters p0 = null;
            DeviceParametersEx1 p1 = null;
            DeviceParametersEx2 p2 = null;
            DeviceParametersEx3 p3 = null;
            DeviceParametersEx4 p4 = null;

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                await Task.Run(delegate {
                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        p0 = LogConvert.Parse<DeviceParameters>(con.GetDeviceParameters());
                        p1 = LogConvert.Parse<DeviceParametersEx1>(con.GetDeviceParametersEx1());
                        p2 = LogConvert.Parse<DeviceParametersEx2>(con.GetDeviceParametersEx2());
                        p3 = LogConvert.Parse<DeviceParametersEx3>(con.GetDeviceParametersEx3());
                        p4 = LogConvert.Parse<DeviceParametersEx4>(con.GetDeviceParametersEx4());
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                });

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            if (p0 != null && p1 != null && p2 != null && p3 !=null && p4!=null)
            {
                var d = new DeviceWindow();
                d.Params0 = p0;
                d.Params1 = p1;
                d.Params2 = p2;
                d.Params3 = p3;
                d.Params4 = p4;
                d.Show();
            }
            else MessageBox.Show("Geräteparameter konnten nicht gelesen werden");
#endif
        }
    }
}
