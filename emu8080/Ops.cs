using System;

namespace emu8080
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        public static void NOP(Data data, Registers registers)
        {
            Console.Write("NOP");
        }

        public static void JMP(Data data, Registers registers)
        {
            var addr1 = bytes[++index];
            var addr2 = bytes[++index];
            Console.Write($"JMP, {addr1:X} {addr2:X}");
            return index;
        }

        public static void LXI_H(Data data, Registers registers)
        {
            var addr1 = bytes[++index];
            var addr2 = bytes[++index];
            Console.Write($"LXI H, {addr1:X} {addr2:X}");
            return index;
        }
    }
}