using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace emu8080.Core
{
    public class Cpu
    {
        private static readonly Dictionary<byte, Func<Memory, Cpu, int>> _ops;

        private readonly ILogger<Cpu> _logger;
        private readonly ConcurrentQueue<byte> _interrupts;

        static Cpu()
        {
            _ops = new Dictionary<byte, Func<Memory, Cpu, int>>()
            {
                { 0x00, Ops.NOP },
                { 0x01, Ops.LXI_B },
                { 0x02, Ops.STAX_B },
                { 0x03, Ops.INX_B },
                { 0x04, Ops.INR_B },
                { 0x05, Ops.DCR_B },
                { 0x06, Ops.MVI_B },
                { 0x07, Ops.RLC },
                { 0x08, Ops.NOP },
                { 0x09, Ops.DAD_B },
                { 0x0a, Ops.LDAX_B },
                { 0x0b, Ops.DCX_B },
                { 0x0d, Ops.DCR_C },
                { 0x0e, Ops.MVI_C },
                { 0x0f, Ops.RRC },
                { 0x10, Ops.NOP },
                { 0x11, Ops.LXI_D },
                { 0x12, Ops.STAX_D },
                { 0x13, Ops.INX_D },
                { 0x14, Ops.INR_D },
                { 0x15, Ops.DCR_D},
                { 0x18, Ops.NOP },
                { 0x19, Ops.DAD_D},
                { 0x1a, Ops.LDAX_D},
                { 0x1c, Ops.INR_E },
                { 0x1e, Ops.MVI_E },
                { 0x1f, Ops.RAR },
                { 0x20, Ops.NOP },
                { 0x21, Ops.LXI_H },
                { 0x22, Ops.SHLD },
                { 0x23, Ops.INX_H },
                { 0x24, Ops.INR_H },
                { 0x25, Ops.DCR_H },
                { 0x26, Ops.MVI_H },
                { 0x28, Ops.NOP },
                { 0x29, Ops.DAD_H },
                { 0x2a, Ops.LHLD },
                { 0x2e, Ops.MVI_L },
                { 0x2f, Ops.CMA },
                { 0x30, Ops.NOP },
                { 0x31, Ops.LXI_SP },
                { 0x32, Ops.STA },
                { 0x35, Ops.DCR_M },
                { 0x36, Ops.MVI_M },
                { 0x37, Ops.STC },
                { 0x38, Ops.NOP },
                { 0x39, Ops.DAD_SP },
                { 0x3a, Ops.LDA },
                { 0x3c, Ops.INR_A },
                { 0x3d, Ops.DCR_A },
                { 0x3e, Ops.MVI_A },
                { 0x3f, Ops.CMC },
                { 0x40, Ops.MOV_B_B },
                { 0x41, Ops.MOV_B_C },
                { 0x42, Ops.MOV_B_D },
                { 0x43, Ops.MOV_B_E },
                { 0x44, Ops.MOV_B_H },
                { 0x45, Ops.MOV_B_L },
                { 0x46, Ops.MOV_B_M },
                { 0x49, Ops.MOV_C_C },
                { 0x4a, Ops.MOV_C_D },
                { 0x4e, Ops.MOV_C_M },
                { 0x4f, Ops.MOV_C_A },
                { 0x54, Ops.MOV_D_H },
                { 0x56, Ops.MOV_D_M },
                { 0x57, Ops.MOV_D_A },
                { 0x5f, Ops.MOV_E_A },
                { 0x5e, Ops.MOV_E_M },
                { 0x61, Ops.MOV_H_C },
                { 0x66, Ops.MOV_H_M },
                { 0x67, Ops.MOV_H_A },
                { 0x69, Ops.MOV_L_C },
                { 0x6d, Ops.MOV_L_L },
                { 0x6f, Ops.MOV_L_A },
                { 0x77, Ops.MOV_M_A },
                { 0x78, Ops.MOV_A_B },
                { 0x79, Ops.MOV_A_C },
                { 0x7a, Ops.MOV_A_D },
                { 0x7b, Ops.MOV_A_E },
                { 0x7c, Ops.MOV_A_H },
                { 0x7d, Ops.MOV_A_L },
                { 0x7e, Ops.MOV_A_M },
                { 0x7f, Ops.MOV_A_A },
                { 0x80, Ops.ADD_B },
                { 0x81, Ops.ADD_C },
                { 0x87, Ops.ADD_A },
                { 0x88, Ops.ADC_B },
                { 0x89, Ops.ADC_C },
                { 0x8b, Ops.ADC_E },
                { 0x8c, Ops.ADC_H },
                { 0x8d, Ops.ADC_L },
                { 0x8e, Ops.ADC_M },
                { 0x90, Ops.SUB_B },
                { 0x99, Ops.SBB_C },
                { 0x9a, Ops.SBB_D },
                { 0x9b, Ops.SBB_E },
                { 0x9c, Ops.SBB_H },
                { 0x9d, Ops.SBB_L },
                { 0x9e, Ops.SBB_M },
                { 0x9f, Ops.SBB_A },
                { 0xa0, Ops.ANA_B },
                { 0xa7, Ops.ANA_A },
                { 0xaa, Ops.XRA_D },
                { 0xaf, Ops.XRA_A },
                { 0xb0, Ops.ORA_B },
                { 0xb6, Ops.ORA_M },
                { 0xbe, Ops.CMP_M },
                { 0xc0, Ops.RNZ },
                { 0xc1, Ops.POP_BC },
                { 0xc2, Ops.JNZ },
                { 0xc3, Ops.JMP },
                { 0xc4, Ops.CNZ },
                { 0xc5, Ops.PUSH_B },
                { 0xc6, Ops.ADI },
                { 0xc8, Ops.RZ },
                { 0xc9, Ops.RET },
                { 0xca, Ops.JZ },
                { 0xcc, Ops.CZ },
                { 0xcd, Ops.CALL },
                { 0xce, Ops.ACI },
                { 0xd0, Ops.RNC },
                { 0xd1, Ops.POP_DE },
                { 0xd2, Ops.JNC },
                { 0xd3, Ops.OUT },
                { 0xd5, Ops.PUSH_DE },
                { 0xd6, Ops.SUI },
                { 0xd8, Ops.RC },
                { 0xda, Ops.JC },
                { 0xdb, Ops.IN_D8 },
                { 0xe1, Ops.POP_HL },
                { 0xe2, Ops.JPO },
                { 0xe3, Ops.XTHL },
                { 0xe5, Ops.PUSH_HL },
                { 0xe6, Ops.ANI },
                { 0xe9, Ops.PCHL },
                { 0xea, Ops.JPE },
                { 0xeb, Ops.XCHG },
                { 0xec, Ops.CPE },
                { 0xee, Ops.XRI },
                { 0xf1, Ops.POP_PSW },
                { 0xf2, Ops.JP },
                { 0xf3, Ops.DI },
                { 0xf4, Ops.CP },
                { 0xf5, Ops.PUSH_PSW },
                { 0xf6, Ops.ORI_D },
                { 0xf8, Ops.RM },
                { 0xfa, Ops.JM },
                { 0xfb, Ops.EI },
                { 0xfc, Ops.CM },
                { 0xfe, Ops.CPI },
                { 0xff, Ops.RST_7 }
            };
        }

        public Registers Registers { get; }
        public Bus Bus { get; }

        public Cpu(Registers registers, Bus bus, ILogger<Cpu> logger)
        {
            Registers = registers;
            Bus = bus;

            _logger = logger;
            _interrupts = new ConcurrentQueue<byte>();
        }

        public void Reset() => Registers.Reset();

        public void AddInterrupt(byte op_code)
        {
            _interrupts.Enqueue(op_code);
        }

        public int Step(Memory memory)
        => (_interrupts.Count != 0 && _interrupts.TryDequeue(out var loc)) ?
                ProcessOp(memory, loc) : ProcessOp(memory, Registers.ProgramCounter);

        private int ProcessOp(Memory memory, ushort loc)
        {
            var op = memory[loc];

            // _logger.LogDebug($"processing op {op:X} at {loc:X}");

            if (_ops.ContainsKey(op))
            {
                return _ops[op](memory, this);
            }
            else
            {
                _logger.LogError($"not implemented: {op:X} \n");
            }

            return 4;
        }
    }
}