using System;
using System.Collections.Generic;

namespace emu8080
{
    public class Cpu
    {
        private readonly Dictionary<byte, Action<Data, Registers>> _ops;
        private readonly Registers _registers;

        public Cpu(Registers registers)
        {
            _registers = registers;

            _ops = new Dictionary<byte, Action<Data, Registers>>();
            _ops.Add(0x00, Ops.NOP);
            _ops.Add(0x01, Ops.LXI_B);
            _ops.Add(0x21, Ops.LXI_H);
            _ops.Add(0x41, Ops.MOV_B_C);
            _ops.Add(0x42, Ops.MOV_B_D);
            _ops.Add(0x43, Ops.MOV_B_E);
            _ops.Add(0xc3, Ops.JMP);
        }

        public void Process(Data data)
        {
            do{
                var op = data.GetCurrent();

                Console.Write($"processing byte '{op:X}': ");

                if (_ops.ContainsKey(op)){
                    _ops[op](data, _registers);
                }else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("not implemented!");
                    Console.ResetColor();
                }

                Console.Write("\n");
            }while(data.Increment());
        }
    }
}