using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.FormattableString;

namespace alclib
{
    // a: ChargeState
    // b: + channel ?
    // c: ?
    // d: DB entry
    // e: device parameters ex3
    // f: timeout
    // g: device parameters
    // h: device parameters ex1
    // i: loginfo
    // j: device parameters ex2
    // k: timeout
    // l: timeout
    // m: metric
    // n: timeout
    // o: timeout
    // p: Parameter
    // q: timeout
    // r: timeout
    // s: ?
    // t: temperature
    // u: ?
    // v: log block
    // w: timeout
    // x: timeout
    // y: timeout
    // z: device parameters ex4

    public class AlcConnection : IAlcConnection
    {
        public IAlcRawConnection Inner;

        public AlcConnection(IAlcRawConnection raw)
        {
            Inner = raw;
        }

        public void Open() => Inner.Open();
        public void Close() => Inner.Close();

        public Buffer GetDbEntry(int index) => Cmd(new byte[] { (byte)'d', (byte)index });
        public Buffer GetLogBlock(int channel, int block) => Cmd(new byte[] { (byte)'v', (byte)channel, (byte)(block >> 8), (byte)(block & 255) });
        public Buffer GetLogInfo(int channel) => Cmd(new byte[] { (byte)'i', (byte)channel });
        public Buffer GetParameter(int channel) => Cmd(new byte[] { (byte)'p', (byte)channel });
        public Buffer SetParameter(byte[] p) => Cmd(p);
        public Buffer Start(int channel) => Cmd(new byte[] { (byte)'A', (byte)channel, 0 });
        public Buffer Stop(int channel) => Cmd(new byte[] { (byte)'A', (byte)channel, 1 });
        public Buffer GetChargeState(int channel) => Cmd(new byte[] { (byte)'a', (byte)channel });
        public Buffer GetMetric(int channel) => Cmd(new byte[] { (byte)'m', (byte)channel });
        public Buffer GetTemperatures() => Cmd(new byte[] { (byte)'t' });
        public Buffer GetDeviceParameters() => Cmd(new byte[] { (byte)'g' });
        public Buffer GetDeviceParametersEx1() => Cmd(new byte[] { (byte)'h' });
        public Buffer GetDeviceParametersEx2() => Cmd(new byte[] { (byte)'j' });
        public Buffer GetDeviceParametersEx3() => Cmd(new byte[] { (byte)'e' });
        public Buffer GetDeviceParametersEx4() => Cmd(new byte[] { (byte)'z' });
        public Buffer DeleteLog(int channel) => Cmd(new byte[] { (byte)'L', (byte)channel });
        
        public Buffer Cmd(byte[] data)
        {
            data = Escape(data);

            byte[] result = null;
            for (int i=0; i<5; i++)
            {
                result = Unescape(Inner.Cmd(data));
                if (result.Length != 1 || result[0] != 4)
                    break;
            }
            return new Buffer(result);
        }

        private static byte[] Escape(byte[] data)
        {
            var ms = new MemoryStream();
            ms.WriteByte(2);
            foreach (var b in data)
            {
                if (b == 2 || b == 3 || b == 5)
                {
                    ms.WriteByte(5);
                    ms.WriteByte((byte)(b + 0x10));
                }
                else ms.WriteByte(b);
            }
            ms.WriteByte(3);
            return ms.ToArray();
        }

        private static byte[] Unescape(byte[] data)
        {
            MemoryStream ms = null;
            bool escaped = false;
            for (int i = 0; i < data.Length; i++)
            {
                if (escaped)
                {
                    ms?.WriteByte((byte)(data[i] - 0x10));
                    escaped = false;
                }
                else if (data[i] == 5)
                    escaped = true;
                else if (data[i] == 2)
                    ms = new MemoryStream();
                else if (data[i] == 3)
                    break;
                else
                    ms?.WriteByte(data[i]);
            }
            return ms?.ToArray();
        }

        public LogRaw GetLog(int channel)
        {
            var p = LogConvert.Parse<ChannelParameter>(GetParameter(channel));
            int records = p.LogEnd;
            int blocks = 650;

            byte[] info = GetLogInfo(channel);
            byte[] resultinfo = new byte[24];
            Array.Copy(info, 2, resultinfo, 0, 24);

            byte[] alldata = new byte[blocks * 800];
            for (int b = 0; b < blocks; b++)
            {
                byte[] data = GetLogBlock(channel, b);
                if (data.Length != 804)
                    throw new IOException("wrong block length");

                Array.Copy(data, 4, alldata, b * 800, 800);
            }

            return new LogRaw { Data = new Buffer(alldata), Info=resultinfo, Records=records };
        }
    }
}
