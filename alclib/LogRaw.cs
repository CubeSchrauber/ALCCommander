using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class LogRaw
    {
        public int Records;
        public byte[] Info;
        public Buffer Data;

        public void Save(string Filename)
        {
            using (var stream = new FileStream(Filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(Records);
                writer.Write(Data.Length);
                writer.Write(Info);
                writer.Write(Data);
                stream.Close();
            }
        }

        public static LogRaw Load(string Filename)
        {
            var result = new LogRaw();
            using (var stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new BinaryReader(stream);
                result.Records = reader.ReadInt32();
                int length = reader.ReadInt32();
                result.Info = reader.ReadBytes(24);
                result.Data = new Buffer(reader.ReadBytes(length));
                stream.Close();
            }
            return result;
        }
    }
}
