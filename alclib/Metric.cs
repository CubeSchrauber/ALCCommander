using System;
using System.IO;
using static System.FormattableString;

namespace alclib
{
    public class Metric : Initializable
    {
        public int Channel;
        public int Voltage;
        public int Current;
        public int Capacity;

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'm')
                throw new ArgumentException();
            Channel = data[pos++];
            Voltage = ToInt16(data, ref pos);
            Current = ToInt16(data, ref pos);
            Capacity = ToInt32(data, ref pos);
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine($"Channel: {Channel}");
            writer.WriteLine(Invariant($"Voltage: {(decimal)Voltage / 1000m:F3} V"));
            writer.WriteLine(Invariant($"Current: {(decimal)Current / 10000m:F3} A"));
            writer.WriteLine(Invariant($"Capacity: {(decimal)Capacity / 10000000m:F3} Ah"));
        }
    }

}