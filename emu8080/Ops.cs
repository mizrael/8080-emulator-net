using System;

namespace emu8080
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        // 0x00
        public static void NOP(Data data, Registers registers)
        {
            Console.Write("NOP");
        }

        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(Data data, Registers registers)
        {
            data.Increment();
            registers.C = data.GetCurrent();
            data.Increment();
            registers.B = data.GetCurrent();
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(Data data, Registers registers)
        {
            data.Increment();
            registers.L = data.GetCurrent();
            data.Increment();
            registers.H = data.GetCurrent();
        }

        // 0x41 , B <- C
        public static void MOV_B_C(Data data, Registers registers)
        {
            registers.B = registers.C;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(Data data, Registers registers)
        {
            registers.B = registers.D;
        }

        // 0x42 , B <- E
        public static void MOV_B_E(Data data, Registers registers)
        {
            registers.B = registers.E;
        }

        // 0xc3
        public static void JMP(Data data, Registers registers)
        {
            data.Increment();
            var addr1 = data.GetCurrent();
            data.Increment();
            var addr2 = data.GetCurrent();
            var addr = addr1 + (addr2 << 4);
            data.SetAddress(addr);
        }

    }
}