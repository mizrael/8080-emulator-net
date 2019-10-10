using System;
using System.Collections.Generic;

namespace emu8080.Core
{
    public class Cpu
    {
        private static Dictionary<byte, Action<Memory, Cpu>> _ops;

        static Cpu()
        {
            SetupOpcodes();
        }

        static void SetupOpcodes()
        {
            _ops = new Dictionary<byte, Action<Memory, Cpu>>();
            _ops.Add(0x00, Ops.NOP);
            _ops.Add(0x01, Ops.LXI_B);
            _ops.Add(0x02, Ops.STAX_B);
            _ops.Add(0x03, Ops.INX_B);
            _ops.Add(0x04, Ops.INR_B);
            _ops.Add(0x05, Ops.DCR_B);
            _ops.Add(0x06, Ops.MVI_B);
            _ops.Add(0x07, Ops.RLC);
            _ops.Add(0x08, Ops.NOP);
            _ops.Add(0x09, Ops.DAD_B);
            _ops.Add(0x0a, Ops.LDAX_A);
            _ops.Add(0x0b, Ops.DCX_B);
            _ops.Add(0x0c, Ops.INR_C);
            _ops.Add(0x0d, Ops.DCR_C);
            _ops.Add(0x0e, Ops.MVI_C);
            _ops.Add(0x0f, Ops.RRC);
            _ops.Add(0x10, Ops.NOP);
            _ops.Add(0x11, Ops.LXI_D);
            _ops.Add(0x13, Ops.INX_D);
            _ops.Add(0x14, Ops.INR_D);
            _ops.Add(0x15, Ops.DCR_D);
            _ops.Add(0x16, Ops.MVI_D);
            _ops.Add(0x19, Ops.DAD_D);
            _ops.Add(0x1a, Ops.LDAX_D);
            _ops.Add(0x1c, Ops.INR_E);
            _ops.Add(0x1f, Ops.RAR);
            _ops.Add(0x20, Ops.NOP);
            _ops.Add(0x21, Ops.LXI_H);
            _ops.Add(0x22, Ops.SHLD);
            _ops.Add(0x23, Ops.INX_H);
            _ops.Add(0x26, Ops.MVI_H);
            _ops.Add(0x28, Ops.NOP);
            _ops.Add(0x29, Ops.DAD_H);
            _ops.Add(0x2a, Ops.LHLD);
            _ops.Add(0x2b, Ops.DCX_H);
            _ops.Add(0x2e, Ops.MVI_L);
            _ops.Add(0x2f, Ops.CMA);
            _ops.Add(0x30, Ops.NOP);
            _ops.Add(0x31, Ops.LXI_SP);
            _ops.Add(0x32, Ops.STA);
            _ops.Add(0x33, Ops.INX_SP);
            _ops.Add(0x34, Ops.INR_M);
            _ops.Add(0x35, Ops.DCR_M);
            _ops.Add(0x36, Ops.MVI_M);
            _ops.Add(0x37, Ops.STC);
            _ops.Add(0x38, Ops.NOP);
            _ops.Add(0x3a, Ops.LDA);
            _ops.Add(0x3b, Ops.DCX_SP);
            _ops.Add(0x3c, Ops.INR_A);
            _ops.Add(0x3d, Ops.DCR_A);
            _ops.Add(0x3e, Ops.MVI_A);
            _ops.Add(0x40, Ops.MOV_B_B);
            _ops.Add(0x41, Ops.MOV_B_C);
            _ops.Add(0x42, Ops.MOV_B_D);
            _ops.Add(0x43, Ops.MOV_B_E);
            _ops.Add(0x44, Ops.MOV_B_H);
            _ops.Add(0x45, Ops.MOV_B_L);
            _ops.Add(0x46, Ops.MOV_B_M);
            _ops.Add(0x47, Ops.MOV_B_A);
            _ops.Add(0x48, Ops.MOV_C_B);
            _ops.Add(0x49, Ops.MOV_C_C);
            _ops.Add(0x4a, Ops.MOV_C_D);
            _ops.Add(0x4b, Ops.MOV_C_E);
            _ops.Add(0x4c, Ops.MOV_C_H);
            _ops.Add(0x4d, Ops.MOV_C_L);
            _ops.Add(0x4e, Ops.MOV_C_M);
            _ops.Add(0x4f, Ops.MOV_C_A);
            _ops.Add(0x50, Ops.MOV_D_B);
            _ops.Add(0x51, Ops.MOV_D_C);
            _ops.Add(0x52, Ops.MOV_D_D);
            _ops.Add(0x53, Ops.MOV_D_E);
            _ops.Add(0x54, Ops.MOV_D_H);
            _ops.Add(0x55, Ops.MOV_D_L);
            _ops.Add(0x56, Ops.MOV_D_M);
            _ops.Add(0x57, Ops.MOV_D_A);
            _ops.Add(0x58, Ops.MOV_E_B);
            _ops.Add(0x59, Ops.MOV_E_C);
            _ops.Add(0x5a, Ops.MOV_E_D);
            _ops.Add(0x5b, Ops.MOV_E_E);
            _ops.Add(0x5c, Ops.MOV_E_H);
            _ops.Add(0x5d, Ops.MOV_E_L);
            _ops.Add(0x5e, Ops.MOV_E_M);
            _ops.Add(0x5f, Ops.MOV_E_A);
            _ops.Add(0x61, Ops.MOV_H_C);
            _ops.Add(0x66, Ops.MOV_H_M);
            _ops.Add(0x67, Ops.MOV_H_A);
            _ops.Add(0x68, Ops.MOV_L_B);
            _ops.Add(0x69, Ops.MOV_L_C);
            _ops.Add(0x6f, Ops.MOV_L_A);
            _ops.Add(0x70, Ops.MOV_M_B);
            _ops.Add(0x71, Ops.MOV_M_C);
            _ops.Add(0x72, Ops.MOV_M_D);
            _ops.Add(0x73, Ops.MOV_M_E);
            _ops.Add(0x74, Ops.MOV_M_H);
            _ops.Add(0x75, Ops.MOV_M_L);
            _ops.Add(0x76, Ops.HLT); 
            _ops.Add(0x77, Ops.MOV_M_A);
            _ops.Add(0x78, Ops.MOV_A_B);
            _ops.Add(0x79, Ops.MOV_A_C);
            _ops.Add(0x7a, Ops.MOV_A_D);
            _ops.Add(0x7b, Ops.MOV_A_E);
            _ops.Add(0x7c, Ops.MOV_A_H);
            _ops.Add(0x7d, Ops.MOV_A_L);
            _ops.Add(0x7e, Ops.MOV_A_M);
            _ops.Add(0x80, Ops.ADD_B);
            _ops.Add(0x81, Ops.ADD_C);
            _ops.Add(0x82, Ops.ADD_D);
            _ops.Add(0x83, Ops.ADD_E);
            _ops.Add(0x84, Ops.ADD_H);
            _ops.Add(0x85, Ops.ADD_L);
            _ops.Add(0x86, Ops.ADD_M);
            _ops.Add(0x87, Ops.ADD_A);
            _ops.Add(0x88, Ops.ADC_B);
            _ops.Add(0x90, Ops.SUB_B);
            _ops.Add(0x9e, Ops.SBB_M);
            _ops.Add(0xa0, Ops.ANA_B);
            _ops.Add(0xa6, Ops.ANA_M);
            _ops.Add(0xa7, Ops.ANA_A);
            _ops.Add(0xa8, Ops.XRA_B);
            _ops.Add(0xa9, Ops.XRA_C);
            _ops.Add(0xaa, Ops.XRA_D);
            _ops.Add(0xab, Ops.XRA_E);
            _ops.Add(0xac, Ops.XRA_H);
            _ops.Add(0xad, Ops.XRA_L);
            _ops.Add(0xae, Ops.XRA_M);
            _ops.Add(0xaf, Ops.XRA_A);
            _ops.Add(0xb0, Ops.ORA_B);
            _ops.Add(0xb1, Ops.ORA_C);
            _ops.Add(0xb2, Ops.ORA_D);
            _ops.Add(0xb3, Ops.ORA_E);
            _ops.Add(0xb4, Ops.ORA_H);
            _ops.Add(0xb5, Ops.ORA_L);
            _ops.Add(0xb6, Ops.ORA_M);
            _ops.Add(0xb7, Ops.ORA_A);
            _ops.Add(0xb8, Ops.CMP_B);
            _ops.Add(0xb9, Ops.CMP_C);
            _ops.Add(0xba, Ops.CMP_D);
            _ops.Add(0xbb, Ops.CMP_E);
            _ops.Add(0xbc, Ops.CMP_H);
            _ops.Add(0xbd, Ops.CMP_L);
            _ops.Add(0xbe, Ops.CMP_M);
            _ops.Add(0xbf, Ops.CMP_A);
            _ops.Add(0xc0, Ops.RNZ);
            _ops.Add(0xc1, Ops.POP_BC);
            _ops.Add(0xc2, Ops.JNZ);
            _ops.Add(0xc3, Ops.JMP);
            _ops.Add(0xc4, Ops.CNZ);
            _ops.Add(0xc5, Ops.PUSH_CD);
            _ops.Add(0xc6, Ops.ADI);
            _ops.Add(0xc8, Ops.RZ);
            _ops.Add(0xc9, Ops.RET);
            _ops.Add(0xca, Ops.JZ);
            _ops.Add(0xcc, Ops.CZ);
            _ops.Add(0xcd, Ops.CALL);
            _ops.Add(0xce, Ops.ACI); 
            _ops.Add(0xd0, Ops.RNC);
            _ops.Add(0xd1, Ops.POP_DE);
            _ops.Add(0xd2, Ops.JNC);
            _ops.Add(0xd3, Ops.OUT);
            _ops.Add(0xd4, Ops.CNC);
            _ops.Add(0xd5, Ops.PUSH_DE);
            _ops.Add(0xd6, Ops.SUI);
            _ops.Add(0xd8, Ops.RC);
            _ops.Add(0xda, Ops.JC);
            _ops.Add(0xdc, Ops.CC);
            _ops.Add(0xdb, Ops.IN_D8);
            _ops.Add(0xde, Ops.SBI);
            _ops.Add(0xe0, Ops.RPO);
            _ops.Add(0xe1, Ops.POP_HL);
            _ops.Add(0xe2, Ops.JPO);
            _ops.Add(0xe3, Ops.XTHL);
            _ops.Add(0xe4, Ops.CPO);
            _ops.Add(0xe5, Ops.PUSH_HL);
            _ops.Add(0xe9, Ops.PCHL);
            _ops.Add(0xe6, Ops.ANI);
            _ops.Add(0xe8, Ops.RPE);
            _ops.Add(0xea, Ops.JPE);
            _ops.Add(0xeb, Ops.XCHG);
            _ops.Add(0xec, Ops.CPE);
            _ops.Add(0xee, Ops.XRI);
            _ops.Add(0xf0, Ops.RP);
            _ops.Add(0xf1, Ops.POP_PSW);
            _ops.Add(0xf2, Ops.JP);
            _ops.Add(0xf3, Ops.DI);
            _ops.Add(0xf4, Ops.CP);
            _ops.Add(0xf5, Ops.PUSH_PSW);
            _ops.Add(0xf6, Ops.ORI);
            _ops.Add(0xf8, Ops.RM);
            _ops.Add(0xfa, Ops.JM);
            _ops.Add(0xfb, Ops.EI);
            _ops.Add(0xfc, Ops.CM);
            _ops.Add(0xfe, Ops.CPI);
            _ops.Add(0xff, Ops.RST_7);
        }

        public State State {get;}
        public Bus Bus { get; }

        public Cpu(State state, Bus bus)
        {
            State = state;
            Bus = bus;
        }

        public void Reset()
        {
            SetupOpcodes(); 
            State.Reset();
        }

        public void ReplaceOpcode(byte op, Action<Memory, Cpu> func)
        {
            if (!_ops.ContainsKey(op))
                _ops.Add(op, func);
            else
                _ops[op] = func;
        }

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
                this.State.ProgramCounter++;
            }
        }
    }
}