using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            var romsBasePath = "roms";

            var folders = Directory.GetDirectories(romsBasePath);
            if (!folders.Any())
            {
                System.Console.WriteLine("No roms found! Exiting...");
                return;
            }

            var foldersMenu = string.Join("\n", folders.Select((val, i) => $"{i + 1}) {Path.GetFileName(val)}"));
            do
            {
                System.Console.WriteLine("Which rom do you want to load? (press Q to exit)");
                System.Console.WriteLine(foldersMenu);
                var input = System.Console.ReadLine().ToLower();
                if ("q" == input)
                    break;

                if (int.TryParse(input, out var index) && --index < folders.Length)
                {
                    var gameName = Path.GetFileName(folders[index]);
                    var gameRomsPath = Path.Combine(romsBasePath, gameName);

                    Run(gameRomsPath);

                    System.Console.ReadKey();
                }

                System.Console.Clear();
            } while (true);

            System.Console.ResetColor();
        }

        private static void Run(string gameRomsPath)
        {
            System.Console.WriteLine($"loading roms from {gameRomsPath}...");

            var gameName = Path.GetFileName(gameRomsPath);
            var memoryStartOffset = 0;
            if (gameName == "cpudiag")
                memoryStartOffset = 0x100;

            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }

            var memory = Memory.Load(bytes.ToArray(), memoryStartOffset);
            
            var registers = new State();
            var bus = new Bus();

            var cpu = new Cpu(registers, bus);
            cpu.Reset();

            SetupProgram(gameName, memory, cpu);
            
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("processing program, press any key to stop...");

            var tokenSource = new CancellationTokenSource();
            
            var task = Task.Run(() =>
            {
                Debug(cpu, memory, tokenSource.Token);
            }, tokenSource.Token);

            System.Console.ReadKey();

            tokenSource.Cancel();

            //var currentDump = CreateDump(cpu, memory);
            //var workingDump = File.ReadAllText(dumpFilename);
            //if (currentDump == workingDump)
            //{
            //    System.Console.ForegroundColor = ConsoleColor.Green;
            //    System.Console.WriteLine("emulator OK");
            //}
            //else
            //{
            //    System.Console.ForegroundColor = ConsoleColor.Green;
            //    System.Console.WriteLine("emulator FAILED");
            //}

            System.Console.ResetColor();
            System.Console.WriteLine("done! Press any key to continue...");
        }

        private static void SetupProgram(string gameName, Memory memory, Cpu cpu)
        {
            if (gameName != "cpudiag") 
                return;

            //Fix the first instruction to be JMP 0x100    
             cpu.State.ProgramCounter = 0x100;
            //memory[0] = 0xc3;
            //memory[1] = 0;
            //memory[2] = 0x01;

            //Fix the stack pointer from 0x6ad to 0x7ad    
            // this 0x06 byte 112 in the code, which is    
            // byte 112 + 0x100 = 368 in memory    
            memory[368] = 0x7;

            //Skip DAA test    
            memory[0x59c] = 0xc3; //JMP    
            memory[0x59d] = 0xc2;
            memory[0x59e] = 0x05;

            // Inject RET at 0x0005 to handle "CALL 5".
            memory[5] = 0xC9;
            cpu.ReplaceOpcode(0xC9, (m, c) =>
            {
                byte lo = m[cpu.State.StackPointer];
                byte hi = m[cpu.State.StackPointer + 1];
                var ret = Utils.GetValue(hi, lo);
                var op = m[ret];

                var err = $"error at {ret:X} : {op:X} . State: {c.State}";
                System.Console.WriteLine(err);
                throw new InvalidProgramException(err);
            });

            //cpu.ReplaceOpcode(0xcd, (m, c) =>
            //{
            //    if (m[c.State.ProgramCounter] == 0x76)
            //    {
            //        System.Console.WriteLine($"HLT at {c.State.ProgramCounter:X}");
            //        return;
            //    }

            //    var hi = m[c.State.ProgramCounter + 2];
            //    var lo = m[c.State.ProgramCounter + 1];
            //    var value = Utils.GetValue(hi, lo);

            //    if (0 == value)
            //    {
            //        System.Console.WriteLine($"end of the test");
            //    }
            //    else if (0x0005 == value)
            //    {
            //        if (0x09 == c.State.C)
            //        {
            //            // https://github.com/ezet/i8080-emulator/blob/master/diag/CPUDIAG.DOC

            //            var sb = new StringBuilder();
            //            var index = c.State.DE + 3;
            //            var letter = (char)m[index];
            //            while (letter != '$')
            //            {
            //                letter = (char)m[++index];
            //                sb.Append(letter);
            //            }

            //            System.Console.Write(sb);
            //        }
            //        else if (0x02 == c.State.C)
            //        {
            //            System.Console.Write((char)c.State.E);
            //        }

            //        c.State.ProgramCounter += 3;
            //    }
            //    else if (0 == c.State.ProgramCounter)
            //    {
            //        System.Console.WriteLine($"Jump to 0000 from {c.State.ProgramCounter:X}");
            //        return;
            //    }
            //    else if (0x76 == value)
            //    {
            //        System.Console.WriteLine($"HLT at {c.State.ProgramCounter:X}");
            //        return;
            //    }
            //    else
            //    {
            //        Ops.CALL(m, c);

            //        if (0 == c.State.ProgramCounter)
            //            System.Console.WriteLine($"Jump to 0000 from {c.State.ProgramCounter:X}");
            //    }
            //});
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

        private static void Debug(Cpu cpu, Memory memory, CancellationToken cancellationToken)
        {
            int i = 0;
            var sb = new StringBuilder();

            var showDebug = false;
            while (true)
            {
                if (!showDebug)
                {
                    cpu.Step(memory);
                    memory.UpdateVideoBuffer();
                }
                else
                {
                    sb.Append(i++);
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

                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            cpu.Reset();
        }
    }
}
