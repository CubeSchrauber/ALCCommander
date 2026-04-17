using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace alclib
{
    [Obsolete]
    public class AlcRawPortConnection : IAlcRawConnection
    {
        public SerialPort Serial;
        public ManualResetEvent StopEvent;
        public Thread ReceiveThread;
        public string PortName;

        private const int AnswerTimeout = 5 * 1000;
        private byte[] Answer = null;
        private object AnswerLock = new object();

        public AlcRawPortConnection(string Port)
        {
            PortName = Port;
        }

        public void Open()
        {
            Close();

            StopEvent = new ManualResetEvent(false);
            Serial = new SerialPort(PortName);
            Serial.BaudRate = 38400;
            Serial.Parity = Parity.Even;
            Serial.StopBits = StopBits.One;
            Serial.Open();

            ReceiveThread = new Thread(Receiver);
            ReceiveThread.Start();
        }

        public void Close()
        {
            if (Serial != null)
            {
                StopEvent.Set();
                ReceiveThread.Join();
                Serial.Close();
                Serial.Dispose();
                Serial = null;
            }
        }

        public void Write(byte[] data)
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
            data = ms.ToArray();
            Serial.Write(data, 0, data.Length);
        }

        public byte[] GetAnswer()
        {
            byte[] ret = null;
            lock (AnswerLock)
            {
                while (Answer == null)
                    if (!Monitor.Wait(AnswerLock, AnswerTimeout))
                        return null;
                ret = Answer;
                Answer = null;
                Monitor.Pulse(AnswerLock);
            }
            return ret;
        }

        private void Receiver(object state)
        {
            var buffer = new byte[1024];
            try
            {
                MemoryStream ms = null;
                bool escaped = false;

                while (true)
                {
                    var a = Serial.BaseStream.BeginRead(buffer, 0, buffer.Length, null, null);
                    int what = WaitHandle.WaitAny(new WaitHandle[] { StopEvent, a.AsyncWaitHandle });
                    if (what == 0)
                        break;
                    if (what != 1)
                        continue;
                    int bytes = Serial.BaseStream.EndRead(a);

                    for (int i = 0; i < bytes; i++)
                        if (escaped)
                        {
                            ms?.WriteByte((byte)(buffer[i] - 0x10));
                            escaped = false;
                        }
                        else if (buffer[i] == 5)
                            escaped = true;
                        else if (buffer[i] == 2)
                            ms = new MemoryStream();
                        else if (buffer[i] == 3)
                        {
                            lock (AnswerLock)
                            {
                                while (Answer != null)
                                    Monitor.Wait(AnswerLock);
                                Answer = ms?.ToArray();
                                Monitor.Pulse(AnswerLock);
                            }
                            ms = null;
                        }
                        else ms?.WriteByte(buffer[i]);
                }
            }
            catch
            {
            }
        }

        public byte[] Cmd(byte[] cmd)
        {
            byte[] data = null;

            for (int i = 0; i < 5; i++)
            {
                Write(cmd);
                var d = GetAnswer();
                if (d == null)
                    break;
                if (d.Length != 1 || d[0] != 4)
                {
                    data = d;
                    break;
                }
            }

            return data;
        }
    }
}
