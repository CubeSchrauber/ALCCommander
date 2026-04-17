using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.FormattableString;

namespace alclib
{
    public class ChannelParameter : Initializable, IParameter
    {
        public int Channel { get; set; }
        public int BatIndex { get; set; }
        public int BatType { get; set; }
        public int Cells { get; set; }
        public int DisCurrent { get; set; }
        public int ChargeCurrent { get; set; }
        public int Capacity { get; set; }
        public int Program { get; set; }
        public int FormingCurrent { get; set; }
        public int Pause { get; set; }
        public int Flags { get; set; }
        public int ChargeFactor { get; set; }
        public int LogEnd { get; set; }

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'p')
                throw new ArgumentException();
            Channel = data[pos++];
            BatIndex = data[pos++];
            BatType = data[pos++];
            Cells = data[pos++];
            DisCurrent = ToInt16(data, ref pos);
            ChargeCurrent = ToInt16(data, ref pos);
            Capacity = ToInt32(data, ref pos);
            Program = data[pos++];
            FormingCurrent = ToInt16(data, ref pos);
            Pause = ToInt16(data, ref pos);
            Flags = data[pos++];
            LogEnd = ToInt16(data, ref pos);
            ChargeFactor = data[pos++];
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine($"Channel: {Channel}");
            writer.WriteLine($"Battery Index: {BatIndex}");
            writer.WriteLine($"Battery Type: {LogConvert.BatType(BatType)} ({BatType:X02})");
            writer.WriteLine($"Cells: {Cells}");
            writer.WriteLine(Invariant($"Discharge Current: {(decimal)DisCurrent / 10000m:F3} A"));
            writer.WriteLine(Invariant($"Charge Current: {(decimal)ChargeCurrent / 10000m:F3} A"));
            writer.WriteLine(Invariant($"Capacity: {(decimal)Capacity / 10000000m:F3} Ah"));
            writer.WriteLine($"Program: {LogConvert.Function(Program)} ({Program:X02})");
            writer.WriteLine(Invariant($"Forming Current: {(decimal)ChargeCurrent / 10000m:F3} A"));
            writer.WriteLine($"Pause: {Pause} sec");
            writer.WriteLine($"Flags: {Flags:X02}");
            writer.WriteLine($"Charge Factor: {ChargeFactor}");
            writer.WriteLine($"Memory Stop: {LogEnd:X04}");
        }

        public byte[] GetBytes()
        {
            var data = new byte[20];
            int pos = 0;
            data[pos++] = (byte)'P';
            data[pos++] = (byte)Channel;
            data[pos++] = (byte)BatIndex;
            data[pos++] = (byte)BatType;
            data[pos++] = (byte)Cells;
            data[pos++] = (byte)(DisCurrent >> 8);
            data[pos++] = (byte)(DisCurrent & 255);
            data[pos++] = (byte)(ChargeCurrent >> 8);
            data[pos++] = (byte)(ChargeCurrent & 255);
            data[pos++] = (byte)(Capacity >> 24);
            data[pos++] = (byte)(Capacity >> 16);
            data[pos++] = (byte)(Capacity >> 8);
            data[pos++] = (byte)(Capacity & 255);
            data[pos++] = (byte)Program;
            data[pos++] = (byte)(FormingCurrent >> 8);
            data[pos++] = (byte)(FormingCurrent & 255);
            data[pos++] = (byte)(Pause >> 8);
            data[pos++] = (byte)(Pause & 255);
            data[pos++] = (byte)Flags;
            data[pos++] = (byte)ChargeFactor;
            return data;
        }
    }
}
