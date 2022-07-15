﻿using System;

namespace emu8080.Core
{
    public static class Utils
    {
        public static byte Decrement(byte a, Registers state)
        {
            byte result = (byte)((a - 1) & 0xff);
            state.Flags.CalcZeroFlag(result);
            state.Flags.CalcSignFlag(result);
            state.Flags.CalcParityFlag(result);

            return result;
        }

        public static byte Increment(byte a, Registers state)
        {
            UInt16 result = (UInt16)(a + 1);
            state.Flags.CalcZeroFlag(result);
            state.Flags.CalcSignFlag(result);

            byte bRes = (byte)(result & 0xff);
            state.Flags.CalcParityFlag(bRes);
            state.Flags.CalcAuxCarryFlag(bRes, 0x0F);

            return bRes; 
        }

        public static byte GetLow(this int value)
        {
            return (byte)(value & 0xff);
        }

        public static byte GetHigh(this int value)
        {
            return (byte)((value >> 8) & 0xff);
        }

        public static byte GetLow(this ushort value)
        {
            return (byte)(value & 0xff);
        }

        public static byte GetHigh(this ushort value)
        {
            return (byte)((value >> 8) & 0xff);
        }

        public static ushort GetValue(byte hi, byte lo)
        {
            return (ushort) ((hi << 8) | lo);
        }

        public static bool GetParity(byte val)
        {
            return 1 == ParityTable[val];
        }

        private static byte[] ParityTable = {
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1
        };
    }
}