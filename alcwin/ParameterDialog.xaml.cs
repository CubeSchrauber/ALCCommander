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
using alclib;

namespace alcwin
{
    /// <summary>
    /// Interaktionslogik für ParameterDialog.xaml
    /// </summary>
    public partial class ParameterDialog : Window
    {
        private int Function;
        public ParameterDialog()
        {
            InitializeComponent();
        }

        public void SetParameter(ChannelParameter p)
        {
            if (p != null)
                Function = p.Program;
            Parameter.SetParameter(p);
        }

        public ChannelParameter GetParameter()
        {
            var p = Parameter.GetParameter();
            p.Program = Function;
            return p;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
