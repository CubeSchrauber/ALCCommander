using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace alclib
{
    public class AlcFileConnection : IAlcConnection
    {
        private DirectoryInfo DataDir;
        private Regex ChannelPattern;
        private Regex DbPattern;

        private FileInfo[] ChannelFile;
        private FileInfo DbFile;
        private LogRaw[] Logs;
        private byte[] DbData;


        public AlcFileConnection(string directory)
        {
            DataDir = new DirectoryInfo(directory);
            ChannelPattern = new Regex(@"\d{4}-\d{2}-\d{2}-(?:-\d{2}){3}-channel(\d)\.dat", RegexOptions.Compiled|RegexOptions.IgnoreCase);
            DbPattern = new Regex(@"\d{4}-\d{2}-\d{2}-(?:-\d{2}){3}-db\.dat", RegexOptions.Compiled|RegexOptions.IgnoreCase);
        }

        public void Open() 
        {
            ChannelFile = new FileInfo[4];
            foreach(var file in DataDir.GetFiles("*.dat"))
            {
                var match = ChannelPattern.Match(file.Name);
                if (match.Success)
                {
                    int channel = Convert.ToInt32(match.Groups[1].Value) - 1;
                    if (channel < 0 || channel > 3)
                        continue;
                    if (ChannelFile[channel] == null)
                        ChannelFile[channel] = file;
                    else if (ChannelFile[channel].LastWriteTime < file.LastWriteTime)
                        ChannelFile[channel] = file;
                }

                if (DbPattern.IsMatch(file.Name))
                {
                    if (DbFile == null)
                        DbFile = file;
                    else if (DbFile.LastWriteTime < file.LastWriteTime)
                        DbFile = file;
                }

                if (DbFile != null)
                    DbData = File.ReadAllBytes(DbFile.FullName);
            }

            Logs = new LogRaw[4];
            for (int i = 0; i < 4; i++)
                Logs[i] = LogRaw.Load(ChannelFile[i].FullName);
        }

        public void Close()
        {

        }

        public Buffer GetChargeState(int channel)
        {
            return null;
        }

        public Buffer GetLogBlock(int channel, int block)
        {
            var raw = Logs[channel];
            var result = new byte[804];
            result[0] = (byte)'v';
            result[1] = (byte)channel;
            result[2] = (byte)(block >> 8);
            result[3] = (byte)(block & 255);
            Array.Copy(raw.Data, block * 800, result, 4, 800);
            return new Buffer(result);
        }

        public Buffer GetLogInfo(int channel)
        {
            var raw = Logs[channel];
            var result = new byte[raw.Info.Length + 2];
            result[0] = (byte)'i';
            result[1] = (byte)channel;
            Array.Copy(raw.Info, 0, result, 2, raw.Info.Length);
            return new Buffer(result);
        }

        public Buffer GetDbEntry(int index)
        {
            int pos = index * 24;
            byte[] result = new byte[26];
            result[0] = (byte)'d';
            result[1] = (byte)index;
            if (DbData != null)
                Array.Copy(DbData, index * 24, result, 2, 24);
            return new Buffer(result);
        }

        public Buffer GetMetric(int channel)
        {
            return null;
        }

        public Buffer GetParameter(int channel)
        {
            return null;
        }

        public Buffer Cmd(byte[] p)
        {
            throw new NotImplementedException();
        }

        public Buffer Start(int channel)
        {
            throw new NotImplementedException();
        }

        public Buffer Stop(int channel)
        {
            throw new NotImplementedException();
        }

        public Buffer GetDeviceParameters()
        {
            throw new NotImplementedException();
        }

        public Buffer GetDeviceParametersEx1()
        {
            throw new NotImplementedException();
        }
        public Buffer GetDeviceParametersEx2()
        {
            throw new NotImplementedException();
        }

        public Buffer GetDeviceParametersEx3()
        {
            throw new NotImplementedException();
        }

        public Buffer GetDeviceParametersEx4()
        {
            throw new NotImplementedException();
        }

        public Buffer DeleteLog(int channel)
        {
            return null;
        }
    }
}
