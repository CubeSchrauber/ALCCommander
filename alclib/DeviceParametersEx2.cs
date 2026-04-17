using System;
using System.IO;
using static System.FormattableString;

namespace alclib
{
    public class DeviceParametersEx2 : Initializable
    {
        public int FinalDischargeVoltageLiFePo4;
        public int PauseLiFePo4;
        public int ChargeVoltageLiFePo4;
        public int MaintainVoltageLiFePo4;
        public int Reserved;
        public int ConfigAlc;
        public int Contrast;

        public DeviceParametersEx2()
        {
            ChargeVoltageLiFePo4 = 3650;
            ConfigAlc = 9;
            Contrast = 12;
            FinalDischargeVoltageLiFePo4 = 2300;
            MaintainVoltageLiFePo4 = 3450;
            PauseLiFePo4 = 5;
            Reserved = 0;
        }

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'j')
                throw new ArgumentException();
            FinalDischargeVoltageLiFePo4 = ToInt16(data, ref pos);
            PauseLiFePo4 = data[pos++];
            ChargeVoltageLiFePo4 = ToInt16(data, ref pos);
            MaintainVoltageLiFePo4 = ToInt16(data, ref pos);
            Reserved = data[pos++];
            ConfigAlc = data[pos++];
            Contrast = data[pos++];
        }

        public byte[] GetBytes()
        {
            var s = new MemoryStream();
            WriteInt16(s, FinalDischargeVoltageLiFePo4);
            s.WriteByte((byte)PauseLiFePo4);
            WriteInt16(s, ChargeVoltageLiFePo4);
            WriteInt16(s, MaintainVoltageLiFePo4);
            s.WriteByte((byte)Reserved);
            s.WriteByte((byte)ConfigAlc);
            s.WriteByte((byte)Contrast);
            return s.ToArray();
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine(Invariant($"Final Discharge Voltage LiFePo4: {(decimal)FinalDischargeVoltageLiFePo4 / 1000m:F3}"));
            writer.WriteLine($"Pause LiFePo4: {PauseLiFePo4}");
            writer.WriteLine(Invariant($"Charge Voltage LiFePo4: {(decimal)ChargeVoltageLiFePo4 / 1000m:F3}"));
            writer.WriteLine(Invariant($"Maintain Voltage LiFePo4: {(decimal)MaintainVoltageLiFePo4 / 1000m:F3}"));
            writer.WriteLine($"Reserved: {Reserved:X02}");
            writer.WriteLine($"ConfigAlc: {ConfigAlc:X02}");
            writer.WriteLine($"Contrast: {Contrast:X02}");
        }
    }

}