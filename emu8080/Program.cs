using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace emu8080
{
    class Program
    {

        /// <summary>
        /// http://www.emulator101.com/
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var gameName = "invaders";
            
            var romsBasePath = "roms";

            var registers = new State();
            var cpu = new Cpu(registers);

            var gameRomsPath = Path.Combine(romsBasePath, gameName);
            
            Console.WriteLine($"loading roms from {gameRomsPath}...");

            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }
            
            var memory = Memory.Load(bytes.ToArray());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("processing program...");
            
            cpu.Reset();

            int i=0;
            var sb = new StringBuilder();
            while (++i<45000)
            {
                sb.Append(i);
                sb.Append(" ) ");
                sb.Append(cpu.State);

                cpu.Step(memory);

                sb.Append(cpu.State.ProgramCounter.ToString("X"));
                sb.Append(" : ");
                sb.Append(memory[cpu.State.ProgramCounter].ToString("X"));
                sb.Append(memory[cpu.State.ProgramCounter + 1].ToString("X"));
                sb.Append(memory[cpu.State.ProgramCounter + 2].ToString("X"));

                Console.WriteLine(sb.ToString());
                sb.Clear();

                //cpu.Step(instructions);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("done!");

            Console.ResetColor();
        }
    }
}
