using System;
using System.IO;
using System.Text;
using static System.FormattableString;

namespace alclib
{
    public class DbEntry : Initializable, IParameter
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int BatType { get; set; }
        public int Cells { get; set; }
        public int Capacity { get; set; }
        public int DisCurrent { get; set; }
        public int ChargeCurrent { get; set; }
        public int Pause { get; set; }
        public int Flags { get; set; }
        public int ChargeFactor { get; set; }
        public int Reserved { get; set; }

        private static Encoding NameEncoding;

        static DbEntry()
        {
            NameEncoding = Encoding.GetEncoding(1252);
        }


        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'd')
                throw new ArgumentException();

            Index = data[pos++];
            Name = NameEncoding.GetString(data, pos, 9).Trim(); pos += 9;
            BatType = data[pos++];
            Cells = data[pos++];
            Capacity = ToInt32(data, ref pos);
            DisCurrent = ToInt16(data, ref pos);
            ChargeCurrent = ToInt16(data, ref pos);
            Pause = ToInt16(data, ref pos);
            Flags = data[pos++];
            ChargeFactor = data[pos++];
            Reserved = data[pos++];
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[26];
            int pos = 0;
            data[pos++] = (byte)'D';
            data[pos++] = (byte)Index;
            byte[] name_bytes = NameEncoding.GetBytes(Name);
            for (int i = 0; i < 9; i++)
                data[pos++] = i < name_bytes.Length ? name_bytes[i] : (byte)' ';
            data[pos++] = (byte)BatType;
            data[pos++] = (byte)Cells;
            data[pos++] = (byte)(Capacity >> 24);
            data[pos++] = (byte)(Capacity >> 16);
            data[pos++] = (byte)(Capacity >> 8);
            data[pos++] = (byte)(Capacity & 255);
            data[pos++] = (byte)(DisCurrent >> 8);
            data[pos++] = (byte)(DisCurrent & 255);
            data[pos++] = (byte)(ChargeCurrent >> 8);
            data[pos++] = (byte)(ChargeCurrent & 255);
            data[pos++] = (byte)(Pause >> 8);
            data[pos++] = (byte)(Pause & 255);
            data[pos++] = (byte)Flags;
            data[pos++] = (byte)ChargeFactor;
            data[pos++] = (byte)Reserved;
            return data;
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine($"Index: {Index}");
            writer.WriteLine($"Name: {Name}");
            writer.WriteLine($"Battery Type: {LogConvert.BatType(BatType)} ({BatType:X02})");
            writer.WriteLine($"Cells: {Cells}");
            writer.WriteLine(Invariant($"Capacity: {(decimal)Capacity / 10000000m:F3} Ah"));
            writer.WriteLine(Invariant($"Discharge Current: {(decimal)DisCurrent / 10000m:F3} A"));
            writer.WriteLine(Invariant($"Charge Current: {(decimal)ChargeCurrent / 10000m:F3} A"));
            writer.WriteLine($"Pause: {Pause} sec");
            writer.WriteLine($"Flags: {Flags:X02}");
            writer.WriteLine($"Charge Factor: {ChargeFactor}");
            writer.WriteLine($"Reserved: {Reserved:X02}");
        }
    }

}