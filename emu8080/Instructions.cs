using System.Collections.Generic;
using System.Linq;

namespace emu8080
{
    public class Instructions
    {
        private readonly byte[] _bytes;

        private Instructions(byte[] data)
        {
            _bytes = data;
        }

        public byte this[int index] {
            get => _bytes[index];
            set => _bytes[index] = value;
        }

        public static Instructions Load(byte[] data){
            var destBytes = new byte[0x10000]; // 16bit
            System.Array.Copy(data, destBytes, data.Length);

            var instructions = new Instructions(destBytes);
            return instructions;
        }
    }
}