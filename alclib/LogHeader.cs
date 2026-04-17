using System;
using System.IO;

namespace alclib
{
    [Serializable]
    public class LogHeader : Initializable
    {
        public int BatIndex;
        public int Program;
        public int Second, Minute, Hour, Day, Month, Year;
        public int BatType;
        public int Cells;
        public int Capacity;
        public int ChargeCurrent;

        public int BatType2;
        public int Cells2;
        public int DischargeCurrent;
        public int FormingCurrent;
        public int Pause;

        public override void Init(Buffer data, ref int pos)
        {
            BatIndex = data[pos++];
            Program = data[pos++];
            Second = data[pos++];
            Minute = data[pos++];
            Hour = data[pos++];
            Day = data[pos++];
            Month = data[pos++];
            Year = data[pos++];

            BatType = data[pos++];
            Cells = data[pos++];
            Capacity = ToInt32(data, ref pos);
            ChargeCurrent = ToInt16(data, ref pos);

            BatType2 = data[pos++];
            Cells2 = data[pos++];
            DischargeCurrent = ToInt16(data, ref pos);
            FormingCurrent = ToInt16(data, ref pos);
            Pause = ToInt16(data, ref pos);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine($"Battery Index: {BatIndex}");
            writer.WriteLine($"Function: {LogConvert.Function(Program)} ({Program})");
            writer.WriteLine($"Time: {Year:d2}-{Month:d2}-{Day:d2}-{Hour:d2}-{Minute:d2}-{Second:d2}");
            writer.WriteLine($"Battery type: {LogConvert.BatType(BatType)} ({BatType})");
            writer.WriteLine($"Cells: {Cells}");
            writer.WriteLine($"Capacity: {LogConvert.Capacity(Capacity):f3}Ah ({Capacity})");
            writer.WriteLine($"Charge Current: {LogConvert.Current(ChargeCurrent):f3}A ({ChargeCurrent})");

            writer.WriteLine($"Battery type 2: {LogConvert.BatType(BatType2)} ({BatType2})");
            writer.WriteLine($"Cells 2: {Cells2}");
            writer.WriteLine($"Discharge Current: {LogConvert.Current(DischargeCurrent):f3}A ({DischargeCurrent})");
            writer.WriteLine($"Forming Current: {LogConvert.Current(FormingCurrent):f3}A ({FormingCurrent})");
            writer.WriteLine($"Pause: {LogConvert.Pause(Pause)}min ({Pause})");

        }
    }
}