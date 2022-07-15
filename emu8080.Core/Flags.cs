using System;

namespace emu8080.Core
{
    /// <summary>
    /// https://github.com/amensch/e8080/blob/master/e8080/Intel8080/i8080ConditionalRegister.cs
    /// </summary>
    public class Flags{
        public const byte SignFlag = 0x80;        // 0=positive, 1=negative
        public const byte ZeroFlag = 0x40;        // 0=non-zero, 1=zero
        public const byte AuxCarryFlag = 0x08;   // not implemented for now
        public const byte ParityFlag = 0x04;      // 0=odd, 1=even
        public const byte CarryFlag = 0x01;       // 0=no carry, 1=carry

        private byte _flags = 0;
       
        public void Reset()
        {
            _flags = 0;
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
            set => SetBit(AuxCarryFlag, value);
        }
        public bool Parity
        {
            get => GetBit(ParityFlag);
            private set => SetBit(ParityFlag, value);
        }

        public void CalcCarryFlag(int result)
        {
            Carry = ((result & 0xffff0000) != 0);
        }

        public void CalcCarryFlag(UInt16 value)
        {
            Carry = (value > 0xff);
        }

        public void CalcCarryFlag(byte data)
        {
            Carry = (data & 0x01) == 0x01;
        }

        public void CalcZeroFlag(UInt16 value)
        {
            Zero = ((value & 0xff) == 0);
        }

        public void CalcZeroFlag(byte value)
        {
            Zero = ((value & 0xff) == 0);
        }

        public void CalcSignFlag(UInt16 value)
        {
            Sign = ((value & SignFlag) == SignFlag);
        }

        public void CalcSignFlag(byte value)
        {
            Sign = ((value & SignFlag) == SignFlag);
        }

        public void CalcParityFlag(byte result)
        {
            Parity = Utils.GetParity(result);
        }

        public void CalcAuxCarryFlag(byte a, byte b)
        {
            AuxCarry = (a & b) == 0; 
        }

        public void CalcAuxCarryFlag(byte a, byte b, byte c)
        {
            AuxCarry = (byte)((a & 0x0f) + (b & 0x0f) + (c & 0x0f)) > 0x0f;
        }

        /// Sign, Zero, Parity, Carry
        public void CalcSZPC(ushort val){
            this.CalcZeroFlag(val);
            this.CalcSignFlag(val);
            this.CalcParityFlag( (byte)(val&0xff));
            this.CalcCarryFlag(val);
        }

        /// Sign, Zero, Parity, Carry
        public void CalcSZPC(byte val)
        {
            this.CalcZeroFlag(val);
            this.CalcSignFlag(val);
            this.CalcParityFlag(val);
            this.CalcCarryFlag(val);
        }

        public byte PSW{
            get => _flags;
            set
            {
                _flags = value;
                //this.Zero  = (0x01 == (value & 0x01));
                //this.Sign  = (0x02 == (value & 0x02));
                //this.Parity  = (0x04 == (value & 0x04));
                //this.Carry = (0x05 == (value & 0x08));
                //this.AuxCarry = (0x10 == (value & 0x10));
            }
        }

        private bool GetBit(byte bit)
        {
            return ((_flags & bit) == bit);
        }

        private void SetBit(byte bit, bool set)
        {
            if (set)
                _flags = (byte)(_flags | bit);
            else
                _flags = (byte)(_flags & ~bit);
        }

        public override string ToString(){
            return $"{(Zero?"Z":string.Empty)} {(Sign?"S":string.Empty)} {(Parity?"P":string.Empty)} {(Carry?"C":string.Empty)} {(AuxCarry?"AC":string.Empty)}";
        }
    }
}