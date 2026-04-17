using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class LogData
    {
        public LogHeader Header;
        public LogSection[] Sections;
        public const double TimeStep = 5;

        public int VoltageMin { get; private set; }
        public int VoltageMax { get; private set; }
        public int CurrentMin { get; private set; }
        public int CurrentMax { get; private set; }
        public int CapacityMin { get; private set; }
        public int CapacityMax { get; private set; }
        public int TimeMax { get; private set; }

        public LogData(LogHeader header, LogRecord[] records)
        {
            Header = header;

            VoltageMin = int.MaxValue; VoltageMax = int.MinValue;
            CurrentMin = int.MaxValue; CurrentMax = int.MinValue;
            CapacityMin = int.MaxValue; CapacityMax = int.MinValue;
            TimeMax = records.Length;

            Sections = LogConvert.SplitRecords(records);

            foreach(var r in records)
            {
                if (r.Valid)
                {
                    int v = r.Voltage;
                    if (v < VoltageMin) VoltageMin = v;
                    if (v > VoltageMax) VoltageMax = v;

                    int c = r.Current;
                    if (c < CurrentMin) CurrentMin = c;
                    if (c > CurrentMax) CurrentMax = c;

                    int a = r.Capacity;
                    if (a < CapacityMin) CapacityMin = a;
                    if (a > CapacityMax) CapacityMax = a;
                }
            }
        }

        public double nt(int s, int i)
        {
            return (double)(Sections[s].Index + i) / (double)TimeMax;
        }

        public double nv(int s, int i)
        {
            return LogConvert.Voltage(Sections[s].Data[i].Voltage - VoltageMin) / LogConvert.Voltage(VoltageMax - VoltageMin);
        }

        public double nc(int s, int i)
        {
            return LogConvert.Current(Sections[s].Data[i].Current - CurrentMin) / LogConvert.Current(CurrentMax - CurrentMin);
        }
    }
}
