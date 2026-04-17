using System;
using System.IO;
using static System.FormattableString;

namespace alclib
{
    public class DeviceParameters : Initializable
    {
        public int FinalDischargeVoltageNC;
        public int FinalDischargeVoltageNiMH;
        public int FinalDischargeVoltageLiIon;
        public int FinalDischargeVoltageLiPol;
        public int FinalDischargeVoltagePb;
        public int CyclesNC;
        public int CyclesNiMH;
        public int CyclesFormNC;
        public int CyclesFormNiMH;
        public int PauseNC;
        public int PauseNiMH;
        public int PauseLiIon;
        public int PauseLiPol;
        public int PausePb;
        public int ChargeEndLimitNC;
        public int ChargeEndLimitNiMH;

        public DeviceParameters()
        {
            ChargeEndLimitNC = 40;
            ChargeEndLimitNiMH = 20;
            CyclesFormNC = 10;
            CyclesFormNiMH = 10;
            CyclesNC = 10;
            CyclesNiMH = 10;
            FinalDischargeVoltageLiIon = 3000;
            FinalDischargeVoltageLiPol = 3100;
            FinalDischargeVoltageNC = 900;
            FinalDischargeVoltageNiMH = 1000;
            FinalDischargeVoltagePb = 1850;
            PauseLiIon = 5;
            PauseLiPol = 5;
            PauseNC = 5;
            PauseNiMH = 5;
            PausePb = 5;
        }

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'g')
                throw new ArgumentException();
            FinalDischargeVoltageNC = ToInt16(data, ref pos);
            FinalDischargeVoltageNiMH = ToInt16(data, ref pos);
            FinalDischargeVoltageLiIon = ToInt16(data, ref pos);
            FinalDischargeVoltageLiPol = ToInt16(data, ref pos);
            FinalDischargeVoltagePb = ToInt16(data, ref pos);
            CyclesNC = data[pos++];
            CyclesNiMH = data[pos++];
            CyclesFormNC = data[pos++];
            CyclesFormNiMH = data[pos++];
            PauseNC = data[pos++];
            PauseNiMH = data[pos++];
            PauseLiIon = data[pos++];
            PauseLiPol = data[pos++];
            PausePb = data[pos++];
            ChargeEndLimitNC = data[pos++];
            ChargeEndLimitNiMH = data[pos++];
        }

        public byte[] GetBytes()
        {
            var s = new MemoryStream();
            s.WriteByte((byte)'G');
            WriteInt16(s, FinalDischargeVoltageNC);
            WriteInt16(s, FinalDischargeVoltageNiMH);
            WriteInt16(s, FinalDischargeVoltageLiIon);
            WriteInt16(s, FinalDischargeVoltageLiPol);
            WriteInt16(s, FinalDischargeVoltagePb);
            s.WriteByte((byte)CyclesNC);
            s.WriteByte((byte)CyclesNiMH);
            s.WriteByte((byte)CyclesFormNC);
            s.WriteByte((byte)CyclesFormNiMH);
            s.WriteByte((byte)PauseNC);
            s.WriteByte((byte)PauseNiMH);
            s.WriteByte((byte)PauseLiIon);
            s.WriteByte((byte)PauseLiPol);
            s.WriteByte((byte)PausePb);
            s.WriteByte((byte)ChargeEndLimitNC);
            s.WriteByte((byte)ChargeEndLimitNiMH);
            return s.ToArray();
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine(Invariant($"Final Discharge Voltage NC: {(decimal)FinalDischargeVoltageNC / 1000m:F3} V"));
            writer.WriteLine(Invariant($"Final Discharge Voltage NiMH: {(decimal)FinalDischargeVoltageNiMH / 1000m:F3} V"));
            writer.WriteLine(Invariant($"Final Discharge Voltage LiIon: {(decimal)FinalDischargeVoltageLiIon / 1000m:F3} V"));
            writer.WriteLine(Invariant($"Final Discharge Voltage LiPol: {(decimal)FinalDischargeVoltageLiPol / 1000m:F3} V"));
            writer.WriteLine(Invariant($"Final Discharge Voltage Pb: {(decimal)FinalDischargeVoltagePb / 1000m:F3} V"));
            writer.WriteLine($"Cycles NC: {CyclesNC}");
            writer.WriteLine($"Cycles NiMH: {CyclesNiMH}");
            writer.WriteLine($"Cycles Form NC: {CyclesFormNC}");
            writer.WriteLine($"Cycles Form NiMH: {CyclesFormNiMH}");
            writer.WriteLine($"Pause NC: {PauseNC}");
            writer.WriteLine($"Pause NiMH: {PauseNiMH}");
            writer.WriteLine($"Pause LiIon: {PauseLiIon}");
            writer.WriteLine($"Pause LiPol: {PauseLiPol}");
            writer.WriteLine($"Pause Pb: {PausePb}");
            writer.WriteLine(Invariant($"Charge End Limit NC: {(decimal)ChargeEndLimitNC / 100m:F2}%"));
            writer.WriteLine(Invariant($"Charge End Limit NiMH: {(decimal)ChargeEndLimitNiMH / 100m:F2}%"));
        }
    }

}