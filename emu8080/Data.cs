using System.Collections.Generic;
using System.Linq;

namespace emu8080
{
    public class Data
    {
        private readonly byte[] _bytes;
        private int _pc;

        public Data(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToArray();
            _pc = 0;
        }

        public byte GetCurrent()
        {
            return _bytes[_pc];
        }

        public bool Increment(){
            return this.SetAddress(_pc+1);
        }

        public bool SetAddress(int address){
            _pc = address; 
            return _pc < _bytes.Length;
        }
    }
}