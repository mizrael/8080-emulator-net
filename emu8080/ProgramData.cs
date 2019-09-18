using System.Collections.Generic;
using System.Linq;

namespace emu8080
{
    public class ProgramData
    {
        private readonly byte[] _bytes;

        public ProgramData(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToArray();
        }

        public byte this[int index] => _bytes[index];
        public int Length => _bytes.Length;
    }
}