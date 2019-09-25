﻿using System;
using System.Collections.Generic;

namespace emu8080.Core
{
    public class Cpu
    {
        private readonly Dictionary<byte, Action<Memory, Cpu>> _ops;
        public State State {get;}
        public Bus Bus { get; }

        public Cpu(State state, Bus bus)
        {
            State = state;
            Bus = bus;

            _ops = new Dictionary<byte, Action<Memory, Cpu>>();
            _ops.Add(0x00, Ops.NOP);
            _ops.Add(0x01, Ops.LXI_B);
            _ops.Add(0x02, Ops.STAX_B);
            _ops.Add(0x03, Ops.INX_B);
            _ops.Add(0x05, Ops.DCR_B);
            _ops.Add(0x06, Ops.MVI_B);
            _ops.Add(0x07, Ops.RLC);
            _ops.Add(0x09, Ops.DAD_B);
            _ops.Add(0x0a, Ops.LDAX_A);
            _ops.Add(0x0d, Ops.DCR_C);
            _ops.Add(0x0e, Ops.MVI_C);
            _ops.Add(0x0f, Ops.RRC);
            _ops.Add(0x10, Ops.NOP);
            _ops.Add(0x11, Ops.LXI_D);
            _ops.Add(0x13, Ops.INX_D);
            _ops.Add(0x19, Ops.DAD_D);
            _ops.Add(0x1a, Ops.LDAX_D);
            _ops.Add(0x1c, Ops.INR_E);
            _ops.Add(0x1f, Ops.RAR);
            _ops.Add(0x20, Ops.NOP);
            _ops.Add(0x21, Ops.LXI_H);
            _ops.Add(0x23, Ops.INX_H);
            _ops.Add(0x26, Ops.MVI_H);
            _ops.Add(0x28, Ops.NOP);
            _ops.Add(0x29, Ops.DAD_H);
            _ops.Add(0x2e, Ops.MVI_L);
            _ops.Add(0x2f, Ops.CMA);
            _ops.Add(0x30, Ops.NOP);
            _ops.Add(0x31, Ops.LXI_SP);
            _ops.Add(0x32, Ops.STA);
            _ops.Add(0x35, Ops.DCR_M);
            _ops.Add(0x36, Ops.MVI_M);
            _ops.Add(0x37, Ops.STC);
            _ops.Add(0x3a, Ops.LDA);
            _ops.Add(0x3d, Ops.DCR_A);
            _ops.Add(0x3e, Ops.MVI_A);
            _ops.Add(0x41, Ops.MOV_B_C);
            _ops.Add(0x42, Ops.MOV_B_D);
            _ops.Add(0x43, Ops.MOV_B_E);
            _ops.Add(0x46, Ops.MOV_B_M);
            _ops.Add(0x4e, Ops.MOV_C_M);
            _ops.Add(0x4f, Ops.MOV_C_A);
            _ops.Add(0x56, Ops.MOV_D_M);
            _ops.Add(0x57, Ops.MOV_D_A);
            _ops.Add(0x5f, Ops.MOV_E_A);
            _ops.Add(0x5e, Ops.MOV_E_M);
            _ops.Add(0x61, Ops.MOV_H_C);
            _ops.Add(0x66, Ops.MOV_H_M);
            _ops.Add(0x67, Ops.MOV_H_A);
            _ops.Add(0x6f, Ops.MOV_L_A);
            _ops.Add(0x77, Ops.MOV_M_A);
            _ops.Add(0x79, Ops.MOV_A_C);
            _ops.Add(0x7a, Ops.MOV_A_D);
            _ops.Add(0x7b, Ops.MOV_A_E);
            _ops.Add(0x7c, Ops.MOV_A_H);
            _ops.Add(0x7d, Ops.MOV_A_L);
            _ops.Add(0x7e, Ops.MOV_A_M);
            _ops.Add(0x90, Ops.SUB_B);
            _ops.Add(0xa7, Ops.ANA_A);
            _ops.Add(0xaf, Ops.XRA_A);
            _ops.Add(0xb0, Ops.ORA_B);
            _ops.Add(0xb6, Ops.ORA_M);
            _ops.Add(0xc0, Ops.RNZ);
            _ops.Add(0xc1, Ops.POP_BC);
            _ops.Add(0xc2, Ops.JNZ);
            _ops.Add(0xc3, Ops.JMP);
            _ops.Add(0xc5, Ops.PUSH_CD);
            _ops.Add(0xc6, Ops.ADI);
            _ops.Add(0xc8, Ops.RZ);
            _ops.Add(0xc9, Ops.RET);
            _ops.Add(0xca, Ops.JZ);
            _ops.Add(0xcd, Ops.CALL);
            _ops.Add(0xd1, Ops.POP_DE);
            _ops.Add(0xd2, Ops.JNC);
            _ops.Add(0xd3, Ops.OUT);
            _ops.Add(0xd5, Ops.PUSH_DE);
            _ops.Add(0xd8, Ops.RC);
            _ops.Add(0xda, Ops.JC);
            _ops.Add(0xdb, Ops.IN_D8);
            _ops.Add(0xe1, Ops.POP_HL);
            _ops.Add(0xe5, Ops.PUSH_HL);
            _ops.Add(0xe3, Ops.XCHG);
            _ops.Add(0xe9, Ops.PCHL);
            _ops.Add(0xe6, Ops.ANI);
            _ops.Add(0xeb, Ops.XCHG);
            _ops.Add(0xf1, Ops.POP_PSW);
            _ops.Add(0xf3, Ops.DI);
            _ops.Add(0xf5, Ops.PUSH_PSW);
            _ops.Add(0xfb, Ops.EI);
            _ops.Add(0xfe, Ops.CPI);
            _ops.Add(0xff, Ops.RST_7);
        }

        public void Reset() => State.Reset();

        public void Step(Memory memory)
        {
            var op = memory[State.ProgramCounter];

            if (_ops.ContainsKey(op)){
                _ops[op](memory, this);
            }else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"not implemented: {op:X}");

                Console.ResetColor();
            }
        }
    }
}