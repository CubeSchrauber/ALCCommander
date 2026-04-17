using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alclib
{
    public class Buffer : IEnumerable<byte>
    {
        private byte[] Data;

        public Buffer(byte[] data)
        {
            this.Data = data;
        }

        public byte this[int idx] => Data[idx%Data.Length];
        public int Length => Data.Length;

        public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)Data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();

        public static implicit operator byte[](Buffer data) => data.Data;

    }
}
