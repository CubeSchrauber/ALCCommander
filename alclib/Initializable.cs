using System;
using System.IO;

namespace alclib
{
    public abstract class Initializable
    {
        public abstract void Init(Buffer data, ref int pos);

        public static int ToInt16(Buffer data, ref int pos)
        {
            int result = data[pos + 1] | (data[pos + 0] << 8);
            pos += 2;
            return result;
        }

        public static int ToInt32(Buffer data, ref int pos)
        {
            int result = data[pos + 3] | (data[pos + 2] << 8) | (data[pos + 1] << 16) | (data[pos + 0] << 24);
            pos += 4;
            return result;
        }

        public static void WriteInt16(MemoryStream stream, int value)
        {
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value & 255));
        }

    }
}