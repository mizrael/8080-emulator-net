namespace emu8080
{
    public static class NumbersUtils
    {
        public static byte GetLow(this int value)
        {
            return (byte)(value & 0xff);
        }

        public static byte GetHigh(this int value)
        {
            return (byte)((value >> 8) & 0xff);
        }

        public static byte GetLow(this short value)
        {
            return (byte)(value & 0xff);
        }

        public static byte GetHigh(this short value)
        {
            return (byte)((value >> 8) & 0xff);
        }

        public static ushort GetValue(byte hi, byte lo)
        {
            return (ushort) (((hi << 8) | lo) & 0xff);
        }
    }
}