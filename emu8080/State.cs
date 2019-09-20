﻿namespace emu8080
{
    public class State
    {
        public State(){
            this.ConditionalFlags = new ConditionalFlags();
            this.Stack = new byte[0x10000]; // 16bit
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
        public readonly byte[] Stack;

        public ushort BC => NumbersUtils.GetValue(this.C, this.B);
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

        public void DAD(ushort value)
        {
            ushort res = (ushort)(this.HL + value);
            this.ConditionalFlags.CalcCarryFlag(res);
            this.HL = res;
        }
    }
}