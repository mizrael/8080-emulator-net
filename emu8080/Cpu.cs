using System;
using System.Collections.Generic;

namespace emu8080
{
    public class Cpu
    {
        private readonly Dictionary<byte, Action<Data, Registers>> ops = new Dictionary<byte, Action<Data, Registers>>();
        private readonly Registers _registers;

        public Cpu(Registers registers)
        {
            _registers = registers;

            ops.Add(0x00, Ops.NOP);
            ops.Add(0x21, Ops.LXI_H);
            ops.Add(0xc3, Ops.JMP);
        }

        public void Process(Data data)
        {
            byte op = bytes[index];

            Console.Write($"processing byte '{op:X}': ");
            if (ops.ContainsKey(op))
                ops[op](_data);
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("not implemented!");
                Console.ResetColor();
            }
        }
    }
}