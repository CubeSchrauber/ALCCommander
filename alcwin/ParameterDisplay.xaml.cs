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

namespace alcwin
{
    /// <summary>
    /// Interaktionslogik für ParameterDisplay.xaml
    /// </summary>
    public partial class ParameterDisplay : UserControl
    {
        public ParameterDisplay()
        {
            InitializeComponent();
        }

        public void SetParameter(ChannelParameter p)
        {
            if (p.Program>=0 && p.Program<Data.Functions.Length)
                Function.Text = Data.Functions[p.Program];
            if (p.BatType >= 0 && p.BatType < Data.BatTypes.Length)
            {
                var b = Data.BatTypes[p.BatType];
                BatType.Text = b.Caption;
                Voltage.Text = ((decimal)(p.Cells*b.Voltage) / 1000m).ToString("F1", CultureInfo.InvariantCulture);
            }
            Capacity.Text = (p.Capacity / 10000).ToString();
            ChargeCurrent.Text = (p.ChargeCurrent / 10).ToString();
            DischargeCurrent.Text = (p.DisCurrent / 10).ToString();
        }
    }
}
