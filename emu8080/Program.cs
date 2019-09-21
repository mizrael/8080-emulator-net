using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            
            var instructions = ProgramInstructions.Load(bytes.ToArray());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("processing program...");

            cpu.Reset();
            int i=0;
            while(++i<10000){
                var op = cpu.Step(instructions);
                Console.WriteLine($"{i}) op: {op:X} {cpu.State}");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("done!");

            Console.ResetColor();
        }
    }
}
