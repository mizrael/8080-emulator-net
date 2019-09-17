namespace emu8080
{
    public class Registers
    {
        public Registers(){
            this.Flags = new Flags();
        }

        public byte A = 0;
        public byte B = 0;
        public byte C = 0;
        public byte D = 0;
        public byte E = 0;
        public byte H = 0;
        public byte L = 0;
        public readonly Flags Flags;
    }

    public class Flags{
        public byte Z = 0;
        public byte S = 0;
        public byte P = 0;
        public byte CY = 0;
        public byte AC = 0;
        public byte Pad = 0;
    }
}