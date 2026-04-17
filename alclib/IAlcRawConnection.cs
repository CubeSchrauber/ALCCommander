using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public interface IAlcRawConnection
    {
        void Close();
        byte[] Cmd(byte[] data);
        void Open();
    }
}
