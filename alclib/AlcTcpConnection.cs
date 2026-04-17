using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class AlcTcpConnection : IAlcRawConnection
    {
        private string Hostname;
        private TcpClient TcpClient;
        private NetworkStream Stream;

        public AlcTcpConnection(string host)
        {
            Hostname = host;
        }

        public void Open()
        {
            TcpClient = new TcpClient(Hostname, 2318);
            TcpClient.ReceiveTimeout = 1000;
            TcpClient.SendTimeout = 1000;
            Stream = TcpClient.GetStream();
        }

        public void Close()
        {
            var writer = new BinaryWriter(Stream);
            writer.Write((ushort)0);
            Stream.Close();
            TcpClient.Close();
        }

        public byte[] Cmd(byte[] buffer)
        {
            Stream.Write(buffer, 0, buffer.Length);

            var ms = new MemoryStream();
            while (Stream.ReadByte() != 2) ;
            ms.WriteByte(2);
            while(true)
            {
                int b = Stream.ReadByte();
                ms.WriteByte((byte)b);
                if (b == 3)
                    break;
            }
            return ms.ToArray();
        }

    }
}
