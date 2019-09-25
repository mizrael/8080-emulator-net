using System.Linq;

namespace emu8080
{
    public class Memory
    {
        private readonly byte[] _bytes;

        public const ushort videoBufferStartAddress = 0x2400;
        public const ushort videoBufferEndAddress = 0x4000;
        public const int videoBufferSize = videoBufferEndAddress - videoBufferStartAddress;
        
        private Memory(byte[] data)
        {
            _bytes = data;
        }

        public byte this[int index] {
            get => _bytes[index];
            set => _bytes[index] = value;
        }

        public void GetVideoBuffer(byte[] videoBuffer)
        {
            System.Buffer.BlockCopy(_bytes, videoBufferStartAddress, videoBuffer, 0, videoBufferSize);
           //return _bytes.Skip(0x2400).Take(0x4000 - 0x2400).Reverse().ToArray();
        }

        public static Memory Load(byte[] data){
            var destBytes = new byte[0x10000]; // 16bit
            System.Array.Copy(data, destBytes, data.Length);

            var memory = new Memory(destBytes);
            return memory;
        }
    }
}