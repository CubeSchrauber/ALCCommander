using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace alcwin
{
    public partial class App : Application
    {
        public static ConnectionHolder ConnectionHolder;
        public static Settings Settings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings = Settings.Load();

#if SIMFILE
            ConnectionHolder = new ConnectionHolder(@"C:\Prj\Misc\ALC500\TestData");
            ConnectionHolder.Open();
            Current.MainWindow = new MainWindow();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow.Show();
#else
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var d = new SerialDialog();
            if (d.ShowDialog()??false)
            {
                try
                {
                    Settings.Save();

                    ConnectionHolder = new ConnectionHolder(d.Port, d.Network);
                    ConnectionHolder.Open();

                    var main = new MainWindow();
                    Current.MainWindow = main;
                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    main.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Current.Shutdown();
                }

            }
            else
            {
                Current.Shutdown();
            }
#endif
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (ConnectionHolder!=null)
                    ConnectionHolder.Close();

                Settings.Save();
            }
            catch
            {

            }
        }
    }
}
