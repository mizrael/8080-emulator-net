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

        public byte Register
        {
            get;
            set;
        }

        public ConditionalFlags()
        {
            // in the real CPU bit 1 is unused and always 1
            Register = 0x00;
        }

        public bool CarryFlag
        {
            get
            {
                return GetBit(CARRY_FLAG);
            }
            set
            {
                SetBit(CARRY_FLAG, value);
            }
        }
        public bool SignFlag
        {
            get
            {
                return GetBit(SIGN_FLAG);
            }
            set
            {
                SetBit(SIGN_FLAG, value);
            }
        }
        public bool ZeroFlag
        {
            get
            {
                return GetBit(ZERO_FLAG);
            }
            set
            {
                SetBit(ZERO_FLAG, value);
            }
        }
        public bool AuxCarryFlag
        {
            get
            {
                return GetBit(AUX_CARRY_FLAG);
            }
            set
            {
                SetBit(AUX_CARRY_FLAG, value);
            }
        }
        public bool ParityFlag
        {
            get
            {
                return GetBit(PARITY_FLAG);
            }
            set
            {
                SetBit(PARITY_FLAG, value);
            }
        }

        public void CalcCarryFlag(UInt16 result)
        {
            CarryFlag = (result > 0xff);
        }

        public void CalcZeroFlag(UInt16 result)
        {
            ZeroFlag = ((result & 0xff) == 0);
        }

        public void CalcSignFlag(UInt16 result)
        {
            SignFlag = ((result & 0x80) == 0x80);
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
            ParityFlag = (total % 2) == 0;
        }

        public void CalcAuxCarryFlag(byte a, byte b)
        {
            AuxCarryFlag = (byte)((a & 0x0f) + (b & 0x0f)) > 0x0f;
        }

        public void CalcAuxCarryFlag(byte a, byte b, byte c)
        {
            AuxCarryFlag = (byte)((a & 0x0f) + (b & 0x0f) + (c & 0x0f)) > 0x0f;
        }

        private bool GetBit(byte bit)
        {
            return ((Register & bit) == bit);
        }

        private void SetBit(byte bit, bool set)
        {
            if (set)
                Register = (byte)(Register | bit);
            else
                Register = (byte)(Register & ~bit);
        }

    }
}