using System;
using System.IO;
using static System.FormattableString;

namespace alclib
{
    public class Temperatures : Initializable
    {
        public int Battery;
        public int Transformer;
        public int HeatSink;

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 't')
                throw new ArgumentException();
            Battery = ToInt16(data, ref pos);
            Transformer = ToInt16(data, ref pos);
            HeatSink = ToInt16(data, ref pos);
        }

        public void Dump(TextWriter writer)
        {
            writer.WriteLine(Invariant($"Battery Temperature: {(decimal)Battery / 100m:F2}°C"));
            writer.WriteLine(Invariant($"Transformer Temperature: {(decimal)Transformer / 100m:F2}°C"));
            writer.WriteLine(Invariant($"Heat Sink Temperature: {(decimal)HeatSink / 100m:F2}°C"));
        }
    }

}