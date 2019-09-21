using System;

namespace emu8080
{
    public static class MathUtils
    {
        public static byte Decrement(byte a, State state)
        {
            byte result = (byte) ((a - 1) & 0xff);
            state.Flags.CalcZeroFlag(result);
            state.Flags.CalcSignFlag(result);
            state.Flags.CalcParityFlag(result);

            return result;
        }

    }
}