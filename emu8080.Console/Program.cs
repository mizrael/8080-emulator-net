using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using emu8080.Core;

namespace emu8080.Console
{
    class Program
    {
        private const string dumpFilename = "dump.txt";

        /// <summary>
        /// http://www.emulator101.com/
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var gameName = "cpudiag";

            var romsBasePath = "roms";

            var gameRomsPath = Path.Combine(romsBasePath, gameName);

            System.Console.WriteLine($"loading roms from {gameRomsPath}...");

            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }

            ushort startPos = 0x100;
            var memory = Memory.Load(bytes.ToArray(), startPos);

            memory[368] = 0x7;

            // Skip DAA test (not implemented).
            memory[0x59c] = 0xc3; //JMP
            memory[0x59d] = 0xc2;
            memory[0x59e] = 0x05;

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("processing program, press ESC to quit");

            var registers = new State();
            var bus = new Bus();
            
            var cpu = new Cpu(registers, bus);
            cpu.Reset();
            cpu.State.ProgramCounter = startPos;
           
            Run(cpu, memory);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("done!");

            System.Console.ResetColor();
        }

        private static void SaveDump(string dump)
        {
            File.WriteAllText(dumpFilename, dump);
        }

        private static void Run(Cpu cpu, Memory memory)
        {
            while (true)
            {
                if (cpu.State.ProgramCounter == 0x0000)
                    break;
                else if (cpu.State.ProgramCounter == 0x689) //CPUER
                {
                    byte lo = memory[cpu.State.StackPointer];
                    byte hi = memory[cpu.State.StackPointer + 1];
                    var errLoc = (ushort)(Utils.GetValue(hi, lo) + 1);
                    var errOp = memory[errLoc];
                    System.Console.WriteLine($"cpu failed at op {errOp:X}");

                    break;
                }
                else if (cpu.State.ProgramCounter == 0x0005)
                {
                    if (cpu.State.C == 0x02)
                        System.Console.Write(Encoding.ASCII.GetString(new[] {cpu.State.E}));
                    else if (cpu.State.C == 0x09)
                    {
                        var ptr = cpu.State.DE;
                        var sb = new StringBuilder();
                        while (true)
                        {
                            var c = (char)memory[ptr];
                            if (c == '$') break;
                            sb.Append(c);
                            ptr++;
                        }
                        System.Console.Write(sb);
                    }

                    cpu.State.ProgramCounter++;
                }
                else
                    cpu.Step(memory);
            }
        }
    }
}
