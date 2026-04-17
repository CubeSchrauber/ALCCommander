using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public interface IAlcConnection
    {
        void Close();
        Buffer Cmd(byte[] data);
        void Open();
        Buffer GetChargeState(int channel);
        Buffer GetDbEntry(int index);
        Buffer GetDeviceParameters();
        Buffer GetDeviceParametersEx1();
        Buffer GetDeviceParametersEx2();
        Buffer GetDeviceParametersEx3();
        Buffer GetDeviceParametersEx4();
        Buffer GetLogBlock(int channel, int block);
        Buffer GetLogInfo(int channel);
        Buffer GetMetric(int channel);
        Buffer GetParameter(int channel);
        Buffer Start(int channel);
        Buffer Stop(int channel);
        Buffer DeleteLog(int channel);
    }
}
