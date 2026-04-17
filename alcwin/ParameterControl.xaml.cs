using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using alclib;
using static System.FormattableString;

namespace alcwin
{
    /// <summary>
    /// Interaktionslogik für ParameterControl.xaml
    /// </summary>
    public partial class ParameterControl : UserControl
    {
        RadioButton[] VoltageButtons;
        RadioButton[] BatTypeButtons;
        int BatIndex = 40;

        public ParameterControl()
        {
            InitializeComponent();
            FillVoltageGrid();
            BatTypeButtons = new RadioButton[] { BatNiCd, BatNiMH, BatLiIon, BatLiPol, BatPb, BatLiPeFo4, BatLi435, BatNiZn, BatAgmCa };
        }

        private void BatType_Click(object sender, RoutedEventArgs e)
        {
            int idx = Convert.ToInt32(((RadioButton)sender).Tag);
            FillVoltage(idx);
        }

        private void FillVoltage(int idx)
        {
            var item = Data.BatTypes[idx];
            int v = item.Voltage;
            for (int i=0; i<VoltageButtons.Length; i++)
            {
                var b = VoltageButtons[i];
                b.Content = Invariant($"{(decimal)v / 1000m:F1} V");
                b.IsEnabled = v <= 30000;
                b.IsChecked = false;
                v += item.Voltage;
            }
            VoltageButtons[0].IsChecked = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void FillVoltageGrid()
        {
            int columns = VoltageGrid.ColumnDefinitions.Count;
            int rows = VoltageGrid.RowDefinitions.Count;
            VoltageButtons = new RadioButton[columns * rows];
            for (int col = 0; col < columns; col++)
                for (int row = 0; row < rows; row++)
                {
                    var b = new RadioButton();
                    b.GroupName = "Voltage";
                    Grid.SetRow(b, row);
                    Grid.SetColumn(b, col);
                    VoltageGrid.Children.Add(b);
                    VoltageButtons[col * rows + row] = b;
                    b.Click += VoltageButton_Click;
                }
        }

        private void VoltageButton_Click(object sender, RoutedEventArgs e)
        {
            var b = (ToggleButton)sender;
        }

        private void SetBaseParameter(IParameter p)
        {
            foreach (var b in BatTypeButtons)
                b.IsChecked = false;
            foreach (var b in VoltageButtons)
                b.IsChecked = false;

            if (p.BatType >= 0 && p.BatType < BatTypeButtons.Length)
            {
                BatTypeButtons[p.BatType].IsChecked = true;
                FillVoltage(p.BatType);
                if (p.Cells > 0 && p.Cells < VoltageButtons.Length - 1)
                    VoltageButtons[p.Cells - 1].IsChecked = true;
            }

            Capacity.Text = (p.Capacity / 10000).ToString();
            ChargeCurrent.Text = (p.ChargeCurrent / 10).ToString();
            DischargeCurrent.Text = (p.DisCurrent / 10).ToString();
            Pause.Text = ((decimal)p.Pause / 60m).ToString(CultureInfo.InvariantCulture);
            FillFactor.Text = p.ChargeFactor.ToString(CultureInfo.InvariantCulture);
            Activator.IsChecked = (p.Flags & 1) != 0;
        }

        private void GetBaseParameter(IParameter p)
        {
            for (int i = 0; i < BatTypeButtons.Length; i++)
                if (BatTypeButtons[i].IsChecked ?? false)
                    p.BatType = i;
            for (int i = 0; i < VoltageButtons.Length; i++)
                if (VoltageButtons[i].IsChecked ?? false)
                    p.Cells = i + 1;

            if (Int32.TryParse(Capacity.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out int cap))
                p.Capacity = cap * 10000;
            if (Int32.TryParse(ChargeCurrent.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out int cc))
                p.ChargeCurrent = cc * 10;
            if (Int32.TryParse(DischargeCurrent.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out int dc))
                p.DisCurrent = dc * 10;
            if (Decimal.TryParse(Pause.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal pause))
                p.Pause = (int)(pause * 60);
            if (Int32.TryParse(FillFactor.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out int ff))
                p.ChargeFactor = ff;
            if (Activator.IsChecked ?? false)
                p.Flags |= 1;
        }

        public void SetDbEntry(DbEntry e)
        {
            if (e == null)
                return;
            SetBaseParameter(e);
            BatIndex = e.Index;
            NameEdit.Text = e.Name;
            FormingCaption.Visibility = Visibility.Collapsed;
            FormingCurrent.Visibility = Visibility.Collapsed;
            FormingUnit.Visibility = Visibility.Collapsed;
            FormingButtons.Visibility = Visibility.Collapsed;
        }

        public DbEntry GetDbEntry()
        {
            var e = new DbEntry();
            GetBaseParameter(e);
            e.Name = NameEdit.Text;
            e.Index = BatIndex;
            return e;
        }

        public void Import(ChannelParameter p)
        {
            if (p == null)
                return;
            SetBaseParameter(p);
        }

        public void SetParameter(ChannelParameter p)
        {
            if (p == null)
                return;

            SetBaseParameter(p);
            FormingCurrent.Text = (p.FormingCurrent / 10).ToString();
            NameCaption.Visibility = Visibility.Collapsed;
            NameEdit.Visibility = Visibility.Collapsed;
        }

        public ChannelParameter GetParameter()
        {
            var p = new ChannelParameter();

            GetBaseParameter(p);
            p.BatIndex = 40;
            if (Int32.TryParse(FormingCurrent.Text, NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out int fc))
                p.FormingCurrent = fc * 10;
            return p;
        }


        private void ChargeCurrent_Click(object sender, RoutedEventArgs e)
        {
            var p = Convert.ToInt32(((FrameworkElement)sender).Tag);
            if (Int32.TryParse(Capacity.Text, out int c))
                ChargeCurrent.Text = (c * p / 60).ToString(CultureInfo.InvariantCulture);
        }

        private void DischargeCurrent_Click(object sender, RoutedEventArgs e)
        {
            var p = Convert.ToInt32(((FrameworkElement)sender).Tag);
            if (Int32.TryParse(Capacity.Text, out int c))
                DischargeCurrent.Text = (c * p / 60).ToString(CultureInfo.InvariantCulture);
        }

        private void FormingCurrent_Click(object sender, RoutedEventArgs e)
        {
            var p = Convert.ToInt32(((FrameworkElement)sender).Tag);
            if (Int32.TryParse(Capacity.Text, out int c))
                FormingCurrent.Text = (c * p / 60).ToString(CultureInfo.InvariantCulture);
        }
    }
}
