using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class LogSection
    {
        public LogRecord[] Data { get; private set; }
        public int Index { get; private set; }

        public int Count => Data.Length;

        public LogSection(LogRecord[] data, int index)
        {
            Data = data;
            Index = index;
        }
    }
}
