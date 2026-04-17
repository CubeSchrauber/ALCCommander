using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class AlcSerialConnection : IAlcRawConnection
    {
        public SerialPort Serial;
        public string PortName;

        public AlcSerialConnection(string Port)
        {
            PortName = Port;
        }

        public void Open()
        {
            Close();

            Serial = new SerialPort(PortName);
            Serial.BaudRate = 38400;
            Serial.Parity = Parity.Even;
            Serial.StopBits = StopBits.One;
            Serial.ReadTimeout = 500;
            Serial.Open();
        }

        public void Close()
        {
            if (Serial != null)
            {
                Serial.Close();
                Serial.Dispose();
                Serial = null;
            }
        }

        public byte[] Cmd(byte [] data)
        {
            Serial.Write(data, 0, data.Length);
            var ms = new MemoryStream();
            byte[] buffer = new byte[256];

            try
            {
                bool eol = false;
                while (!eol)
                {
                    int bytes = Serial.Read(buffer, 0, buffer.Length);
                    if (bytes == 0)
                        break;
                    for (int i = 0; i < bytes; i++)
                    {
                        ms.WriteByte(buffer[i]);
                        if (buffer[i] == 3)
                        {
                            eol = true;
                            break;
                        }
                    }
                }
            }
            catch(TimeoutException)
            {
                return null;
            }
            return ms.ToArray();
        }
    }
}
