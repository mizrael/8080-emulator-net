using System;

namespace emu8080
{
    public static class MathUtils
    {
        public static byte Decrement(byte a, State state)
        {
            byte result = (byte) ((a - 1) & 0xff);
            state.ConditionalFlags.CalcZeroFlag(result);
            state.ConditionalFlags.CalcSignFlag(result);
            state.ConditionalFlags.CalcParityFlag(result);

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
            state.ConditionalFlags.CalcCarryFlag(answer);
            state.ConditionalFlags.Carry = !state.ConditionalFlags.Carry;

            state.ConditionalFlags.CalcZeroFlag(answer);
            state.ConditionalFlags.CalcParityFlag(answer);
            state.ConditionalFlags.CalcSignFlag(answer);
            state.ConditionalFlags.CalcAuxCarryFlag(a, (byte)(~b & 0xff), 1);
            return (byte)(answer & 0xff);
        }

    }
}