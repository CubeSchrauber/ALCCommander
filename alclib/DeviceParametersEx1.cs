using System;
using System.IO;
using static System.FormattableString;

namespace alclib
{
    public class DeviceParametersEx1 : Initializable
    {
        public int Reserved1;
        public int Reserved2;
        public int Reserved3;
        public int Reserved4;
        public int ChargeVoltageLiIon;
        public int MaintainVoltageLiIon;
        public int ChargeVoltageLiPol;
        public int MaintainVoltageLiPol;
        public int ChargeVoltagePb;
        public int MaintainVoltagePb;
        public int LowBatSource;

        public DeviceParametersEx1()
        {
            ChargeVoltageLiIon = 4100;
            ChargeVoltageLiPol = 4200;
            ChargeVoltagePb = 2360;
            LowBatSource = 11000;
            MaintainVoltageLiIon = 4050;
            MaintainVoltageLiPol = 4150;
            MaintainVoltagePb = 2260;
            Reserved1 = 1500;
            Reserved2 = 1500;
            Reserved3 = 1500;
            Reserved4 = 1500;
        }

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'h')
                throw new ArgumentException();
            Reserved1 = ToInt16(data, ref pos);
            Reserved2 = ToInt16(data, ref pos);
            Reserved3 = ToInt16(data, ref pos);
            Reserved4 = ToInt16(data, ref pos);
            ChargeVoltageLiIon = ToInt16(data, ref pos);		
            MaintainVoltageLiIon = ToInt16(data, ref pos);
            ChargeVoltageLiPol = ToInt16(data, ref pos);
            MaintainVoltageLiPol = ToInt16(data, ref pos);
            ChargeVoltagePb = ToInt16(data, ref pos);
            MaintainVoltagePb = ToInt16(data, ref pos);
            LowBatSource = ToInt16(data, ref pos);
        }

        public byte[] GetBytes()
        {
            var s = new MemoryStream();
            s.WriteByte((byte)'H');
            WriteInt16(s, Reserved1);
            WriteInt16(s, Reserved2);
            WriteInt16(s, Reserved3);
            WriteInt16(s, Reserved4);
            WriteInt16(s, ChargeVoltageLiIon);
            WriteInt16(s, MaintainVoltageLiIon);
            WriteInt16(s, ChargeVoltageLiPol);
            WriteInt16(s, MaintainVoltageLiPol);
            WriteInt16(s, ChargeVoltagePb);
            WriteInt16(s, MaintainVoltagePb);
            WriteInt16(s, LowBatSource);
            return s.ToArray();
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine($"Reserved1: {Reserved1:X4}");
            writer.WriteLine($"Reserved2: {Reserved2:X4}");
            writer.WriteLine($"Reserved3: {Reserved3:X4}");
            writer.WriteLine($"Reserved4: {Reserved4:X4}");
            writer.WriteLine(Invariant($"Charge Voltage LiIon: {(decimal)ChargeVoltageLiIon / 1000m:F3}"));
            writer.WriteLine(Invariant($"Maintain Voltage LiIon: {(decimal)MaintainVoltageLiIon / 1000m:F3}"));
            writer.WriteLine(Invariant($"Charge Voltage LiPol: {(decimal)ChargeVoltageLiPol / 1000m:F3}"));
            writer.WriteLine(Invariant($"Maintain Voltage LiPol: {(decimal)MaintainVoltageLiPol / 1000m:F3}"));
            writer.WriteLine(Invariant($"Charge Voltage Pb: {(decimal)ChargeVoltagePb / 1000m:F3}"));
            writer.WriteLine(Invariant($"Maintain Voltage Pb: {(decimal)MaintainVoltagePb / 1000m:F3}"));
            writer.WriteLine(Invariant($"Low Battery Limit Source: {(decimal)LowBatSource / 1000m:F3}"));
        }
    }
}