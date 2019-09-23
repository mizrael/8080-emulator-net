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
            _ops.Add(0x03, Ops.INX_B);
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
            _ops.Add(0x31, Ops.LXI_SP);
            _ops.Add(0x32, Ops.STA);
            _ops.Add(0x36, Ops.MVI_M);
            _ops.Add(0x3a, Ops.LDA);
            _ops.Add(0x3e, Ops.MVI_A);
            _ops.Add(0x41, Ops.MOV_B_C);
            _ops.Add(0x42, Ops.MOV_B_D);
            _ops.Add(0x43, Ops.MOV_B_E);
            _ops.Add(0x56, Ops.MOV_D_M);
            _ops.Add(0x5e, Ops.MOV_E_M);
            _ops.Add(0x66, Ops.MOV_H_M);
            _ops.Add(0x6f, Ops.MOV_L_A);
            _ops.Add(0x77, Ops.MOV_M_A);
            _ops.Add(0x7a, Ops.MOV_A_D);
            _ops.Add(0x7b, Ops.MOV_A_E);
            _ops.Add(0x7c, Ops.MOV_A_H);
            _ops.Add(0x7e, Ops.MOV_A_M);
            _ops.Add(0xa7, Ops.ANA_A);
            _ops.Add(0xaf, Ops.XRA_A);
            _ops.Add(0xc0, Ops.RNZ);
            _ops.Add(0xc1, Ops.POP_BC);
            _ops.Add(0xc2, Ops.JNZ);
            _ops.Add(0xc3, Ops.JMP);
            _ops.Add(0xc5, Ops.PUSH_CD);
            _ops.Add(0xc6, Ops.ADI);
            _ops.Add(0xc9, Ops.RET);
            _ops.Add(0xcd, Ops.CALL);
            _ops.Add(0xd1, Ops.POP_DE);
            _ops.Add(0xd3, Ops.OUT);
            _ops.Add(0xd5, Ops.PUSH_DE);
            _ops.Add(0xe1, Ops.POP_HL);
            _ops.Add(0xe5, Ops.PUSH_HL);
            _ops.Add(0xe6, Ops.ANI);
            _ops.Add(0xeb, Ops.XCHG);
            _ops.Add(0xf1, Ops.POP_PSW);
            _ops.Add(0xf5, Ops.PUSH_PSW);
            _ops.Add(0xfb, Ops.EI);
            _ops.Add(0xfe, Ops.CPI);
        }

        public void Reset() => State.Reset();

        public void Step(ProgramInstructions instructions)
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
        }
    }
}