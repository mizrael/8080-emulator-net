using System;
using System.Collections.Generic;
using System.IO;

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

            var registers = new Registers();
            var cpu = new Cpu(registers);

            var gameRomsPath = Path.Combine(romsBasePath, gameName);
            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files)
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }
            
            var data = new Data(bytes);
            cpu.Process(data);

            //for (int index = 0;index<bytes.Length;++index)
            //{
            //    cpu.Process(bytes, ref index);

                //switch (op)
                //{
                //    case 0x00:
                //        Console.Write("NOP");
                //        break;
                //    case 0x21:
                //        addr1 = bytes[++index];
                //        addr2 = bytes[++index];
                //        Console.Write($"LXI H, {addr1:X} {addr2:X}");
                //        break;
                //    case 0x32:
                //        addr1 = bytes[++index];
                //        addr2 = bytes[++index];
                //        Console.Write($"STA {addr1:X} {addr2:X}");
                //        break;
                //    case 0x35:
                //        Console.Write("DCR M");
                //        break;
                //    case 0x3e:
                //        addr1 = bytes[++index];
                //        Console.Write($"MVI A, {addr1:X}");
                //        break;
                //    case 0xc3:
                //        addr1 = bytes[++index];
                //        addr2 = bytes[++index];
                //        Console.Write($"JMP {addr1:X} {addr2:X}");
                //        break;
                //    case 0xc5:
                //        Console.Write("PUSH B");
                //        break;
                //    case 0xcd:
                //        addr1 = bytes[++index];
                //        addr2 = bytes[++index];
                //        Console.Write($"CALL adr H, {addr1:X} {addr2:X}");
                //        break;
                //    case 0xd5:
                //        Console.Write("PUSH D");
                //        break;
                //    case 0xe5:
                //        Console.Write("PUSH H");
                //        break;

                //    case 0xf5:
                //        Console.Write("PUSH PSW");
                //        break;

                //    default:
                //        Console.ForegroundColor = ConsoleColor.Red;
                //        Console.Write("not implemented!");
                //        Console.ResetColor();
                //        break;
                //}

            //    Console.Write("\n");
            //}
        }
    }
}
