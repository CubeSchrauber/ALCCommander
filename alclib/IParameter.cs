using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public interface IParameter
    {
        int BatType { get; set; }
        int Cells { get; set; }
        int DisCurrent { get; set; }
        int ChargeCurrent { get; set; }
        int Capacity { get; set; }
        int Pause { get; set; }
        int Flags { get; set; }
        int ChargeFactor { get; set; }
    }
}
