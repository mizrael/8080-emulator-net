using System;

namespace emu8080
{
    /// <summary>
    /// https://github.com/amensch/e8080/blob/master/e8080/Intel8080/i8080ConditionalRegister.cs
    /// </summary>
    public class ConditionalFlags{
        private const byte SIGN_FLAG = 0x80;        // 0=positive, 1=negative
        private const byte ZERO_FLAG = 0x40;        // 0=non-zero, 1=zero
        private const byte AUX_CARRY_FLAG = 0x08;   // not implemented for now
        private const byte PARITY_FLAG = 0x04;      // 0=odd, 1=even
        private const byte CARRY_FLAG = 0x01;       // 0=no carry, 1=carry

        private byte _register = 0x00;
        
        public bool Carry
        {
            get => GetBit(CARRY_FLAG);
            private set => SetBit(CARRY_FLAG, value);
        }
        public bool Sign
        {
            get => GetBit(SIGN_FLAG);
            private set => SetBit(SIGN_FLAG, value);
        }
        public bool Zero
        {
            get => GetBit(ZERO_FLAG);
            private set => SetBit(ZERO_FLAG, value);
        }
        public bool AuxCarry
        {
            get => GetBit(AUX_CARRY_FLAG);
            private set => SetBit(AUX_CARRY_FLAG, value);
        }
        public bool Parity
        {
            get => GetBit(PARITY_FLAG);
            private set => SetBit(PARITY_FLAG, value);
        }

        public void CalcCarryFlag(UInt16 result)
        {
            Carry = (result > 0xff);
        }

        public void CalcZeroFlag(UInt16 result)
        {
            Zero = ((result & 0xff) == 0);
        }

        public void CalcSignFlag(UInt16 result)
        {
            Sign = ((result & SIGN_FLAG) == SIGN_FLAG);
        }

        public void CalcParityFlag(UInt16 result)
        {
            // parity = 0 is odd
            // parity = 1 is even
            CalcParityFlag((byte)(result & 0xff));
        }

        public void CalcParityFlag(byte result)
        {
            // parity = 0 is odd
            // parity = 1 is even
            byte num = (byte)(result & 0xff);
            byte total = 0;
            for (total = 0; num > 0; total++)
            {
                num &= (byte)(num - 1);
            }
            Parity = (total % 2) == 0;
        }

        public void CalcAuxCarryFlag(byte a, byte b)
        {
            AuxCarry = (byte)((a & 0x0f) + (b & 0x0f)) > 0x0f;
        }

        public void CalcAuxCarryFlag(byte a, byte b, byte c)
        {
            AuxCarry = (byte)((a & 0x0f) + (b & 0x0f) + (c & 0x0f)) > 0x0f;
        }

        private bool GetBit(byte bit)
        {
            return ((_register & bit) == bit);
        }

        private void SetBit(byte bit, bool set)
        {
            if (set)
                _register = (byte)(_register | bit);
            else
                _register = (byte)(_register & ~bit);
        }

    }
}