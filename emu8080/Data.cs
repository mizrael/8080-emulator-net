using System.Collections.Generic;
using System.Linq;

namespace emu8080
{
    public class Data
    {
        private readonly byte[] _bytes;
        public int PC;

        public Data(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToArray();
            this.PC = 0;
        }

        public byte GetCurrent()
        {
            return this._bytes[this.PC];
        }
    }
}