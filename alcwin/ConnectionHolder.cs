using alclib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace alcwin
{
    public class ConnectionHolder
    {
        private string Name;
        private bool Network;
        private IAlcConnection Connection;
        private Mutex ConnectionLock;

        public ConnectionHolder(string p, bool n)
        {
            Network = n;
            Name = p;
            ConnectionLock = new Mutex();
        }

        public void Open()
        {
#if SIMFILE
            Connection = new AlcFileConnection(Port);
#else
            IAlcRawConnection raw = null;
            if (Network)
                raw = new AlcTcpConnection(Name); 
            else 
                raw = new AlcSerialConnection(Name);
            Connection = new AlcConnection(raw);
#endif
            Connection.Open();
        }

        public void Close()
        {
            Connection.Close();
        }

        public IAlcConnection GetConnection(bool wait = false)
        {
            if (wait)
            {
                ConnectionLock.WaitOne();
                return Connection;
            }
            else if (ConnectionLock.WaitOne(0))
                return Connection;
            return null;
        }

        public void ReleaseConnection()
        {
            ConnectionLock.ReleaseMutex();
        }
    }
}
