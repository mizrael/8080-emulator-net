using System.Linq;

namespace emu8080.Core
{
    public class Memory
    {
        private readonly byte[] _bytes;
        private byte[] _videoBuffer;

        public const ushort videoBufferStartAddress = 0x2400;
        public const ushort videoBufferEndAddress = 0x4000;
        public const int videoBufferSize = videoBufferEndAddress - videoBufferStartAddress;
        
        private Memory(byte[] data)
        {
            _bytes = data;
            _videoBuffer = new byte[videoBufferSize];
        }

        public byte this[int index] {
            get => _bytes[index];
            set => _bytes[index] = value;
        }

        public byte[] VideoBuffer => _videoBuffer;

        public void UpdateVideoBuffer()
        {
            System.Buffer.BlockCopy(_bytes, videoBufferStartAddress, _videoBuffer, 0, videoBufferSize);
            //_videoBuffer = _bytes.Skip(videoBufferStartAddress).Take(videoBufferEndAddress - videoBufferStartAddress).Reverse().ToArray();
        }

        public static Memory Load(byte[] data){
            var destBytes = new byte[0x10000]; // 16bit
            System.Array.Copy(data, destBytes, data.Length);

            var memory = new Memory(destBytes);
            return memory;
        }
    }
}