using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class DeviceParametersEx4 : Initializable
    {
        public int Unknown1;
        public int Unknown2;
        public int Unknown3;
        public int FinalDischargeVoltagePb;

        public DeviceParametersEx4()
        {
            FinalDischargeVoltagePb = 1730;
            Unknown1 = 3600;
            Unknown2 = 3700;
            Unknown3 = 3700;
        }

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'z')
                throw new ArgumentException();
            Unknown1 = ToInt16(data, ref pos);
            Unknown2 = ToInt16(data, ref pos);
            Unknown3 = ToInt16(data, ref pos);
            FinalDischargeVoltagePb = ToInt16(data, ref pos);
        }

        public byte[] GetBytes()
        {
            var s = new MemoryStream();
            WriteInt16(s, Unknown1);
            WriteInt16(s, Unknown1);
            WriteInt16(s, Unknown1);
            WriteInt16(s, FinalDischargeVoltagePb);
            return s.ToArray();
        }
    }
}
