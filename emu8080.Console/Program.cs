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
            var gameName = "invaders";

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

            var memory = Memory.Load(bytes.ToArray());

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("processing program, press ESC to quit");

            var registers = new State();
            var bus = new Bus();
            
            var cpu = new Cpu(registers, bus);
            cpu.Reset();
            
            var currentDump = CreateDump(cpu, memory);
            var workingDump = File.ReadAllText(dumpFilename);
            if (currentDump == workingDump)
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("emulator OK");
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("emulator FAILED");
            }

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("done!");

            System.Console.ResetColor();
        }

        private static void SaveDump(string dump)
        {
            File.WriteAllText(dumpFilename, dump);
        }

        private static string CreateDump(Cpu cpu, Memory memory)
        {
            int i = 0;
            var sb = new StringBuilder();
            while (++i < 41000)
            {
                cpu.Step(memory);
                sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(cpu.State));
            }

            return sb.ToString();
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

                System.Console.WriteLine(sb.ToString());
                sb.Clear();
            }
        }
    }
}
