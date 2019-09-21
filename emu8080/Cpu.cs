using System;
using System.Collections.Generic;

namespace emu8080
{
    public class Cpu
    {
        private readonly Dictionary<byte, Action<ProgramInstructions, State>> _ops;
        public State State {get;}

        public Cpu(State state)
        {
            State = state;

            _ops = new Dictionary<byte, Action<ProgramInstructions, State>>();
            _ops.Add(0x00, Ops.NOP);
            _ops.Add(0x01, Ops.LXI_B);
            _ops.Add(0x05, Ops.DCR_B);
            _ops.Add(0x06, Ops.MVI_B);
            _ops.Add(0x09, Ops.DAD_B);
            _ops.Add(0x0d, Ops.DCR_C);
            _ops.Add(0x0e, Ops.MVI_C);
            _ops.Add(0x0f, Ops.RRC);
            _ops.Add(0x11, Ops.LXI_D);
            _ops.Add(0x13, Ops.INX_D);
            _ops.Add(0x19, Ops.DAD_D);
            _ops.Add(0x1a, Ops.LDAX_D);
            _ops.Add(0x1f, Ops.RAR);
            _ops.Add(0x20, Ops.NOP);
            _ops.Add(0x21, Ops.LXI_H);
            _ops.Add(0x23, Ops.INX_H);
            _ops.Add(0x26, Ops.MVI_H);
            _ops.Add(0x29, Ops.DAD_H);
            _ops.Add(0x2f, Ops.CMA);
            _ops.Add(0x30, Ops.RNZ);
            _ops.Add(0x31, Ops.LXI_SP);
            _ops.Add(0x41, Ops.MOV_B_C);
            _ops.Add(0x42, Ops.MOV_B_D);
            _ops.Add(0x43, Ops.MOV_B_E);
            _ops.Add(0x77, Ops.MOV_M_A);
            _ops.Add(0xc1, Ops.POP);
            _ops.Add(0xc2, Ops.JNZ);
            _ops.Add(0xc3, Ops.JMP);
            _ops.Add(0xc5, Ops.PUSH);
            _ops.Add(0xc9, Ops.RET);
            _ops.Add(0xcd, Ops.CALL);
            _ops.Add(0xf1, Ops.POP_PSW);
            _ops.Add(0xfe, Ops.CPI);
        }

        public void Reset() => State.Reset();

        public byte Step(ProgramInstructions instructions)
        {
            var op = instructions[State.ProgramCounter];

            if (_ops.ContainsKey(op)){
                _ops[op](instructions, State);
            }else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("not implemented!");
                Console.ResetColor();
            }

            return op;
        }
    }

    public struct Instruction{
        public Instruction(byte op, byte a1, byte a2){
            this.Op = op;
            this.Arg1 = a1;
            this.Arg2 = a2;
        }

        public byte Op {get;}
        public byte Arg1 {get;}
        public byte Arg2 {get;}

        public static Instruction Build(ProgramInstructions instructions, State state){
            return new Instruction(
                instructions[state.ProgramCounter],
                instructions[state.ProgramCounter+1],
                instructions[state.ProgramCounter+2]
            );
        }
    }
}