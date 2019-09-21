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

        // https://github.com/amensch/e8080/blob/master/e8080/Intel8080/i8080.cs
        public static void Compare(byte a, byte b, State state)
        {
            // This operation only alters the conditional register.
            // Comparision is calculated by subtraction.  The
            // zero flag indicates equality

            UInt16 answer = Subtract(a, b, state);
        }

        public static byte Subtract(byte a, byte b, State state)
        {
            // Subtract using 2's compliment
            UInt16 answer = (UInt16)(a + (~b & 0xff) + 1);

            // On subtraction no carry out sets the carry bit
            // this is opposite from the normal calculation           
            state.Flags.CalcCarryFlag(answer);
            state.Flags.Carry = !state.Flags.Carry;

            state.Flags.CalcZeroFlag(answer);
            state.Flags.CalcParityFlag(answer);
            state.Flags.CalcSignFlag(answer);
            state.Flags.CalcAuxCarryFlag(a, (byte)(~b & 0xff), 1);
            return (byte)(answer & 0xff);
        }

    }
}