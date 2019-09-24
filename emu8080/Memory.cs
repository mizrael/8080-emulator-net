namespace emu8080
{
    public class Memory
    {
        private readonly byte[] _bytes;

        private Memory(byte[] data)
        {
            _bytes = data;
        }

        public byte this[int index] {
            get => _bytes[index];
            set => _bytes[index] = value;
        }

        public static Memory Load(byte[] data){
            var destBytes = new byte[0x10000]; // 16bit
            System.Array.Copy(data, destBytes, data.Length);

            var memory = new Memory(destBytes);
            return memory;
        }
    }
}