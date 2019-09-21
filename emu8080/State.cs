using System;

namespace emu8080
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
            get => NumbersUtils.GetValue(this.B, this.C);
            set
            {
                this.C = NumbersUtils.GetLow(value);
                this.B = NumbersUtils.GetHigh(value);
            }
        }

        public ushort DE
        {
            get => NumbersUtils.GetValue(this.D, this.E);
            set
            {
                this.E = NumbersUtils.GetLow(value);
                this.D = NumbersUtils.GetHigh(value);
            }
        }

        public ushort HL
        {
            get => NumbersUtils.GetValue(this.H, this.L);
            set
            {
                this.H = NumbersUtils.GetHigh(value);
                this.L = NumbersUtils.GetLow(value);
            }
        }

        public void SetCounterToAddr(ProgramInstructions instructions)
        {
            var lo = instructions[this.ProgramCounter+1];
            var hi = instructions[this.ProgramCounter+2];
            this.ProgramCounter = NumbersUtils.GetValue(hi, lo);
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
            ushort res = (ushort)(this.HL + value);
            this.Flags.CalcCarryFlag(res);
            this.HL = res;
            this.ProgramCounter++;
        }

        public override string ToString() {
            return $"SP: {StackPointer:X4} PC: {ProgramCounter:X4} A: {A:X} BC: {BC:X} DE: {DE:X} HL: {HL:X} Flags: {this.Flags}";
        }
    }
}