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
            var res = Utils.GetValue(memory[cpu.Registers.ProgramCounter + 2], memory[cpu.Registers.ProgramCounter + 1]);
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

        private static void WriteMemory(Memory memory, ushort address, byte value)
        {
            if (address < 0x2000)
                return; // Writing ROM not allowed

            if (address >= 0x4000)
                return; // Writing out of Space Invaders RAM not allowed
           
            memory[address] = value;
        }

        private static void PUSH(ushort val, Memory memory, Cpu cpu)
        {
            WriteMemory(memory, (ushort) (cpu.Registers.StackPointer - 1), val.GetHigh());
            WriteMemory(memory, (ushort) (cpu.Registers.StackPointer - 2), val.GetLow());

            cpu.Registers.StackPointer -= 2;
            cpu.Registers.ProgramCounter += 1;
        }

        private static ushort POP(Memory memory, Cpu cpu)
        {
            ushort res = Utils.GetValue(memory[cpu.Registers.StackPointer + 1], memory[cpu.Registers.StackPointer]);
            cpu.Registers.StackPointer += 2;
            cpu.Registers.ProgramCounter++;
            return res;
        }

        private static void JUMP_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
                cpu.Registers.SetCounterToAddr(memory);
            else
                cpu.Registers.ProgramCounter += 3;
        }

        private static void RET_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
                RET(memory, cpu);
            else
                cpu.Registers.ProgramCounter++;
        }

        private static void ADC(Memory memory, Cpu cpu, byte value)
        {
            var result = (ushort) (cpu.Registers.A + value);
            if (cpu.Registers.Flags.Carry)
            {
                result++;
                cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, value, 1);
            }
            else
            {
                cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, value);
            }

            cpu.Registers.Flags.CalcSZPC(result);

            cpu.Registers.A = (byte) (result & 0xff);

            cpu.Registers.ProgramCounter++;
        }

        private static void SBB(Memory memory, Cpu cpu, byte value)
        {
            ushort result = (ushort) (cpu.Registers.A - value - (cpu.Registers.Flags.Carry ? 1 : 0));
            cpu.Registers.Flags.CalcSZPC(result);
            cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, (byte) (~value & 0xff), 1);

            cpu.Registers.A = (byte) (result & 0xff);

            cpu.Registers.ProgramCounter++;
        }

        public static void SUB(Memory memory, Cpu cpu, byte value)
        {
            ushort result = (ushort) (cpu.Registers.A - value);
            cpu.Registers.Flags.CalcSZPC(result);
            cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, (byte) (~value & 0xff), 1);

            cpu.Registers.A = (byte) (result & 0xff);

            cpu.Registers.ProgramCounter++;
        }

        private static void ANA(Memory memory, Cpu cpu, byte value)
        {
            byte result = (byte) (cpu.Registers.A & value);

            cpu.Registers.Flags.Carry = false;
            cpu.Registers.Flags.AuxCarry = false;
            cpu.Registers.Flags.CalcZeroFlag(result);
            cpu.Registers.Flags.CalcSignFlag(result);
            cpu.Registers.Flags.CalcParityFlag(result);

            cpu.Registers.A = (byte) (result & 0xff);
            cpu.Registers.ProgramCounter++;
        }

        #endregion Private methods

        // 0x00
        public static void NOP(Memory memory, Cpu cpu)
        {
            //nopnopnopnopnopnopnopnopnopnop
            cpu.Registers.ProgramCounter++;
        }

        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC = LXI(memory, cpu);
        }

        // 0x02 , (BC) <- A
        public static void STAX_B(Memory memory, Cpu cpu)
        {
            WriteMemory(memory, cpu.Registers.BC, cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
        }

        // 0x03 , BC <- BC+1
        public static void INX_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC++;
            cpu.Registers.ProgramCounter++;
        }

        // 0x04 , B <- B+1
        public static void INR_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = Utils.Increment(cpu.Registers.B, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x05 , B <- B-1
        public static void DCR_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = Utils.Decrement(cpu.Registers.B, cpu.Registers);

            cpu.Registers.ProgramCounter++;
        }

        // 0x06 , B <- byte 2
        public static void MVI_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = MVI(memory, cpu);
        }

        // 0x07 , 	A = A << 1; bit 0 = prev bit 7; CY = prev bit 7
        public static void RLC(Memory memory, Cpu cpu)
        {
            byte x = cpu.Registers.A;
            cpu.Registers.A = (byte) (((x & 0x80) >> 7) | (x << 1));
            cpu.Registers.Flags.Carry = (0x80 == (x & 0x80));
            cpu.Registers.ProgramCounter++;
        }

        // 0x09 , HL = HL + BC
        public static void DAD_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.DAD(cpu.Registers.BC);
        }

        // 0x0a , 	A <- (BC)
        public static void LDAX_B(Memory memory, Cpu cpu)
        {
            LDAX(cpu.Registers.BC, memory, cpu);
        }

        // 0x0b , 	BC = BC-1
        public static void DCX_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC = (ushort) (cpu.Registers.BC - 1); //TODO: check
            cpu.Registers.ProgramCounter++;
        }

        // 0x0d , C <-C-1
        public static void DCR_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = Utils.Decrement(cpu.Registers.C, cpu.Registers);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.C);
            cpu.Registers.ProgramCounter++;
        }

        // 0x0e , C <- byte 2
        public static void MVI_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = MVI(memory, cpu);
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static void RRC(Memory memory, Cpu cpu)
        {
            cpu.Registers.Flags.Carry = ((cpu.Registers.A & Flags.CarryFlag) == Flags.CarryFlag);

            cpu.Registers.A = (byte) ((cpu.Registers.A >> 1) & 0xff);

            if (cpu.Registers.Flags.Carry)
                cpu.Registers.A = (byte) (cpu.Registers.A | 0x80);

            cpu.Registers.ProgramCounter++;
        }

        // 0x11 , D <- byte 3, E <- byte 2
        public static void LXI_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.DE = LXI(memory, cpu);
        }

        // 0x12 , (DE) <- A
        public static void STAX_D(Memory memory, Cpu cpu)
        {
            WriteMemory(memory, cpu.Registers.DE, cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
        }

        // 0x13 , DE <- DE + 1
        public static void INX_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.DE++;
            cpu.Registers.ProgramCounter++;
        }

        // 0x14 , D <- D+1
        public static void INR_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = Utils.Increment(cpu.Registers.D, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x15 , D <- D-1
        public static void DCR_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = Utils.Decrement(cpu.Registers.D, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x19 , HL = HL + DE
        public static void DAD_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.DAD(cpu.Registers.DE);
        }

        // 0x1a , A <- (DE)
        public static void LDAX_D(Memory memory, Cpu cpu)
        {
            LDAX(cpu.Registers.DE, memory, cpu);
        }

        // 0x1c , E <-E+1
        public static void INR_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = Utils.Increment(cpu.Registers.E, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x1e , E <- byte 2
        public static void MVI_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = MVI(memory, cpu);
        }

        // 0x1f , A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0
        public static void RAR(Memory memory, Cpu cpu)
        {
            byte temp = (byte) (cpu.Registers.A >> 1);

            if (cpu.Registers.Flags.Carry)
                temp = (byte) (temp | 0x80);

            cpu.Registers.Flags.Carry = ((byte) (cpu.Registers.A & 0x01) == 0x01);

            cpu.Registers.A = temp;

            cpu.Registers.ProgramCounter++;
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL = LXI(memory, cpu);
        }

        // 0x22 , (adr) <-L; (adr+1)<-H
        public static void SHLD(Memory memory, Cpu cpu)
        {
            //TODO: check
            var address = Utils.GetValue(memory[cpu.Registers.ProgramCounter + 1], memory[cpu.Registers.ProgramCounter]);
            WriteMemory(memory, address, cpu.Registers.L);
            WriteMemory(memory, (ushort) (address + 1), cpu.Registers.H);

            cpu.Registers.ProgramCounter += 3;
        }

        // 0x23 , HL <- HL + 1
        public static void INX_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL++;
            cpu.Registers.ProgramCounter++;
        }

        // 0x24 , D <- D+1
        public static void INR_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = Utils.Increment(cpu.Registers.H, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x25 , D <- D-1
        public static void DCR_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = Utils.Decrement(cpu.Registers.H, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x26 , H <- byte 2
        public static void MVI_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = MVI(memory, cpu);
        }

        // 0x29 , HL = HL + HI
        public static void DAD_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.DAD(cpu.Registers.HL);
        }

        // 0x2e , L <- byte 2
        public static void MVI_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.L = MVI(memory, cpu);
        }

        // 0x2F , A <- !A
        public static void CMA(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte) ~cpu.Registers.A;
        }

        // 0x31 , SP.hi <- byte 3, SP.lo <- byte 2
        public static void LXI_SP(Memory memory, Cpu cpu)
        {
            cpu.Registers.StackPointer = LXI(memory, cpu);
        }

        // 0x32 , (adr) <- A
        public static void STA(Memory memory, Cpu cpu)
        {
            var res = Utils.GetValue(memory[cpu.Registers.ProgramCounter + 2], memory[cpu.Registers.ProgramCounter + 1]);
            memory[res] = cpu.Registers.A;
            cpu.Registers.ProgramCounter += 3;
        }

        // 0x35 , (HL) <- (HL)-1
        public static void DCR_M(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.HL] = (byte) (memory[cpu.Registers.HL] - 1);
            cpu.Registers.ProgramCounter++;
        }

        // 0x36 , (HL) <- byte 2
        public static void MVI_M(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.HL] = memory[cpu.Registers.ProgramCounter + 1];
            cpu.Registers.ProgramCounter += 2;
        }

        // 0x37 , CY = 1
        public static void STC(Memory memory, Cpu cpu)
        {
            cpu.Registers.Flags.Carry = true;
            cpu.Registers.ProgramCounter++;
        }

        // 0x3a , A <- (adr)
        public static void LDA(Memory memory, Cpu cpu)
        {
            var index = Utils.GetValue(memory[cpu.Registers.ProgramCounter + 2], memory[cpu.Registers.ProgramCounter + 1]);
            cpu.Registers.A = memory[index];
            cpu.Registers.ProgramCounter += 3;
        }

        // 0x3c , A <-A+1
        public static void INR_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = Utils.Increment(cpu.Registers.A, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x3d , 	A <- A-1
        public static void DCR_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = Utils.Decrement(cpu.Registers.A, cpu.Registers);
            cpu.Registers.ProgramCounter++;
        }

        // 0x3e , A <- byte 2
        public static void MVI_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = MVI(memory, cpu);
        }

        // 0x3f , CY=!CY
        public static void CMC(Memory memory, Cpu cpu)
        {
            cpu.Registers.Flags.Carry = !cpu.Registers.Flags.Carry;
            cpu.Registers.ProgramCounter++;
        }


        // 0x40 , B <- C
        public static void MOV_B_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.B;
            cpu.Registers.ProgramCounter++;
        }

        // 0x41 , B <- C
        public static void MOV_B_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.D;
            cpu.Registers.ProgramCounter++;
        }

        // 0x43 , B <- E
        public static void MOV_B_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.E;
            cpu.Registers.ProgramCounter++;
        }

        // 0x44 , B <- H
        public static void MOV_B_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
        }

        // 0x46 , B <- (HL)
        public static void MOV_B_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
        }

        // 0x49 , C <- C
        public static void MOV_C_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
        }

        // 0x4a , C <- D
        public static void MOV_C_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.D;
            cpu.Registers.ProgramCounter++;
        }

        // 0x4e , C <- (HL)
        public static void MOV_C_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
        }

        // 0x4f , C <- A
        public static void MOV_C_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.C = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
        }

        // 0x54 , D <- H
        public static void MOV_D_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
        }

        // 0x56 , D <- (HL)
        public static void MOV_D_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
        }

        // 0x57 , D <- A
        public static void MOV_D_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.D = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
        }

        // 0x5f , E <- A
        public static void MOV_E_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
        }

        // 0x5e , E <- (HL)
        public static void MOV_E_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.E = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
        }

        // 0x61 , H <- C
        public static void MOV_H_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
        }

        // 0x66 , H <- (HL)
        public static void MOV_H_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
        }

        // 0x67 , H <- A
        public static void MOV_H_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.H = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
        }

        // 0x69 , L <- C
        public static void MOV_L_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.L = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
        }

        // 0x6f , 	L <- (HL)
        public static void MOV_L_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.L = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
        }

        // 0x77 , (HL) <- A
        public static void MOV_M_A(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.HL] = cpu.Registers.A;
            cpu.Registers.ProgramCounter++;
        }

        // 0x79 , A <- C
        public static void MOV_A_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.C;
            cpu.Registers.ProgramCounter++;
        }

        // 0x7a , A <- D
        public static void MOV_A_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.D;
            cpu.Registers.ProgramCounter++;
        }

        // 0x7a , A <- E
        public static void MOV_A_E(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.E;
            cpu.Registers.ProgramCounter++;
        }


        // 0x7c , A <- H
        public static void MOV_A_H(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.H;
            cpu.Registers.ProgramCounter++;
        }

        // 0x7d , A <- L
        public static void MOV_A_L(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = cpu.Registers.L;
            cpu.Registers.ProgramCounter++;
        }

        // 0x7e , A <- (HL)
        public static void MOV_A_M(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = memory[cpu.Registers.HL];
            cpu.Registers.ProgramCounter++;
        }

        // 0x80 , A <- A + B
        public static void ADD_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte) ((cpu.Registers.A + cpu.Registers.B) & 0xff);
            cpu.Registers.ProgramCounter++;
        }

        // 0x81 , A <- A + C
        public static void ADD_C(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte) ((cpu.Registers.A + cpu.Registers.C) & 0xff);
            cpu.Registers.ProgramCounter++;
        }

        // 0x87 , 	A <- A + A + CY
        public static void ADC_A(Memory memory, Cpu cpu)
        {
            ADC(memory, cpu, cpu.Registers.A);
        }

        // 0x88 , 	A <- A + B + CY
        public static void ADC_B(Memory memory, Cpu cpu)
        {
            ADC(memory, cpu, cpu.Registers.B);
        }

        // 0x89 , 	A <- A + C + CY
        public static void ADC_C(Memory memory, Cpu cpu)
        {
            ADC(memory, cpu, cpu.Registers.C);
        }

        // 0x8b , 	A <- A + E + CY
        public static void ADC_E(Memory memory, Cpu cpu)
        {
            ADC(memory, cpu, cpu.Registers.E);
        }

        // 0x8c , 	A <- A + H + CY
        public static void ADC_H(Memory memory, Cpu cpu)
        {
            ADC(memory, cpu, cpu.Registers.H);
        }

        // 0x8d , 	A <- A + L + CY
        public static void ADC_L(Memory memory, Cpu cpu)
        {
            ADC(memory, cpu, cpu.Registers.L);
        }

        // 0x90 , A <- A - B
        public static void SUB_B(Memory memory, Cpu cpu) => SUB(memory, cpu, cpu.Registers.B);

        // 0x99 , A <- A - C - CY
        public static void SBB_C(Memory memory, Cpu cpu)
        {
            SBB(memory, cpu, cpu.Registers.C);
        }

        // 0x9a , A <- A - D - CY
        public static void SBB_D(Memory memory, Cpu cpu)
        {
            SBB(memory, cpu, cpu.Registers.D);
        }

        // 0x9b , A <- A - E - CY
        public static void SBB_E(Memory memory, Cpu cpu)
        {
            SBB(memory, cpu, cpu.Registers.E);
        }

        // 0x9c , A <- A - H - CY
        public static void SBB_H(Memory memory, Cpu cpu)
        {
            SBB(memory, cpu, cpu.Registers.H);
        }

        // 0x9d , A <- A - L - CY
        public static void SBB_L(Memory memory, Cpu cpu)
        {
            SBB(memory, cpu, cpu.Registers.L);
        }

        // 0x9e , A <- A - (HL) - CY
        public static void SBB_M(Memory memory, Cpu cpu)
        {
            var mem = memory[cpu.Registers.HL];

            SBB(memory, cpu, mem);
        }

        // 0x9f , A <- A - A - CY
        public static void SBB_A(Memory memory, Cpu cpu)
        {
            SBB(memory, cpu, cpu.Registers.A);
        }

        // 0xa0 , A <- A & B
        public static void ANA_B(Memory memory, Cpu cpu)
        {
            ANA(memory, cpu, cpu.Registers.B);
        }

        // 0xa7 , A <- A & A
        public static void ANA_A(Memory memory, Cpu cpu)
        {
            ANA(memory, cpu, cpu.Registers.A);
        }

        // 0xaa , A <- A ^ D
        public static void XRA_D(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte) ((cpu.Registers.A ^ cpu.Registers.D) & 0xff);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
        }

        // 0xaf , A <- A ^ A
        public static void XRA_A(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = (byte) ((cpu.Registers.A ^ cpu.Registers.A) & 0xff);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
        }

        // 0xb0 , A <- A | B
        public static void ORA_B(Memory memory, Cpu cpu)
        {
            cpu.Registers.B = (byte) ((cpu.Registers.A | cpu.Registers.B) & 0xff);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
        }

        // 0xb6 , A <- A | (HL)
        public static void ORA_M(Memory memory, Cpu cpu)
        {
            var m = memory[cpu.Registers.HL];
            cpu.Registers.A = (byte) ((cpu.Registers.A | m) & 0xff);
            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.ProgramCounter++;
        }

        // 0xbe , A - (HL)
        public static void CMP_M(Memory memory, Cpu cpu)
        {
            var mem = (ushort) memory[cpu.Registers.HL];
            ushort res = (ushort) (cpu.Registers.A - mem);
            cpu.Registers.Flags.CalcSZPC(res);
            cpu.Registers.ProgramCounter++;
        }

        // 0xc0 , if NZ, RET
        public static void RNZ(Memory memory, Cpu cpu)
        {
            if (!cpu.Registers.Flags.Zero)
                RET(memory, cpu);
            else
                cpu.Registers.StackPointer++;
        }

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static void POP_BC(Memory memory, Cpu cpu)
        {
            cpu.Registers.BC = POP(memory, cpu);
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static void JNZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Zero);
        }

        // 0xc3 , PC <= adr
        public static void JMP(Memory memory, Cpu cpu)
        {
            cpu.Registers.SetCounterToAddr(memory);
        }

        // 0xc4 CNZ  adr 
        public static void CNZ(Memory memory, Cpu cpu)
        {
            if (!cpu.Registers.Flags.Zero)
                CALL(memory, cpu);
            else
                cpu.Registers.ProgramCounter += 3;
        }

        // 0xc5 , (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static void PUSH_CD(Memory memory, Cpu cpu)
        {
            PUSH(cpu.Registers.BC, memory, cpu);
        }

        public static void PUSH_PC(Memory memory, Cpu cpu)
        {
            PUSH(cpu.Registers.ProgramCounter, memory, cpu);
        }

        // 0xc6 , A <- A + byte
        public static void ADI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.Registers.ProgramCounter + 1];
            ushort sum = (ushort) (cpu.Registers.A + data);
            cpu.Registers.A = (byte) (sum & 0xFF);
            cpu.Registers.Flags.CalcSZPC(sum);
            cpu.Registers.ProgramCounter += 2;
        }

        // 0xc8 , if Z, RET
        public static void RZ(Memory memory, Cpu cpu)
        {
            if (cpu.Registers.Flags.Zero)
                RET(memory, cpu);
            else
                cpu.Registers.ProgramCounter++;
        }

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static void RET(Memory memory, Cpu cpu)
        {
            byte lo = memory[cpu.Registers.StackPointer];
            byte hi = memory[cpu.Registers.StackPointer + 1];
            cpu.Registers.ProgramCounter = Utils.GetValue(hi, lo);
            cpu.Registers.StackPointer += 2;
        }

        // 0xca , if Z, PC <- adr
        public static void JZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Zero);
        }

        // 0xcc CZ  adr 
        public static void CZ(Memory memory, Cpu cpu)
        {
            if (cpu.Registers.Flags.Zero)
                CALL(memory, cpu);
            else
                cpu.Registers.ProgramCounter += 3;
        }

        // 0xcd , (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static void CALL(Memory memory, Cpu cpu)
        {
            var retAddr = cpu.Registers.ProgramCounter + 2;

            memory[cpu.Registers.StackPointer - 1] = retAddr.GetHigh();
            memory[cpu.Registers.StackPointer - 2] = retAddr.GetLow();

            cpu.Registers.StackPointer -= 2;

            cpu.Registers.SetCounterToAddr(memory);
        }

        // 0xce , A <- A + data + CY
        public static void ACI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.Registers.ProgramCounter + 1];
            ushort sum = (ushort) (cpu.Registers.A + data + (cpu.Registers.Flags.Carry ? 1 : 0));
            cpu.Registers.A = (byte) (sum & 0xFF);

            cpu.Registers.Flags.CalcSZPC(cpu.Registers.A);
            cpu.Registers.ProgramCounter += 2;
        }

        // 0xd1 , E <- (sp); D <- (sp+1); sp <- sp+2
        public static void POP_DE(Memory memory, Cpu cpu)
        {
            cpu.Registers.DE = POP(memory, cpu);
        }

        // 0xd2 , 	if NCY, PC<-adr
        public static void JNC(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Carry);
        }

        // 0xd3
        public static void OUT(Memory memory, Cpu cpu)
        {
            //TODO accumulator is written out to the port
            cpu.Registers.ProgramCounter += 2;
        }

        // 0xd5 , (sp-2)<-E; (sp-1)<-D; sp <- sp - 2
        public static void PUSH_DE(Memory memory, Cpu cpu)
        {
            PUSH(cpu.Registers.DE, memory, cpu);
        }

        // 0xd6 , A <- A - data
        public static void SUI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.Registers.ProgramCounter + 1];
            ushort diff = (ushort) (cpu.Registers.A - data);
            cpu.Registers.A = (byte) (diff & 0xFF);
            cpu.Registers.Flags.CalcSZPC(diff);
            cpu.Registers.ProgramCounter += 2;
        }

        // 0xd8 , if CY, RET
        public static void RC(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, cpu.Registers.Flags.Carry);
        }

        // 0xda , if CY, PC<-adr
        public static void JC(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Carry);
        }

        // 0xdb
        public static void IN_D8(Memory memory, Cpu cpu)
        {
            //TODO ?
            cpu.Registers.ProgramCounter++;
        }

        // 0xe1 , L <- (sp); H <- (sp+1); sp <- sp+2
        public static void POP_HL(Memory memory, Cpu cpu)
        {
            cpu.Registers.HL = POP(memory, cpu);
        }

        // 0xe2 JPO adr 
        public static void JPO(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Parity);
        }

        // 0xe3 , L <-> (SP); H <-> (SP+1)
        public static void XTHL(Memory memory, Cpu cpu)
        {
            byte t = cpu.Registers.L;
            cpu.Registers.L = memory[cpu.Registers.StackPointer];
            memory[cpu.Registers.StackPointer] = t;

            t = cpu.Registers.H;
            cpu.Registers.H = memory[cpu.Registers.StackPointer + 1];
            memory[cpu.Registers.StackPointer + 1] = t;

            cpu.Registers.ProgramCounter++;
        }

        // 0xe5 , (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
        public static void PUSH_HL(Memory memory, Cpu cpu)
        {
            PUSH(cpu.Registers.HL, memory, cpu);
        }

        // 0xe6 , A <- A & data
        public static void ANI(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.Registers.ProgramCounter + 1];
            ANA(memory, cpu, data);
            cpu.Registers.ProgramCounter++;
        }

        // 0xe9 , PC.hi <- H; PC.lo <- L
        public static void PCHL(Memory memory, Cpu cpu)
        {
            cpu.Registers.ProgramCounter = cpu.Registers.HL;
        }

        // 0xea JPE adr 
        public static void JPE(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Parity);
        }

        // 0xeb , H <-> D; L <-> E
        public static void XCHG(Memory memory, Cpu cpu)
        {
            byte t = cpu.Registers.H;
            cpu.Registers.H = cpu.Registers.D;
            cpu.Registers.D = t;

            t = cpu.Registers.L;
            cpu.Registers.L = cpu.Registers.E;
            cpu.Registers.E = t;

            cpu.Registers.ProgramCounter++;
        }

        // 0xf1 , flags <- (sp); A <- (sp+1); sp <- sp+2
        public static void POP_PSW(Memory memory, Cpu cpu)
        {
            cpu.Registers.A = memory[cpu.Registers.StackPointer + 1];
            var psw = memory[cpu.Registers.StackPointer];

            cpu.Registers.Flags.PSW = psw;

            cpu.Registers.StackPointer += 2;
            cpu.Registers.ProgramCounter++;
        }

        // 0xf2 JP adr 
        public static void JP(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.Registers.Flags.Sign);
        }

        // 0xf3
        public static void DI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(false);
            cpu.Registers.ProgramCounter++;
        }

        // 0xf4 CP adr 
        public static void CP(Memory memory, Cpu cpu)
        {
            if (cpu.Registers.Flags.Parity)
                CALL(memory, cpu);
            else
                cpu.Registers.ProgramCounter += 3;
        }

        // 0xf5 , (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
        public static void PUSH_PSW(Memory memory, Cpu cpu)
        {
            memory[cpu.Registers.StackPointer - 1] = cpu.Registers.A;
            memory[cpu.Registers.StackPointer - 2] = cpu.Registers.Flags.PSW;

            cpu.Registers.StackPointer -= 2;
            cpu.Registers.ProgramCounter++;
        }

        // 0xfa JM adr 
        public static void JM(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.Registers.Flags.Sign);
        }

        // 0xfb 
        public static void EI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(true);
            cpu.Registers.ProgramCounter++;
        }

        // 0xfc CM adr 
        public static void CM(Memory memory, Cpu cpu)
        {
            if (cpu.Registers.Flags.Sign)
                CALL(memory, cpu);
            else
                cpu.Registers.ProgramCounter += 3;
        }

        // 0xfe , A - data
        public static void CPI(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.Registers.ProgramCounter + 1];
            var diff = (ushort) (cpu.Registers.A - data);
            cpu.Registers.Flags.CalcSZPC(diff);
            cpu.Registers.Flags.CalcAuxCarryFlag(cpu.Registers.A, (byte)(~data & 0xff), 1);
            cpu.Registers.ProgramCounter += 2;
        }

        // 0xff , CALL $38
        public static void RST_7(Memory memory, Cpu cpu)
        {
            CALL(memory, cpu);
            cpu.Registers.ProgramCounter = 0x38;
        }

    }
}