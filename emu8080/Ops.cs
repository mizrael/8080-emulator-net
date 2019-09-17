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
            data.Increment();
            var addr1 = data.GetCurrent();
            data.Increment();
            var addr2 = data.GetCurrent();
            Console.Write($"JMP, {addr1:X} {addr2:X}");
        }

        public static void LXI_H(Data data, Registers registers)
        {
            data.Increment();
            var addr1 = data.GetCurrent();
            data.Increment();
            var addr2 = data.GetCurrent();
            Console.Write($"LXI H, {addr1:X} {addr2:X}");
        }
    }
}