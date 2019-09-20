using System;

namespace emu8080
{
    /// <summary>
    /// https://github.com/amensch/e8080/blob/master/e8080/Intel8080/i8080ConditionalRegister.cs
    /// </summary>
    public class ConditionalFlags{
        public const byte SignFlag = 0x80;        // 0=positive, 1=negative
        public const byte ZeroFlag = 0x40;        // 0=non-zero, 1=zero
        public const byte AuxCarryFlag = 0x08;   // not implemented for now
        public const byte ParityFlag = 0x04;      // 0=odd, 1=even
        public const byte CarryFlag = 0x01;       // 0=no carry, 1=carry

        //https://github.com/kade-robertson/Emu8080/blob/master/Emu8080/Objects/CPUFlag.cs
        public byte Flags
        {
            get
            {
                byte outb = 0x02; // this bit is always set, bits 3 and 5 are never set
                if (Carry)
                {
                    outb |= (byte)CarryFlag;
                }
                else
                {
                    outb &= (~(byte)(CarryFlag) & 0xFF);
                }
                if (Parity)
                {
                    outb |= (byte)ParityFlag;
                }
                else
                {
                    outb &= (~(byte)(ParityFlag) & 0xFF);
                }
                if (AuxCarry)
                {
                    outb |= (byte)AuxCarryFlag;
                }
                else
                {
                    outb &= (~(byte)(AuxCarryFlag) & 0xFF);
                }
                if (Zero)
                {
                    outb |= (byte)ZeroFlag;
                }
                else
                {
                    outb &= (~(byte)(ZeroFlag) & 0xFF);
                }
                if (Sign)
                {
                    outb |= (byte)SignFlag;
                }
                else
                {
                    outb &= (~(byte)(SignFlag) & 0xFF);
                }
                return outb;
            }
            set
            {
                Carry = (value & (byte)CarryFlag) > 0;
                Parity = (value & (byte)ParityFlag) > 0;
                AuxCarry = (value & (byte)AuxCarryFlag) > 0;
                Zero = (value & (byte)ZeroFlag) > 0;
                Sign = (value & (byte)SignFlag) > 0;
            }
        }

        public bool Carry
        {
            get => GetBit(CarryFlag);
            set => SetBit(CarryFlag, value);
        }
        public bool Sign
        {
            get => GetBit(SignFlag);
            private set => SetBit(SignFlag, value);
        }
        public bool Zero
        {
            get => GetBit(ZeroFlag);
            private set => SetBit(ZeroFlag, value);
        }
        public bool AuxCarry
        {
            get => GetBit(AuxCarryFlag);
            private set => SetBit(AuxCarryFlag, value);
        }
        public bool Parity
        {
            get => GetBit(ParityFlag);
            private set => SetBit(ParityFlag, value);
        }

        public void CalcCarryFlag(UInt16 result)
        {
            Carry = (result > 0xff);
        }

        public void CalcCarryFlag(byte data)
        {
            Carry = (data & 0x01) == 0x01;
        }

        public void CalcZeroFlag(UInt16 result)
        {
            Zero = ((result & 0xff) == 0);
        }

        public void CalcSignFlag(UInt16 result)
        {
            Sign = ((result & SignFlag) == SignFlag);
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
            for (byte i = 0; i != 8; ++i)
                total += (byte)((num >> i) & 1);

            Parity = (total & 1) == 0;
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
            return ((Flags & bit) == bit);
        }

        private void SetBit(byte bit, bool set)
        {
            if (set)
                Flags = (byte)(Flags | bit);
            else
                Flags = (byte)(Flags & ~bit);
        }

    }
}