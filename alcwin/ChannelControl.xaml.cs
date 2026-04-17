using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using alclib;
using static alclib.AlcConnection;

namespace alcwin
{
    public partial class ChannelControl : UserControl
    {
        public static readonly DependencyProperty CaptionProperty;
        public static readonly DependencyProperty ChargeStateProperty;

        public event Action<int> EditEvent;
        public event Action<int> StartEvent;
        public event Action<int> StopEvent;
        public event Action<int> LogEvent;
        public event Action<int> DeleteLogEvent;
        public event Action<int, int> FunctionEvent;

        static ChannelControl()
        {
            CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(ChannelControl), new PropertyMetadata("Kanal"));
            ChargeStateProperty = DependencyProperty.Register("ChargeState", typeof(int), typeof(ChannelControl));
        }

        public ChannelControl()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public int ChargeState
        {
            get { return (int)GetValue(ChargeStateProperty); }
            set { SetValue(ChargeStateProperty, value); }
        }

        public int Channel
        {
            get;set;
        }

        public void SetParameter(ChannelParameter p)
        {
            if (p != null)
            {
                if (p.Program >= 0 && p.Program < Data.Functions.Length)
                    //Function.Text = DisplayConvert.Function(p.Program);
                    Function.Content = DisplayConvert.Function(p.Program);
                if (p.BatType >= 0 && p.BatType < Data.BatTypes.Length)
                {
                    BatType.Text = DisplayConvert.BatType(p.BatType);
                    Voltage.Text = DisplayConvert.Voltage(p.BatType, p.Cells);
                }
                else BatType.Text = $"? {p.BatType:X02}";
                Capacity.Text = DisplayConvert.Capacity(p.Capacity);
                ChargeCurrent.Text = DisplayConvert.Current(p.ChargeCurrent);
                DischargeCurrent.Text = DisplayConvert.Current(p.DisCurrent);
                StateLog.Text = p.LogEnd.ToString();
            }
            else
            {
                //Function.Text = null;
                Function.Content = null;
                BatType.Text = null;
                Voltage.Text = null;
                Capacity.Text = null;
                ChargeCurrent.Text = null;
                DischargeCurrent.Text = null;
                StateLog.Text = null;
            }
        }

        public void SetChargeState(ChargeState s)
        {
            int v = s?.Value ?? 0;
            StateIdle.Visibility = v == 0 ? Visibility.Visible : Visibility.Hidden;
            StatePause.Visibility = v == 1 ? Visibility.Visible : Visibility.Hidden;
            StateDischarging.Visibility = v == 2 ? Visibility.Visible : Visibility.Hidden;
            StateCharging.Visibility = v == 3 ? Visibility.Visible : Visibility.Hidden;
            StateTrickleCharge.Visibility = v == 4 ? Visibility.Visible : Visibility.Hidden;
            StateDischarged.Visibility = v == 5 ? Visibility.Visible : Visibility.Hidden;
            StateEmergency.Visibility = v == 6 ? Visibility.Visible : Visibility.Hidden;

            if (s != null)
                ChargeStateText.Text = s.Name;
            else
                ChargeStateText.Text = null;
        }

        public void SetMetric(Metric m)
        {
            if (m != null)
            {
                StateVoltage.Text = ((decimal)m.Voltage / 1000m).ToString("F2", CultureInfo.InvariantCulture);
                StateCurrent.Text = (m.Current / 10).ToString(CultureInfo.InvariantCulture);
                StateCapacity.Text = (m.Capacity / 10000).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                StateVoltage.Text = null;
                StateCurrent.Text = null;
                StateCapacity.Text = null;
            }
        }

        public void SetInfo(ChannelInfo info)
        {
            SetParameter(info.Parameter);
            SetChargeState(info.ChargeState);
            SetMetric(info.Metric);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EditEvent != null)
                EditEvent(Channel);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (StartEvent != null)
                StartEvent(Channel);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (StopEvent != null)
                StopEvent(Channel);
        }

        private void Log_Click(object sender, RoutedEventArgs e)
        {
            if (LogEvent != null)
                LogEvent(Channel);
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteLogEvent != null)
                DeleteLogEvent(Channel);
        }

        private void Function_Click(object sender, RoutedEventArgs e)
        {
            FunctionPopup.IsOpen = true;
        }

        private void FuncButton_Click(object sender, RoutedEventArgs e)
        {
            FunctionPopup.IsOpen = false;
            var button = sender as RadioButton;
            if (button!=null)
            {
                int function = Convert.ToInt32(button.Tag);
                if (FunctionEvent != null)
                    FunctionEvent(Channel, function);
            }
        }
    }
}
