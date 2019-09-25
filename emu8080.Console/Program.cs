using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using emu8080.Core;

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
            Console.WriteLine("processing program, press ESC to quit");

            var registers = new State();
            var bus = new Bus();
            var canStop = false;
            bus.InterruptChanged += () => canStop = true;

            var cpu = new Cpu(registers, bus);
            cpu.Reset();

            while (!canStop)
            {
                cpu.Step(memory);
                memory.UpdateVideoBuffer();
            }

            // Debug(cpu, memory);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("done!");

            Console.ResetColor();
        }

        private static void Debug(Cpu cpu, Memory memory)
        {
            int i = 0;
            var sb = new StringBuilder();
            while (++i < 45000)
            {
                sb.Append(i);
                sb.Append(" ) ");
                sb.Append(cpu.State);

                cpu.Step(memory);
                memory.UpdateVideoBuffer();

                sb.Append(cpu.State.ProgramCounter.ToString("X"));
                sb.Append(" : ");
                sb.Append(memory[cpu.State.ProgramCounter].ToString("X"));
                sb.Append(memory[cpu.State.ProgramCounter + 1].ToString("X"));
                sb.Append(memory[cpu.State.ProgramCounter + 2].ToString("X"));

                Console.WriteLine(sb.ToString());
                sb.Clear();
            }
        }
    }
}
