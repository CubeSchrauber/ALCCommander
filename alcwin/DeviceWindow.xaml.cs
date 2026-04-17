using alclib;
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
using System.Windows.Shapes;

namespace alcwin
{
    public partial class DeviceWindow : Window
    {
        public DeviceParameters Params0;
        public DeviceParametersEx1 Params1;
        public DeviceParametersEx2 Params2;
        public DeviceParametersEx3 Params3;
        public DeviceParametersEx4 Params4;
        
        public DeviceWindow()
        {
            InitializeComponent();
        }

        private void Display()
        {
            FinalDischargeVoltageNC.Text = Params0.FinalDischargeVoltageNC.ToString();
            ChargeEndLimitNC.Text = (Params0.ChargeEndLimitNC / 100m).ToString(CultureInfo.InvariantCulture);
            CyclesNC.Text = Params0.CyclesNC.ToString(CultureInfo.InvariantCulture);
            CyclesFormNC.Text = Params0.CyclesFormNC.ToString(CultureInfo.InvariantCulture);
            PauseNC.Text = Params0.PauseNC.ToString(CultureInfo.InvariantCulture);
            LadespannungNC.Text = "1500";
            ErhaltungsspannungNC.Text = "1500";

            FinalDischargeVoltageNiMH.Text = Params0.FinalDischargeVoltageNiMH.ToString();
            ChargeEndLimitNiMH.Text = (Params0.ChargeEndLimitNiMH / 100m).ToString(CultureInfo.InvariantCulture);
            CyclesNiMH.Text = Params0.CyclesNiMH.ToString(CultureInfo.InvariantCulture);
            CyclesFormNiMH.Text = Params0.CyclesFormNiMH.ToString(CultureInfo.InvariantCulture);
            PauseNiMH.Text = Params0.PauseNiMH.ToString(CultureInfo.InvariantCulture);
            LadespannungNiMH.Text = "1500";
            ErhaltungsspannungNiMH.Text = "1500";

            FinalDischargeVoltageLiIon.Text = Params0.FinalDischargeVoltageLiIon.ToString();
            PauseLiIon.Text = Params0.PauseLiIon.ToString(CultureInfo.InvariantCulture);
            LadespannungLiIon.Text = Params1.ChargeVoltageLiIon.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungLiIon.Text = Params1.MaintainVoltageLiIon.ToString(CultureInfo.InvariantCulture);

            FinalDischargeVoltageLiPol.Text = Params0.FinalDischargeVoltageLiPol.ToString();
            PauseLiPol.Text = Params0.PauseLiPol.ToString(CultureInfo.InvariantCulture);
            LadespannungLiPol.Text = Params1.ChargeVoltageLiPol.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungLiPol.Text = Params1.MaintainVoltageLiPol.ToString(CultureInfo.InvariantCulture);

            FinalDischargeVoltageLi435.Text = Params3.FinalDischargeVoltageLi435.ToString();
            PauseLi435.Text = Params3.PauseLi435.ToString(CultureInfo.InvariantCulture);
            LadespannungLi435.Text = Params3.ChargeVoltageLi435.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungLi435.Text = Params3.MaintainVoltageLi435.ToString(CultureInfo.InvariantCulture);

            FinalDischargeVoltageLiFePo.Text = Params2.FinalDischargeVoltageLiFePo4.ToString();
            PauseLiFePo.Text = Params2.PauseLiFePo4.ToString(CultureInfo.InvariantCulture);
            LadespannungLiFePo.Text = Params2.ChargeVoltageLiFePo4.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungLiFePo.Text = Params2.MaintainVoltageLiFePo4.ToString(CultureInfo.InvariantCulture);

            FinalDischargeVoltageNiZn.Text = Params3.FinalDischargeVoltageNiZn.ToString();
            PauseNiZn.Text = Params3.PauseNiZn.ToString(CultureInfo.InvariantCulture);
            LadespannungNiZn.Text = Params3.ChargeVoltageNiZn.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungNiZn.Text = Params3.MaintainVoltageNiZn.ToString(CultureInfo.InvariantCulture);

            FinalDischargeVoltagePb.Text = Params4.FinalDischargeVoltagePb.ToString();
            PausePb.Text = Params0.PausePb.ToString(CultureInfo.InvariantCulture);
            LadespannungPb.Text = Params1.ChargeVoltagePb.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungPb.Text = Params1.MaintainVoltagePb.ToString(CultureInfo.InvariantCulture);

            FinalDischargeVoltageAGM.Text = Params3.FinalDischargeVoltageAGM.ToString();
            PauseAGM.Text = Params3.PauseAGM.ToString(CultureInfo.InvariantCulture);
            LadespannungAGM.Text = Params3.ChargeVoltageAGM.ToString(CultureInfo.InvariantCulture);
            ErhaltungsspannungAGM.Text = Params3.MaintainVoltageAGM.ToString(CultureInfo.InvariantCulture);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Display();
        }

        private static void GetInt(TextBox t, ref int v)
        {
            if (Int32.TryParse(t.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out int ival))
                v = ival;
        }

        private static void GetDec(TextBox t, ref int v)
        {
            if (Decimal.TryParse(t.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out decimal dval))
                v = (int)(dval * 100);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var p0 = new DeviceParameters();
            var p1 = new DeviceParametersEx1();
            var p2 = new DeviceParametersEx2();
            var p3 = new DeviceParametersEx3();
            var p4 = new DeviceParametersEx4();

            GetInt(FinalDischargeVoltageNC, ref p0.FinalDischargeVoltageNC);
            GetDec(ChargeEndLimitNC, ref p0.ChargeEndLimitNC);
            GetInt(CyclesNC, ref p0.CyclesNC);
            GetInt(CyclesFormNC, ref p0.CyclesFormNC);
            GetInt(PauseNC, ref p0.PauseNC);

            GetInt(FinalDischargeVoltageNiMH, ref p0.FinalDischargeVoltageNiMH);
            GetDec(ChargeEndLimitNiMH, ref p0.ChargeEndLimitNiMH);
            GetInt(CyclesNiMH, ref p0.CyclesNiMH);
            GetInt(CyclesFormNiMH, ref p0.CyclesFormNiMH);
            GetInt(PauseNiMH, ref p0.PauseNiMH);

            GetInt(FinalDischargeVoltageLiIon, ref p0.FinalDischargeVoltageLiIon);
            GetInt(PauseLiIon, ref p0.PauseLiIon);
            GetInt(LadespannungLiIon, ref p1.ChargeVoltageLiIon);
            GetInt(ErhaltungsspannungLiIon, ref p1.MaintainVoltageLiIon);

            GetInt(FinalDischargeVoltageLiPol, ref p0.FinalDischargeVoltageLiPol);
            GetInt(PauseLiPol, ref p0.PauseLiPol);
            GetInt(LadespannungLiPol, ref p1.ChargeVoltageLiPol);
            GetInt(ErhaltungsspannungLiPol, ref p1.MaintainVoltageLiPol);

            GetInt(FinalDischargeVoltageLi435, ref p3.FinalDischargeVoltageLi435);
            GetInt(PauseLi435, ref p3.PauseLi435);
            GetInt(LadespannungLi435, ref p3.ChargeVoltageLi435);
            GetInt(ErhaltungsspannungLi435, ref p3.MaintainVoltageLi435);

            GetInt(FinalDischargeVoltageLiFePo, ref p2.FinalDischargeVoltageLiFePo4);
            GetInt(PauseLiFePo, ref p2.PauseLiFePo4);
            GetInt(LadespannungLiFePo, ref p2.ChargeVoltageLiFePo4);
            GetInt(ErhaltungsspannungLiFePo, ref p2.MaintainVoltageLiFePo4);

            GetInt(FinalDischargeVoltageNiZn, ref p3.FinalDischargeVoltageNiZn);
            GetInt(PauseNiZn, ref p3.PauseNiZn);
            GetInt(LadespannungNiZn, ref p3.ChargeVoltageNiZn);
            GetInt(ErhaltungsspannungNiZn, ref p3.MaintainVoltageNiZn);

            GetInt(FinalDischargeVoltagePb, ref p4.FinalDischargeVoltagePb);
            GetInt(PausePb, ref p0.PausePb);
            GetInt(LadespannungPb, ref p1.ChargeVoltagePb);
            GetInt(ErhaltungsspannungPb, ref p1.MaintainVoltagePb);

            GetInt(FinalDischargeVoltageAGM, ref p3.FinalDischargeVoltageAGM);
            GetInt(PauseAGM, ref p3.PauseAGM);
            GetInt(LadespannungAGM, ref p3.ChargeVoltageAGM);
            GetInt(ErhaltungsspannungAGM, ref p3.MaintainVoltageAGM);

            try
            {
                await Task.Run(delegate {
                    var con = App.ConnectionHolder.GetConnection(true);
                    try
                    {
                        p0 = LogConvert.Parse<DeviceParameters>(con.Cmd(p0.GetBytes()));
                        p1 = LogConvert.Parse<DeviceParametersEx1>(con.Cmd(p1.GetBytes()));
                        p2 = LogConvert.Parse<DeviceParametersEx2>(con.Cmd(p2.GetBytes()));
                        p3 = LogConvert.Parse<DeviceParametersEx3>(con.Cmd(p3.GetBytes()));
                        p4 = LogConvert.Parse<DeviceParametersEx4>(con.Cmd(p4.GetBytes()));
                    }
                    finally
                    {
                        App.ConnectionHolder.ReleaseConnection();
                    }
                });

                if (p0 != null)
                    Params0 = p0;
                if (p1 != null)
                    Params1 = p1;
                if (p2 != null)
                    Params2 = p2;
                if (p3 != null)
                    Params3 = p3;
                if (p4 != null)
                    Params4 = p4;

                Display();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task LoadParameters()
        {
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            if (p0 != null && p1 != null && p2 != null && p3 != null && p4 != null)
            {
                Params0 = p0;
                Params1 = p1;
                Params2 = p2;
                Params3 = p3;
                Params4 = p4;
                Display();
            }
            else MessageBox.Show("Geräteparameter konnten nicht gelesen werden");
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            await LoadParameters();
        }
    }
}
