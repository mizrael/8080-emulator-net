using System;

namespace emu8080
{
    public class State
    {
        public State(){
            this.ConditionalFlags = new ConditionalFlags();
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
        public readonly ConditionalFlags ConditionalFlags;        
        public ushort BC
        {
            get => NumbersUtils.GetValue(this.C, this.B);
            set
            {
                this.C = NumbersUtils.GetHigh(value);
                this.B = NumbersUtils.GetLow(value);
            }
        }

        public ushort DE
        {
            get => NumbersUtils.GetValue(this.E, this.D);
            set
            {
                this.E = NumbersUtils.GetHigh(value);
                this.D = NumbersUtils.GetLow(value);
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

        public void SetCounterToAddr(Instructions instructions)
        {
            var lo = instructions[++this.ProgramCounter];
            var hi = instructions[++this.ProgramCounter];
            this.ProgramCounter = NumbersUtils.GetValue(hi, lo);
        }

        public void Reset()
        {
            this.A = this.B = this.C = this.D = this.E = this.H = this.L = 0;
            this.ProgramCounter = 0;
            this.StackPointer = 0;
            this.ConditionalFlags.Reset();
        }

        public void DAD(ushort value)
        {
            ushort res = (ushort)(this.HL + value);
            this.ConditionalFlags.CalcCarryFlag(res);
            this.HL = res;
        }

        public override string ToString() {
            return $"SP: {StackPointer} PC: {ProgramCounter} A: {A:X} B: {B:X} C: {C:X} D: {D:X} E: {E:X} H: {H:X} L: {L:X} Flags: {this.ConditionalFlags}";
        }
    }
}