using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public static class LogConvert
    {
        public const int BatTypes = 9;
        public const int Functions = 9;

        public static int ToInt16(byte[] data, int pos)
        {
            return data[pos + 1] | (data[pos + 0] << 8);
        }

        public static string BatType(int batType)
        {
            string result = $"Unknown ({batType:x02})";
            switch (batType)
            {
                case 0: result = "NiCd"; break;
                case 1: result = "NIMH"; break;
                case 2: result = "Li-4.1"; break;
                case 3: result = "Li-4.2"; break;
                case 4: result = "Pb"; break;
                case 5: result = "LiFePO"; break;
                case 6: result = "Li4.35"; break;
                case 7: result = "NiZn"; break;
                case 8: result = "AGM/CA"; break;
            }

            return result;
        }

        public static string Function(int prog)
        {
            string result = $"Unknown ({prog:x02})";

            switch (prog)
            {
                case 0: result = "None"; break;
                case 1: result = "Charge"; break;
                case 2: result = "Discharge"; break;
                case 3: result = "Discharge/Charge"; break;
                case 4: result = "Test"; break;
                case 5: result = "Maintain"; break;
                case 6: result = "Forming"; break;
                case 7: result = "Cycle"; break;
                case 8: result = "Activate"; break;
            }

            return result;
        }

        public static double Capacity(int c)
        {
            return (double)c / 10000000.0;
        }

        public static double Current(int c)
        {
            return (double)c / 10000.0;
        }

        public static double Voltage(int v)
        {
            return (double)v / 1000.0;
        }

        public static string Pause(int p)
        {
            return $"{p / 60:d}:{p % 60:d2}";
        }

        public static (LogHeader, LogRecord[]) GetLog(Buffer data, int start, int end)
        {
            var h = new LogHeader();
            var l = new List<LogRecord>();
            int pos = start * 8;
            h.Init(data, ref pos);
            while (pos < end * 8)
            {
                var r = new LogRecord();
                r.Init(data, ref pos);
                l.Add(r);
            }

            return (h, l.ToArray());
        }

        public static LogSection[] SplitRecords(LogRecord[] records)
        {
            var result = new List<LogSection>();
            List<LogRecord> current = null;
            int currentIndex = 0;
            int index = 0;
            foreach(var r in records)
            {
                if (r.Valid)
                {
                    if (current == null)
                    {
                        current = new List<LogRecord>();
                        currentIndex = index;
                    }
                    current.Add(r);
                }
                else
                {
                    if (current != null)
                        result.Add(new LogSection(current.ToArray(), currentIndex));
                    current = null;
                }
                index++;
            }
            if (current != null)
                result.Add(new LogSection(current.ToArray(), currentIndex));

            return result.ToArray();
        }

#if false
        public static int[] GetLogInfo(byte[] info, int offset = 0)
        {
            int lastpointer = ToInt16(info, offset);
            int[] logpointer = new int[(info.Length - offset) / 2 - 1];
            int lastidx = 0;
            for (int i = 0; i < logpointer.Length; i++)
            {
                logpointer[i] = ToInt16(info, offset + i * 2 + 2);
                if (logpointer[i] == lastpointer)
                    lastidx = i;
            }
            int l = logpointer.Length;
            int firstidx = (lastidx + 2) % l;

            while (logpointer[firstidx] == 0xFFFF)
                firstidx = (firstidx + 1) % l;

            int idx = firstidx;
            var logs = new List<int>();
            while (true)
            {
                logs.Add(logpointer[idx]);
                if (idx == lastidx)
                    break;
                idx = (idx + 1) % l;
            }
            idx = (idx + 1) % l;
            if (logpointer[idx]!=0xFFFF)
                logs.Add(logpointer[idx]);
            return logs.ToArray();
        }
#else
        public static (int,int)[] GetLogInfo(int endptr, byte[] info, int offset = 0)
        {
            int lastpointer = ToInt16(info, offset);
            if (lastpointer == 65535 || endptr==65535)
                return null;

            int[] logpointer = new int[(info.Length - offset) / 2 - 1];
            int lastidx = -1;
            int endidx = -1;
            for (int i = 0; i < logpointer.Length; i++)
            {
                logpointer[i] = ToInt16(info, offset + i * 2 + 2);
                if (logpointer[i] == lastpointer)
                    lastidx = i;
                if (logpointer[i] == endptr)
                    endidx = i;
            }
            if (endidx < 0 || lastidx < 0)
                return null;

            var l = new List<(int, int)>();
            while(l.Count<11)
            {
                if (logpointer[lastidx] == 0xFFFF)
                    break;
                int s = logpointer[lastidx];
                int e = logpointer[endidx];
                if (e < s)
                    e += 65000;
                l.Insert(0, (s, e));
                if (--lastidx < 0)
                    lastidx = logpointer.Length - 1;
                if (--endidx < 0)
                    endidx = logpointer.Length - 1;
                if (lastidx == endidx)
                    break;
            }

            return l.ToArray();
        }
#endif
        public static T Parse<T>(Buffer data) where T : Initializable, new()
        {
            T result = null;
            if (data != null)
            {
                result = new T();
                int pos = 0;
                result.Init(data, ref pos);
                if (pos != data.Length)
                    throw new ArgumentException();
            }
            return result;
        }

        public static bool LogDeleted(int channel, byte[] bytes)
        {
            return bytes.Length == 2 && bytes[0] == (byte)'l' && bytes[1] == (byte)channel;
        }
    }
}
