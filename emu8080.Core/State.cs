namespace emu8080.Core
{
    public class State
    {
        public State(){
            this.Flags = new Flags();
        }

        public byte A = 0;
        public byte B = 0;
        public byte C = 0;
        public byte D = 0;
        public byte E = 0;
        public byte H = 0;
        public byte L = 0;
        public ushort StackPointer = 0;
        public ushort ProgramCounter = 0;
        public readonly Flags Flags;        
        public ushort BC
        {
            get => Utils.GetValue(this.B, this.C);
            set
            {
                this.C = Utils.GetLow(value);
                this.B = Utils.GetHigh(value);
            }
        }

        public ushort DE
        {
            get => Utils.GetValue(this.D, this.E);
            set
            {
                this.E = Utils.GetLow(value);
                this.D = Utils.GetHigh(value);
            }
        }

        public ushort HL
        {
            get => Utils.GetValue(this.H, this.L);
            set
            {
                this.H = Utils.GetHigh(value);
                this.L = Utils.GetLow(value);
            }
        }

        public void SetCounterToAddr(Memory memory)
        {
            var lo = memory[this.ProgramCounter+1];
            var hi = memory[this.ProgramCounter+2];
            this.ProgramCounter = Utils.GetValue(hi, lo);
        }

        public void Reset()
        {
            this.A = this.B = this.C = this.D = this.E = this.H = this.L = 0;
            this.ProgramCounter = 0;
            this.StackPointer = 0;
            this.Flags.Reset();
        }

        public void DAD(ushort value)
        {
            int res = this.HL + value;
            this.H = (byte) ((res & 0xFF00) >> 8);
            this.L = (byte) (res & 0xff);
            this.Flags.CalcCarryFlag(res);
            this.ProgramCounter++;
        }

        public override string ToString() {
            return $"SP: {StackPointer:X4} PC: {ProgramCounter:X4} A: {A:X} BC: {BC:X} DE: {DE:X} HL: {HL:X} Flags: {this.Flags}";
        }
    }
}