using System;
using System.Collections.Generic;

namespace emu8080
{
    public class Cpu
    {
        private readonly Dictionary<byte, Action<ProgramData, State>> _ops;
        private readonly State _state;

        public Cpu(State state)
        {
            _state = state;

            _ops = new Dictionary<byte, Action<ProgramData, State>>();
            _ops.Add(0x00, Ops.NOP);
            _ops.Add(0x01, Ops.LXI_B);
            _ops.Add(0x0f, Ops.RRC);
            _ops.Add(0x1f, Ops.RAR);
            _ops.Add(0x20, Ops.NOP);
            _ops.Add(0x21, Ops.LXI_H);
            _ops.Add(0x2f, Ops.CMA);
            _ops.Add(0x41, Ops.MOV_B_C);
            _ops.Add(0x42, Ops.MOV_B_D);
            _ops.Add(0x43, Ops.MOV_B_E);
            _ops.Add(0xc2, Ops.JNZ);
            _ops.Add(0xc3, Ops.JMP);
            _ops.Add(0xc9, Ops.RET);
            _ops.Add(0xcd, Ops.CALL);
            _ops.Add(0xfe, Ops.CPI);
        }

        public void Process(ProgramData programData)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("processing program...");

            do{
                var op = programData[_state.ProgramCounter];
                
                if (_ops.ContainsKey(op)){
                    _ops[op](programData, _state);
                }else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("not implemented!");
                    Console.ResetColor();
                }
            }while(++_state.ProgramCounter < programData.Length);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("done!");

            Console.ResetColor();
        }
    }
}