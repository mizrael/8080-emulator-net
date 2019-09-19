using System.Collections.Generic;
using System.Linq;

namespace emu8080
{
    public class Instructions
    {
        private readonly byte[] _bytes;

        public Instructions(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToArray();
        }

        public byte this[int index] => _bytes[index];
        public int Length => _bytes.Length;
    }
}