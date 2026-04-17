using System;
using System.IO;

namespace alclib
{
    [Serializable]
    public class LogRecord : Initializable
    {
        public int Voltage;
        public int Current;
        public int Capacity;
        public byte[] Raw;
        
        public bool Valid => Voltage >= 0 && Current >= 0 && Capacity >= 0;

        public override void Init(Buffer data, ref int pos)
        {
            int p = pos;
            Voltage = ToInt16(data, ref pos);
            if (data[pos] != 0xFF || data[pos + 1] != 0xFF)
                Current = ToInt16(data, ref pos);
            else
            {
                Current = -1;
                pos += 2;
            }
            if (data[pos] != 0xFF || data[pos + 1] != 0xFF)
                Capacity = ToInt32(data, ref pos);
            else
            {
                Capacity = -1;
                pos += 4;
            }
            int l = pos - p;
            Raw = new byte[l];
            for (int i = 0; i < l; i++)
                Raw[i] = data[p + i];
        }

        public void Write(TextWriter writer)
        {
            writer.Write($"{LogConvert.Voltage(Voltage),7:f3}V");
            if (Current >= 0)
                writer.Write($"{LogConvert.Current(Current),7:f3}A");
            else
                writer.Write("        ");
            if (Capacity >= 0)
                writer.Write($"{LogConvert.Capacity(Capacity),9:f3}Ah");
            else
                writer.Write("           ");
            writer.Write("  | ");
            for (int i = 0; i < Raw.Length; i++)
                writer.Write($" {Raw[i]:X2}");
            writer.WriteLine();
        }
    }
}