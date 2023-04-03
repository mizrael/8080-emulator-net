using System;

namespace emu8080.Core
{
    public class Memory
    {
        private readonly byte[] _bytes;
        private ReadOnlyMemory<byte> _videoBuffer;
        private const ushort videoBufferStartAddress = 0x2400;
        private const ushort videoBufferSize = 224 * 256 / 8;
        
        private Memory(byte[] data)
        {
            _bytes = data;
             
            _videoBuffer = new ReadOnlyMemory<byte>(data, videoBufferStartAddress, videoBufferSize);
        }

        public byte this[int index] {
            get => _bytes[index];
            set => _bytes[index] = value;
        }

        public ushort ReadAddress(int startLoc)
        {
            var lo = _bytes[startLoc];
            var hi = _bytes[startLoc + 1];
            return Utils.GetValue(hi, lo);
        }

        public ReadOnlyMemory<byte> VideoBuffer => _videoBuffer;

        public static Memory Load(byte[] data, int destLoc = 0)
        {
            var destBytes = new byte[0x10000]; // 64kb
            System.Array.Copy(data, 0, destBytes, destLoc, data.Length);

            var memory = new Memory(destBytes);
            return memory;
        }
    }
}