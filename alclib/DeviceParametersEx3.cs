using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class DeviceParametersEx3 : Initializable
    {
        public int FinalDischargeVoltageLi435;
        public int PauseLi435;
        public int ChargeVoltageLi435;
        public int MaintainVoltageLi435;
        public int FinalDischargeVoltageNiZn;
        public int PauseNiZn;
        public int ChargeVoltageNiZn;
        public int MaintainVoltageNiZn;
        public int FinalDischargeVoltageAGM;
        public int PauseAGM;
        public int ChargeVoltageAGM;
        public int MaintainVoltageAGM;

        public DeviceParametersEx3()
        {
            ChargeVoltageAGM = 2450;
            ChargeVoltageLi435 = 4350;
            ChargeVoltageNiZn = 1900;
            FinalDischargeVoltageAGM = 1850;
            FinalDischargeVoltageLi435 = 3100;
            FinalDischargeVoltageNiZn = 1400;
            MaintainVoltageAGM = 2260;
            MaintainVoltageLi435 = 4150;
            MaintainVoltageNiZn = 1650;
            PauseAGM = 5;
            PauseLi435 = 5;
            PauseNiZn = 5;
        }

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'e')
                throw new ArgumentException();
            FinalDischargeVoltageLi435 = ToInt16(data, ref pos);
            PauseLi435 = data[pos++];
            ChargeVoltageLi435 = ToInt16(data, ref pos);
            MaintainVoltageLi435 = ToInt16(data, ref pos);
            FinalDischargeVoltageNiZn = ToInt16(data, ref pos);
            PauseNiZn = data[pos++];
            ChargeVoltageNiZn = ToInt16(data, ref pos);
            MaintainVoltageNiZn = ToInt16(data, ref pos);
            FinalDischargeVoltageAGM = ToInt16(data, ref pos);
            PauseAGM = data[pos++];
            ChargeVoltageAGM = ToInt16(data, ref pos);
            MaintainVoltageAGM = ToInt16(data, ref pos);
        }

        public byte[] GetBytes()
        {
            var s = new MemoryStream();
            s.WriteByte((byte)'E');
            WriteInt16(s, FinalDischargeVoltageLi435);
            s.WriteByte((byte)PauseLi435);
            WriteInt16(s, ChargeVoltageLi435);
            WriteInt16(s, MaintainVoltageLi435);
            WriteInt16(s, FinalDischargeVoltageNiZn);
            s.WriteByte((byte)PauseNiZn);
            WriteInt16(s, ChargeVoltageNiZn);
            WriteInt16(s, MaintainVoltageNiZn);
            WriteInt16(s, FinalDischargeVoltageAGM);
            s.WriteByte((byte)PauseAGM);
            WriteInt16(s, ChargeVoltageAGM);
            WriteInt16(s, MaintainVoltageAGM);
            return s.ToArray();
        }
    }
}
