using System;

namespace emu8080.Core
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        #region Private methods

        private static ushort LXI(Memory memory, Cpu cpu)
        {
            var res = cpu.Registers.ReadImmediate(memory);
            cpu.Registers.ProgramCounter += 3;
            return res;
        }

        private static byte MVI(Memory memory, Cpu cpu)
        {
            var res = memory[cpu.Registers.ProgramCounter + 1];
            cpu.Registers.ProgramCounter += 2;
            return res;
        }

        private static void LDAX(ushort stackIndex, Memory memory, Cpu cpu)
        {
            cpu.Registers.A = memory[stackIndex];
            cpu.Registers.ProgramCounter++;
        }

        private static int PUSH(ushort val, Memory memory, Cpu cpu)
        {
            cpu.Registers.StackPointer -= 2;

            memory[cpu.Registers.StackPointer + 1] = val.GetHigh();
            memory[cpu.Registers.StackPointer] = val.GetLow();

            cpu.Registers.ProgramCounter++;

            return 11;
        }

        private static ushort POP(Memory memory, Cpu cpu)
        {
            ushort res = Utils.GetValue(memory[cpu.Registers.StackPointer + 1], memory[cpu.Registers.StackPointer]);
            cpu.Registers.StackPointer += 2;
            cpu.Registers.ProgramCounter++;
            return res;
        }

        private static int JUMP_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
                cpu.Registers.ProgramCounter = cpu.Registers.ReadImmediate(memory);
            else
                cpu.Registers.ProgramCounter += 3;

            return 10;
        }

        private static int RET_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
            {
                RET(memory, cpu);
                return 11;
            }

            cpu.Registers.ProgramCounter++;
            return 5;
        }

        private static int CALL_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
            {
                CALL(memory, cpu);
                return 11;
            }

            cpu.Registers.ProgramCounter += 3;
            return 5;
        }

        private static byte ADC(Memory memory, Cpu cpu, byte a, byte b)
        {
            var result = (ushort)(a + b);
            if (cpu.Registers.Flags.Carry)
            {
                result++;
                cpu.Registers.Flags.CalcAuxCarryFlag(a, b, 1);
            }
            else
            {
                cpu.Registers.Flags.CalcAuxCarryFlag(a, b);
            }

            cpu.Registers.Flags.CalcSZPC(result);
            cpu.Registers.ProgramCounter++;
            return (byte)(result & 0xff);
        }

        private static byte SBB(Memory memory, Cpu cpu, byte a, byte b)
        {
            ushort result = (ushort)(a - b - (cpu.Registers.Flags.Carry ? 1 : 0));
            cpu.Registers.Flags.CalcSZPC(result);
            cpu.Registers.Flags.CalcAuxCarryFlag(a, (byte)(~b & 0xff), 1);

            cpu.Registers.ProgramCounter++;

            return (byte)(result & 0xff);
        }

        private static int ADD(Memory memory, Cpu cpu, byte value)
        {
            ushort result = (ushort)(cpu.Registers.A + value);
            cpu.Registers.Flags.CalcSZPC(result);
            cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, value);

            cpu.Registers.ProgramCounter++;

            cpu.Registers.A = (byte)(result & 0xff);

            return 4;
        }

        private static int SUB(Memory memory, Cpu cpu, byte value)
        {
            ushort result = (ushort)(cpu.Registers.A - value);
            cpu.Registers.Flags.CalcSZPC(result);
            cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, (byte)(~value & 0xff), 1);

            cpu.Registers.A = (byte)(result & 0xff);

            cpu.Registers.ProgramCounter++;

            return 4;
        }

        private static int ANA(Memory memory, Cpu cpu, byte value)
        {
            byte result = (byte)(cpu.Registers.A & value);

            cpu.Registers.Flags.Carry = false;
            cpu.Registers.Flags.AuxCarry = false;
            cpu.Registers.Flags.CalcZeroFlag(result);
            cpu.Registers.Flags.CalcSignFlag(result);
            cpu.Registers.Flags.CalcParityFlag(result);

            cpu.Registers.A = (byte)(result & 0xff);
            cpu.Registers.ProgramCounter++;

            return 4;
        }

        private static int XOR(Memory memory, Cpu cpu, byte value)
        {
            byte result = (byte)(cpu.Registers.A ^ value);
            cpu.Registers.Flags.CalcSZPC(result);    
            cpu.Registers.A = (byte)(result & 0xff);
            cpu.Registers.ProgramCounter++;

            return 4;
        }

        private static int AOR(Memory memory, Cpu cpu, byte value)
        {
            byte result = (byte)(cpu.Registers.A & value);

            cpu.Registers.Flags.Carry = false;
            cpu.Registers.Flags.AuxCarry = false;
            cpu.Registers.Flags.CalcZeroFlag(result);
            cpu.Registers.Flags.CalcSignFlag(result);
            cpu.Registers.Flags.CalcParityFlag(result);

            cpu.Registers.A = (byte)(result & 0xff);
            cpu.Registers.ProgramCounter++;

            return 7;
        }

        private static int CMP(Memory memory, Cpu cpu, byte value)
        {
            ushort res = (ushort)(cpu.Registers.A - value);
            cpu.Registers.Flags.CalcSZPC(res);
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        #endregion Private methods

        // 0x00
        public static int NOP(Memory memory, Cpu cpu)
        {
            //nopnopnopnopnopnopnopnopnopnop
            cpu.Registers.ProgramCounter++;

            return 4;
        }

        // 0x01 , B <- byte 3, C <- byte 2
        public static int LXI_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC = LXI(memory, cpu);
            return 10;
        }

        // 0x02 , (BC) <- A
        public static int STAX_B(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.BC] = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 4;
        }

        // 0x03 , BC <- BC+1
        public static int INX_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC++;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x04 , B <- B+1
        public static int INR_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = Utils.Increment(cpu.Registers.B, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x05 , B <- B-1
        public static int DCR_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = Utils.Decrement(cpu.Registers.B, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x06 , B <- byte 2
        public static int MVI_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = MVI(memory, cpu);
            return 7;
        }

        // 0x07 , 	A = A << 1; bit 0 = prev bit 7; CY = prev bit 7
        public static int RLC(Memory memory, Cpu cpu)
        {
            byte x = cpu.Registers.A;
            cpu.Registers.A = (byte)(((x & 0x80) >> 7) | (x << 1));
            cpu.Registers.Flags.Carry = (0x80 == (x & 0x80));
            cpu.Registers.ProgramCounter++;
            return 4;
        }

        // 0x09 , HL = HL + BC
        public static int DAD_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.DAD(cpu.Registers.BC);
            return 10;
        }

        // 0x0a , 	A <- (BC)
        public static int LDAX_B(Memory memory, Cpu cpu)
        {
            LDAX(cpu.Registers.BC, memory, cpu);
            return 7;
        }

        // 0x0b , 	BC = BC-1
        public static int DCX_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC = (ushort)(cpu.Registers.BC - 1); //TODO: check
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x0d , C <-C-1
        public static int DCR_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = Utils.Decrement(cpu.Registers.C, cpu.Registers);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.C);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x0e , C <- byte 2
        public static int MVI_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = MVI(memory, cpu);
            return 7;
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static int RRC(Memory memory, Cpu cpu)
        {
            cpu.Registers.Flags.Carry = ((cpu.Registers.A & Flags.CarryFlag) == Flags.CarryFlag);

            cpu.Registers.A = (byte)((cpu.Registers.A >> 1) & 0xff);

            if (cpu.Registers.Flags.Carry)
                cpu.Registers.A = (byte)(cpu.Registers.A | 0x80);

            cpu.Registers.ProgramCounter++;

            return 4;
        }

        // 0x11 , D <- byte 3, E <- byte 2
        public static int LXI_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.DE = LXI(memory, cpu);

            return 10;
        }

        // 0x12 , (DE) <- A
        public static int STAX_D(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.DE] = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x13 , DE <- DE + 1
        public static int INX_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.DE++;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x14 , D <- D+1
        public static int INR_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = Utils.Increment(cpu.Registers.D, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x15 , D <- D-1
        public static int DCR_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = Utils.Decrement(cpu.Registers.D, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x19 , HL = HL + DE
        public static int DAD_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.DAD(cpu.Registers.DE);
            return 10;
        }

        // 0x1a , A <- (DE)
        public static int LDAX_D(Memory memory, Cpu cpu)
        {
            LDAX(cpu.Registers.DE, memory, cpu);
            return 7;
        }

        // 0x1c , E <-E+1
        public static int INR_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = Utils.Increment(cpu.Registers.E, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x1d, E <-E-1
        public static int DCR_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = Utils.Decrement(cpu.Registers.E, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x1e , E <- byte 2
        public static int MVI_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = MVI(memory, cpu);
            return 7;
        }

        // 0x1f , A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0
        public static int RAR(Memory memory, Cpu cpu)
        {
            byte temp = (byte)(cpu.Registers.A >> 1);

            if (cpu.Registers.Flags.Carry)
                temp = (byte)(temp | 0x80);

            cpu.Registers.Flags.Carry = ((byte)(cpu.Registers.A & 0x01) == 0x01);

            cpu.Registers.A = temp;

            cpu.Registers.ProgramCounter++;

            return 4;
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static int LXI_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL = LXI(memory, cpu);
            return 10;
        }

        // 0x22 , (adr) <-L; (adr+1)<-H
        public static int SHLD(Memory memory, Cpu cpu)
        {
            var address = memory.ReadAddress(cpu.Registers.ProgramCounter + 1);
            memory[address] = cpu.Registers.L;
            memory[address + 1] = cpu.Registers.H;

            cpu.Registers.ProgramCounter += 3;

            return 16;
        }

        // 0x23 , HL <- HL + 1
        public static int INX_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL++;
            cpu.Registers.ProgramCounter++;

            return 5;
        }

        // 0x24 , D <- D+1
        public static int INR_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = Utils.Increment(cpu.Registers.H, cpu.Registers);
            cpu.Registers.ProgramCounter++;

            return 5;
        }

        // 0x25 , D <- D-1
        public static int DCR_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = Utils.Decrement(cpu.Registers.H, cpu.Registers);
            cpu.Registers.ProgramCounter++;

            return 5;
        }

        // 0x26 , H <- byte 2
        public static int MVI_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = MVI(memory, cpu);

            return 7;
        }

        // 0x29 , HL = HL + HI
        public static int DAD_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.DAD(cpu.Registers.HL);

            return 10;
        }

        // 0x2a , LHLD adr 	L <- (adr); H<-(adr+1)
        public static int LHLD(Memory memory, Cpu cpu)
        {
            //TODO tests
            var adr = cpu.Registers.ReadImmediate(memory);
            cpu.Registers.L = memory[adr];
            cpu.Registers.H = memory[adr + 1];
            cpu.Registers.ProgramCounter += 3;
            return 16;
        }

        // 0x2b , 	HL = HL-1
        public static int DCX_HL(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL = (ushort)(cpu.Registers.HL - 1); //TODO: test
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x2e , L <- byte 2
        public static int MVI_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.L = MVI(memory, cpu);

            return 7;
        }

        // 0x2F , A <- !A
        public static int CMA(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte)~cpu.Registers.A;
            cpu.Registers.ProgramCounter++;

            return 4;
        }

        // 0x31 , SP.hi <- byte 3, SP.lo <- byte 2
        public static int LXI_SP(Memory memory, Cpu cpu)
        {
            cpu.Registers.StackPointer = LXI(memory, cpu);
            return 10;
        }

        // 0x32 , (adr) <- A
        public static int STA(Memory memory, Cpu cpu)
        {
            var res = cpu.Registers.ReadImmediate(memory);
            memory[res] = cpu.Registers.A;
            cpu.Registers.ProgramCounter += 3;
            return 13;
        }

        // 0x35 , (HL) <- (HL)-1
        public static int DCR_M(Memory memory, Cpu cpu)
        {
            byte value = (byte)memory[cpu.Registers.HL];
            memory[cpu.Registers.HL] = Utils.Decrement(value, cpu.Registers);

            cpu.Registers.ProgramCounter++;

            return 10;
        }

        // 0x36 , (HL) <- byte 2
        public static int MVI_M(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.HL] = memory[cpu.Registers.ProgramCounter + 1];
            cpu.Registers.ProgramCounter += 2;
            return 10;
        }

        // 0x37 , CY = 1
        public static int STC(Memory memory, Cpu cpu)
        {
            cpu.Registers.Flags.Carry = true;
            cpu.Registers.ProgramCounter++;
            return 4;
        }

        // 0x39 , 	HL = HL + SP
        public static int DAD_SP(Memory memory, Cpu cpu)
        {
            // TODO: test
            cpu.Registers.DAD(cpu.Registers.StackPointer);
            return 10;
        }

        // 0x3a , A <- (adr)
        public static int LDA(Memory memory, Cpu cpu)
        {
            var index = cpu.Registers.ReadImmediate(memory);
            cpu.Registers.A = memory[index];
            cpu.Registers.ProgramCounter += 3;
            return 16;
        }

        // 0x3c , A <-A+1
        public static int INR_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = Utils.Increment(cpu.Registers.A, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x3d , 	A <- A-1
        public static int DCR_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = Utils.Decrement(cpu.Registers.A, cpu.Registers);
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x3e , A <- byte 2
        public static int MVI_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = MVI(memory, cpu);
            return 7;
        }

        // 0x3f , CY=!CY
        public static int CMC(Memory memory, Cpu cpu)
        {
            cpu.Registers.Flags.Carry = !cpu.Registers.Flags.Carry;
            cpu.Registers.ProgramCounter++;
            return 4;
        }


        // 0x40 , B <- C
        public static int MOV_B_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.B;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x41 , B <- C
        public static int MOV_B_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x42 , B <- D
        public static int MOV_B_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.D;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x43 , B <- E
        public static int MOV_B_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.E;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x44 , B <- H
        public static int MOV_B_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x45 , B <- L
        public static int MOV_B_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.L;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x46 , B <- (HL)
        public static int MOV_B_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x47 , MOV B,A
        public static int MOV_B_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x48 , MOV C,B
        public static int MOV_C_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.B;
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x49 , C <- C
        public static int MOV_C_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x4a , C <- D
        public static int MOV_C_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.D;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x4e , C <- (HL)
        public static int MOV_C_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x4f , C <- A
        public static int MOV_C_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x54 , D <- H
        public static int MOV_D_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x56 , D <- (HL)
        public static int MOV_D_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x57 , D <- A
        public static int MOV_D_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x5e , E <- (HL)
        public static int MOV_E_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x5f , E <- A
        public static int MOV_E_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x60 , H <- B
        public static int MOV_H_B(Memory memory, Cpu cpu)
        {
            // TODO: test
            cpu.Registers.H = cpu.Registers.B;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x61 , H <- C
        public static int MOV_H_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x64 , H <- H
        public static int MOV_H_H(Memory memory, Cpu cpu)
        {
            // TODO: test
            cpu.Registers.H = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x66 , H <- (HL)
        public static int MOV_H_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x67 , H <- A
        public static int MOV_H_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x69 , L <- C
        public static int MOV_L_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.L = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x6d , L <- L
        public static int MOV_L_L(Memory memory, Cpu cpu)
        {
            // TODO: test
            cpu.Registers.L = cpu.Registers.L;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x6f , 	L <- (HL)
        public static int MOV_L_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.L = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x76 , HLT
        public static int HLT(Memory memory, Cpu cpu)
        {
            // halts the execution, doesn't increment PC
            return 7;
        }

        // 0x77 , (HL) <- A
        public static int MOV_M_A(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.HL] = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x78 , A <- B
        public static int MOV_A_B(Memory memory, Cpu cpu)
        {
            // TODO: test
            cpu.Registers.A = cpu.Registers.B;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x79 , A <- C
        public static int MOV_A_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x7a , A <- D
        public static int MOV_A_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.D;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x7a , A <- E
        public static int MOV_A_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.E;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x7c , A <- H
        public static int MOV_A_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x7d , A <- L
        public static int MOV_A_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.L;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x7e , A <- (HL)
        public static int MOV_A_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0x7f , A <- A
        public static int MOV_A_A(Memory memory, Cpu cpu)
        {
            // TODO: test
            cpu.Registers.A = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
            return 5;
        }

        // 0x80 , A <- A + B
        public static int ADD_B(Memory memory, Cpu cpu)
            => ADD(memory, cpu, cpu.Registers.B);

        // 0x81 , A <- A + C
        public static int ADD_C(Memory memory, Cpu cpu)
            => ADD(memory, cpu, cpu.Registers.C);

        // 0x82 , A <- A + D
        public static int ADD_D(Memory memory, Cpu cpu)
            => ADD(memory, cpu, cpu.Registers.D); //TODO: test

        // 0x87 , A <- A + A
        public static int ADD_A(Memory memory, Cpu cpu)
            => ADD(memory, cpu, cpu.Registers.A);

        // 0x88 , 	A <- A + B + CY
        public static int ADC_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = ADC(memory, cpu, cpu.Registers.A, cpu.Registers.B);
            return 4;
        }

        // 0x89 , 	A <- A + C + CY
        public static int ADC_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = ADC(memory, cpu, cpu.Registers.A, cpu.Registers.C);
            return 4;
        }

        // 0x8b , 	A <- A + E + CY
        public static int ADC_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = ADC(memory, cpu, cpu.Registers.A, cpu.Registers.E);
            return 4;
        }

        // 0x8c , 	A <- A + H + CY
        public static int ADC_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = ADC(memory, cpu, cpu.Registers.A, cpu.Registers.H);
            return 4;
        }

        // 0x8d , 	A <- A + L + CY
        public static int ADC_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = ADC(memory, cpu, cpu.Registers.A, cpu.Registers.L);
            return 4;
        }

        // 0x8e , A <- A + (HL) + CY
        public static int ADC_M(Memory memory, Cpu cpu)
        {
            // TODO: test
            var value = memory[ cpu.Registers.HL ];
            cpu.Registers.A = ADC(memory, cpu, cpu.Registers.A, value);
            return 7;
        }

        // 0x90 , A <- A - B
        public static int SUB_B(Memory memory, Cpu cpu) => SUB(memory, cpu, cpu.Registers.B);

        // 0x99 , A <- A - C - CY
        public static int SBB_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, cpu.Registers.C);
            return 4;
        }

        // 0x9a , A <- A - D - CY
        public static int SBB_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, cpu.Registers.D);
            return 4;
        }

        // 0x9b , A <- A - E - CY
        public static int SBB_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, cpu.Registers.E);
            return 4;
        }

        // 0x9c , A <- A - H - CY
        public static int SBB_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, cpu.Registers.H);
            return 4;
        }

        // 0x9d , A <- A - L - CY
        public static int SBB_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, cpu.Registers.L);
            return 4;
        }

        // 0x9e , A <- A - (HL) - CY
        public static int SBB_M(Memory memory, Cpu cpu)
        {
            var mem = memory[cpu.Registers.HL];

            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, mem);
            return 7;
        }

        // 0x9f , A <- A - A - CY
        public static int SBB_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = SBB(memory, cpu, cpu.Registers.A, cpu.Registers.A);
            return 4;
        }

        // 0xa0 , A <- A & B
        public static int ANA_B(Memory memory, Cpu cpu)
        => ANA(memory, cpu, cpu.Registers.B);

        // 0xa7 , A <- A & A
        public static int ANA_A(Memory memory, Cpu cpu)
        => ANA(memory, cpu, cpu.Registers.A);

        // 0xaa , A <- A ^ D
        public static int XRA_D(Memory memory, Cpu cpu)
        => XOR(memory, cpu, cpu.Registers.D);

        // 0xaf , A <- A ^ A
        public static int XRA_A(Memory memory, Cpu cpu)
        => XOR(memory, cpu, cpu.Registers.A);

        // 0xb0 , A <- A | B
        public static int ORA_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte)((cpu.Registers.A | cpu.Registers.B) & 0xff);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.Flags.Carry = false;
            cpu.Registers.Flags.AuxCarry = false;

            cpu.Registers.ProgramCounter++;
            return 4;
        }

        // 0xb6 , A <- A | (HL)
        public static int ORA_M(Memory memory, Cpu cpu)
        {
            var m = memory[cpu.Registers.HL];
            cpu.Registers.A = (byte)((cpu.Registers.A | m) & 0xff);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0xb8 , CMP B, A - B
        public static int CMP_B(Memory memory, Cpu cpu)
            => CMP(memory, cpu, cpu.Registers.B); //TODO: test

         // 0xb9 , CMP B, A - C
        public static int CMP_C(Memory memory, Cpu cpu)
            => CMP(memory, cpu, cpu.Registers.C); //TODO: test

        // 0xba , A - D
        public static int CMP_D(Memory memory, Cpu cpu)
            => CMP(memory, cpu, cpu.Registers.D); //TODO: test

        // 0xbb , A - E
        public static int CMP_E(Memory memory, Cpu cpu)
            => CMP(memory, cpu, cpu.Registers.E); //TODO: test

        // 0xbe , A - (HL)
        public static int CMP_M(Memory memory, Cpu cpu)
        {
            //TODO: test
            byte mem = memory[cpu.Registers.HL];
            return CMP(memory, cpu, mem);
        }

        // 0xc0 , if NZ, RET
        public static int RNZ(Memory memory, Cpu cpu)
            => RET_FLAG(memory, cpu, !cpu.Registers.Flags.Zero);

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static int POP_BC(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC = POP(memory, cpu);
            return 10;
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static int JNZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Zero);
            return 10;
        }

        // 0xc3 , PC <= adr
        // 0xcb - special
        public static int JMP(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, true);
            return 10;
        }

        // 0xc4 CNZ  adr 
        public static int CNZ(Memory memory, Cpu cpu)
        {
            if (!cpu.Registers.Flags.Zero)
            {
                CALL(memory, cpu);
                return 17;
            }
            cpu.Registers.ProgramCounter += 3;
            return 11;
        }

        // 0xc5 , (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static int PUSH_B(Memory memory, Cpu cpu)
        {
            PUSH(cpu.Registers.BC, memory, cpu);
            return 11;
        }

        // 0xc6 , A <- A + byte
        public static int ADI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.Registers.ProgramCounter + 1];
            ushort sum = (ushort)(cpu.Registers.A + data);
            cpu.Registers.A = (byte)(sum & 0xFF);
            cpu.Registers.Flags.CalcSZPC(sum);
            cpu.Registers.ProgramCounter += 2;
            return 7;
        }

        // 0xc8 , if Z, RET
        public static int RZ(Memory memory, Cpu cpu)
            => RET_FLAG(memory, cpu, cpu.Registers.Flags.Zero);

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static int RET(Memory memory, Cpu cpu)
        {
            cpu.Registers.ProgramCounter = POP(memory, cpu);
            return 10;
        }

        // 0xca , if Z, PC <- adr
        public static int JZ(Memory memory, Cpu cpu)
            => JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Zero);

        // 0xcc CZ,	if Z, CALL adr 
        public static int CZ(Memory memory, Cpu cpu)
            => CALL_FLAG(memory, cpu, cpu.Registers.Flags.Zero);

        // 0xcd , (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static int CALL(Memory memory, Cpu cpu)
        {
            // adding +3 so that 
            var retAddr = cpu.Registers.ProgramCounter + 3;

            memory[cpu.Registers.StackPointer - 1] = retAddr.GetHigh();
            memory[cpu.Registers.StackPointer - 2] = retAddr.GetLow();

            cpu.Registers.StackPointer -= 2;// TODO: I'm not sure about this

            cpu.Registers.ProgramCounter = cpu.Registers.ReadImmediate(memory);

            return 17;
        }

        // 0xce , A <- A + data + CY
        public static int ACI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.Registers.ProgramCounter + 1];
            ushort sum = (ushort)(cpu.Registers.A + data);
            if (cpu.Registers.Flags.Carry)
            {
                sum++;
                cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, data, 1);
            }
            else
            {
                cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, data);
            }

            cpu.Registers.Flags.CalcSZPC(sum);
            cpu.Registers.A = (byte)(sum & 0xFF);

            cpu.Registers.ProgramCounter += 2;

            return 7;
        }

        // 0xd0 , if NCY, RET
        public static int RNC(Memory memory, Cpu cpu)
            => RET_FLAG(memory, cpu, !cpu.Registers.Flags.Carry); 

        // 0xd1 , E <- (sp); D <- (sp+1); sp <- sp+2
        public static int POP_DE(Memory memory, Cpu cpu)
        {
            cpu.Registers.DE = POP(memory, cpu);
            return 10;
        }

        // 0xd2 , 	if NCY, PC<-adr
        public static int JNC(Memory memory, Cpu cpu)
            => JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Carry);

        // 0xd3
        public static int OUT(Memory memory, Cpu cpu)
        {
            // TODO: accumulator is written out to the port
            cpu.Registers.ProgramCounter += 2;
            return 8;
        }

        // 0xd5 , (sp-2)<-E; (sp-1)<-D; sp <- sp - 2
        public static int PUSH_DE(Memory memory, Cpu cpu)
            => PUSH(cpu.Registers.DE, memory, cpu);

        // 0xd6 , A <- A - data
        public static int SUI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.Registers.ProgramCounter + 1];
            ushort diff = (ushort)(cpu.Registers.A - data);
            cpu.Registers.A = (byte)(diff & 0xFF);
            cpu.Registers.Flags.CalcSZPC(diff);
            cpu.Registers.ProgramCounter += 2;
            return 7;
        }

        // 0xd8 , if CY, RET
        public static int RC(Memory memory, Cpu cpu)
            => RET_FLAG(memory, cpu, cpu.Registers.Flags.Carry);

        // 0xda , if CY, PC<-adr
        public static int JC(Memory memory, Cpu cpu)
            => JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Carry);

        // 0xdb
        public static int IN_D8(Memory memory, Cpu cpu)
        {
            // TODO: ?
            cpu.Registers.ProgramCounter++;
            return 10;
        }

        // 0xe1 , L <- (sp); H <- (sp+1); sp <- sp+2
        public static int POP_HL(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL = POP(memory, cpu);
            return 10;
        }

        // 0xe2 JPO adr 
        public static int JPO(Memory memory, Cpu cpu)
            => JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Parity);

        // 0xe3 , L <-> (SP); H <-> (SP+1)
        public static int XTHL(Memory memory, Cpu cpu)
        {
            byte t = cpu.Registers.L;
            cpu.Registers.L = memory[cpu.Registers.StackPointer];
            memory[cpu.Registers.StackPointer] = t;

            t = cpu.Registers.H;
            cpu.Registers.H = memory[cpu.Registers.StackPointer + 1];
            memory[cpu.Registers.StackPointer + 1] = t;

            cpu.Registers.ProgramCounter++;

            return 18;
        }

        // 0xe5 , (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
        public static int PUSH_HL(Memory memory, Cpu cpu)
        {
            PUSH(cpu.Registers.HL, memory, cpu);
            return 11;
        }

        // 0xe6 , A <- A & data
        public static int ANI(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.Registers.ProgramCounter + 1];
            ANA(memory, cpu, data);
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0xe8 , if PE, RET
        public static int RPE(Memory memory, Cpu cpu)
            => RET_FLAG(memory, cpu, cpu.Registers.Flags.Parity); // TODO: test

        // 0xe9 , PC.hi <- H; PC.lo <- L
        public static int PCHL(Memory memory, Cpu cpu)
        {
            cpu.Registers.ProgramCounter = cpu.Registers.HL;
            return 5;
        }

        // 0xea JPE adr 
        public static int JPE(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Parity);
            return 10;
        }

        // 0xeb , H <-> D; L <-> E
        public static int XCHG(Memory memory, Cpu cpu)
        {
            byte t = cpu.Registers.H;
            cpu.Registers.H = cpu.Registers.D;
            cpu.Registers.D = t;

            t = cpu.Registers.L;
            cpu.Registers.L = cpu.Registers.E;
            cpu.Registers.E = t;

            cpu.Registers.ProgramCounter++;

            return 5;
        }

        // 0xec CPE adr, if PE, CALL adr
        public static int CPE(Memory memory, Cpu cpu)
            => CALL_FLAG(memory, cpu, cpu.Registers.Flags.Parity); // TODO: test

        // 0xee XRI A <- A ^ data
        public static int XRI(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.Registers.ProgramCounter + 1];
            XOR(memory, cpu, data);
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0xef , CALL $28
        public static int RST_5(Memory memory, Cpu cpu)
        {
            CALL(memory, cpu);
            cpu.Registers.ProgramCounter = 0x28;
            return 11;
        }

        // 0xf1 , flags <- (sp); A <- (sp+1); sp <- sp+2
        public static int POP_PSW(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = memory[cpu.Registers.StackPointer + 1];
            var psw = memory[cpu.Registers.StackPointer];

            cpu.Registers.Flags.PSW = psw;

            cpu.Registers.StackPointer += 2;
            cpu.Registers.ProgramCounter++;
            return 10;
        }

        // 0xf2 JP adr 
        public static int JP(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Sign);
            return 10;
        }

        // 0xf3
        public static int DI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(false);
            cpu.Registers.ProgramCounter++;
            return 4;
        }

        // 0xf4 CP adr 
        public static int CP(Memory memory, Cpu cpu)
            => CALL_FLAG(memory, cpu, cpu.Registers.Flags.Sign); 

        // 0xf5 , (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
        public static int PUSH_PSW(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.StackPointer - 1] = cpu.Registers.A;
            memory[cpu.Registers.StackPointer - 2] = cpu.Registers.Flags.PSW;

            cpu.Registers.StackPointer -= 2;
            cpu.Registers.ProgramCounter++;
            return 11;
        }

        // 0xf6 ORI D8 A <- A | data
        public static int ORI_D(Memory memory, Cpu cpu)
        {
            // TODO: tests
            var data = memory[cpu.Registers.ProgramCounter + 1];
            AOR(memory, cpu, data);
            cpu.Registers.ProgramCounter++;
            return 7;
        }

        // 0xf8 , 	if M, RET
        public static int RM(Memory memory, Cpu cpu)
            => RET_FLAG(memory, cpu, cpu.Registers.Flags.Sign);

        // 0xfa JM adr 
        public static int JM(Memory memory, Cpu cpu)
            => JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Sign);

        // 0xfb 
        public static int EI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(true);
            cpu.Registers.ProgramCounter++;
            return 4;
        }

        // 0xfc CM adr 
        public static int CM(Memory memory, Cpu cpu)
        {
            if (cpu.Registers.Flags.Sign)
            {
                CALL(memory, cpu);
                return 17;
            }

            cpu.Registers.ProgramCounter += 3;

            return 11;
        }

        // 0xfe , A - data
        public static int CPI(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.Registers.ProgramCounter + 1];
            var diff = (ushort)(cpu.Registers.A - data);
            cpu.Registers.Flags.CalcSZPC(diff);
            cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, (byte)(~data & 0xff), 1);
            cpu.Registers.ProgramCounter += 2;
            return 7;
        }

        // 0xff , CALL $38
        public static int RST_7(Memory memory, Cpu cpu)
        {
            CALL(memory, cpu);
            cpu.Registers.ProgramCounter = 0x38;
            return 11;
        }
    }
}