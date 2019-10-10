using System;

namespace emu8080.Core
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        #region Private methods
        
        private static ushort LXI(Memory memory, Cpu cpu) {
            var res = Utils.GetValue(memory[cpu.State.ProgramCounter+2], memory[cpu.State.ProgramCounter+1]);
            cpu.State.ProgramCounter += 3;
            return res;
        }

        private static byte MVI(Memory memory, Cpu cpu) {
            var res = memory[cpu.State.ProgramCounter+1];
            cpu.State.ProgramCounter += 2;
            return res;
        }

        private static void LDAX(ushort stackIndex, Memory memory, Cpu cpu)
        {
            cpu.State.A = memory[stackIndex];
            cpu.State.ProgramCounter++;
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
            WriteMemory(memory, (ushort)(cpu.State.StackPointer - 1), val.GetHigh());
            WriteMemory(memory,(ushort)(cpu.State.StackPointer - 2), val.GetLow());

            cpu.State.StackPointer -= 2;
            cpu.State.ProgramCounter += 1;
        }

        private static ushort POP(Memory memory, Cpu cpu)
        {
            ushort res = Utils.GetValue(memory[cpu.State.StackPointer+1], memory[cpu.State.StackPointer]);
            cpu.State.StackPointer += 2;
            cpu.State.ProgramCounter++;
            return res;
        }

        private static void JUMP_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
                cpu.State.SetCounterToAddr(memory);
            else
                cpu.State.ProgramCounter += 3;
        }
        
        private static void RET_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
                RET(memory, cpu);
            else
                cpu.State.ProgramCounter++;
        }

        private static byte SUM(byte a, byte b, Cpu cpu)
        {
            ushort sum = (ushort)(a + b);
            cpu.State.Flags.CalcSZPC(sum);
            cpu.State.ProgramCounter += 2;

            return (byte)(sum & 0xFF);
        }

        private static byte SUB(Cpu cpu, ushort first, ushort second)
        {
            // Subtract using 2's compliment

            ushort answer = (ushort)(first + (~second & 0xff) + 1);
            cpu.State.Flags.CalcSZPC(answer);

            // On subtraction no carry out sets the carry bit
            // this is opposite from the normal calculation           
            cpu.State.Flags.Carry = !cpu.State.Flags.Carry;
            cpu.State.ProgramCounter++;

            return (byte)(answer & 0xff);
        }
        
        private static byte ADD_With_Carry(Cpu cpu, byte first, byte second)
        {
            var answer = (ushort)(first + second);
            if (cpu.State.Flags.Carry)
                answer++;
            cpu.State.Flags.CalcSZPC(answer);

            cpu.State.ProgramCounter++;

            return (byte)(answer & 0xff);
        }
        
        private static byte XOR(Cpu cpu, ushort first, ushort second)
        {
            ushort result = (ushort)((first ^ second) & 0xff);
            cpu.State.Flags.CalcSZPC(result);
            cpu.State.ProgramCounter++;
            return (byte)result;
        }
        
        private static byte AND(Cpu cpu, ushort first, ushort second)
        {
            ushort result = (ushort)(first & second);
            cpu.State.Flags.CalcSZPC(result);
            cpu.State.Flags.Carry = false;
            cpu.State.Flags.AuxCarry = false;
            cpu.State.ProgramCounter++;
            return (byte)(result & 0xff);
        }

        private static void CALL_FLAG(Memory memory, Cpu cpu, bool flag)
        {
            if (flag)
                CALL(memory, cpu);
            else
                cpu.State.ProgramCounter += 3;
        }

        #endregion Private methods

        // 0x00
        public static void NOP(Memory memory, Cpu cpu)
        {
            //nopnopnopnopnopnopnopnopnopnop
            cpu.State.ProgramCounter++;
        }
        
        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(Memory memory, Cpu cpu)
        {
            cpu.State.BC = LXI(memory, cpu);
        }

        // 0x02 , (BC) <- A
        public static void STAX_B(Memory memory, Cpu cpu)
        {
            WriteMemory(memory, cpu.State.BC, cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x03 , BC <- BC+1
        public static void INX_B(Memory memory, Cpu cpu)
        {
            cpu.State.BC++;
            cpu.State.ProgramCounter++;
        }

        // 0x04 , B <- B+1
        public static void INR_B(Memory memory, Cpu cpu)
        {
            cpu.State.B = Utils.Increment(cpu.State.B, cpu.State);
            cpu.State.Flags.CalcSZPC(cpu.State.D);
            cpu.State.ProgramCounter++;
        }

        // 0x05 , B <- B-1
        public static void DCR_B(Memory memory, Cpu cpu)
        {
            cpu.State.B = Utils.Decrement(cpu.State.B, cpu.State);
            cpu.State.ProgramCounter++;
        }

        // 0x06 , B <- byte 2
        public static void MVI_B(Memory memory, Cpu cpu)
        {
            cpu.State.B = MVI(memory, cpu);
        }

        // 0x07 , 	A = A << 1; bit 0 = prev bit 7; CY = prev bit 7
        public static void RLC(Memory memory, Cpu cpu)
        {
            cpu.State.Flags.Carry = (cpu.State.A & 0x80) == 0x80;

            cpu.State.A = (byte)((cpu.State.A << 1) & 0xff);

            if (cpu.State.Flags.Carry) 
                cpu.State.A = (byte)(cpu.State.A | 0x01);

            cpu.State.ProgramCounter++;
        }

        // 0x09 , HL = HL + BC
        public static void DAD_B(Memory memory, Cpu cpu)
        {
            cpu.State.DAD(cpu.State.BC);
        }

        // 0x0a , 	A <- (BC)
        public static void LDAX_A(Memory memory, Cpu cpu)
        {
            LDAX(cpu.State.BC, memory, cpu);
        }

        // 0x0b , 	BC = BC-1
        public static void DCX_B(Memory memory, Cpu cpu)
        {
            cpu.State.BC = (ushort) (cpu.State.BC - 1); //TODO: check
            cpu.State.ProgramCounter++;
        }

        // 0x0c , C <- C+1
        public static void INR_C(Memory memory, Cpu cpu)
        {
            cpu.State.C = Utils.Increment(cpu.State.C, cpu.State);
            cpu.State.Flags.CalcSZPC(cpu.State.D);
            cpu.State.ProgramCounter++;
        }

        // 0x0d , C <-C-1
        public static void DCR_C(Memory memory, Cpu cpu)
        {
            cpu.State.C = Utils.Decrement(cpu.State.C, cpu.State);
            cpu.State.ProgramCounter++;
        }

        // 0x0e , C <- byte 2
        public static void MVI_C(Memory memory, Cpu cpu)
        {
            cpu.State.C = MVI(memory, cpu);
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static void RRC(Memory memory, Cpu cpu)
        {
            cpu.State.Flags.CalcCarryFlag(cpu.State.A);

            cpu.State.A = (byte) ((cpu.State.A >> 1) & 0xff);

            if (cpu.State.Flags.Carry)
                cpu.State.A = (byte)(cpu.State.A | 0x80);

            cpu.State.ProgramCounter++;
        }

        // 0x11 , D <- byte 3, E <- byte 2
        public static void LXI_D(Memory memory, Cpu cpu)
        {
            cpu.State.DE = LXI(memory, cpu);
        }

        // 0x13 , DE <- DE + 1
        public static void INX_D(Memory memory, Cpu cpu)
        {
            cpu.State.DE++;
            cpu.State.ProgramCounter++;
        }

        // 0x14 , D <- D+1
        public static void INR_D(Memory memory, Cpu cpu)
        {
            cpu.State.D = Utils.Increment(cpu.State.D, cpu.State);
            cpu.State.Flags.CalcSZPC(cpu.State.D);
            cpu.State.ProgramCounter++;
        }

        // 0x15 , D <- D-1
        public static void DCR_D(Memory memory, Cpu cpu)
        {
            cpu.State.D = Utils.Decrement(cpu.State.D, cpu.State);
            cpu.State.ProgramCounter++;
        }

        // 0x16 , D <- byte 2
        public static void MVI_D(Memory memory, Cpu cpu)
        {
            cpu.State.D = MVI(memory, cpu);
        }

        // 0x19 , HL = HL + DE
        public static void DAD_D(Memory memory, Cpu cpu)
        {
            cpu.State.DAD(cpu.State.DE);
        }

        // 0x1a , A <- (DE)
        public static void LDAX_D(Memory memory, Cpu cpu)
        {
            LDAX(cpu.State.DE, memory, cpu);
        }

        // 0x1c , E <-E+1
        public static void INR_E(Memory memory, Cpu cpu)
        {
            cpu.State.E++;
            cpu.State.Flags.CalcSZPC(cpu.State.E);
            cpu.State.ProgramCounter++;
        }

        // 0x1f , A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0
        public static void RAR(Memory memory, Cpu cpu)
        {
            byte temp = (byte)(cpu.State.A >> 1);
            
            if (cpu.State.Flags.Carry)
                temp = (byte)(temp | 0x80);
            
            cpu.State.Flags.CalcCarryFlag(cpu.State.A);
            
            cpu.State.A = temp;

            cpu.State.ProgramCounter++;
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(Memory memory, Cpu cpu)
        {
            cpu.State.HL = LXI(memory, cpu);
        }

        // 0x22 , (adr) <-L; (adr+1)<-H
        public static void SHLD(Memory memory, Cpu cpu)
        {   
            var address = Utils.GetValue(memory[cpu.State.ProgramCounter + 2], memory[cpu.State.ProgramCounter + 1]);
            WriteMemory(memory, address, cpu.State.L);
            WriteMemory(memory, (ushort)(address + 1), cpu.State.H);

            cpu.State.ProgramCounter += 3;
        }

        // 0x23 , HL <- HL + 1
        public static void INX_H(Memory memory, Cpu cpu)
        {
            cpu.State.HL++;
            cpu.State.ProgramCounter++;
        }

        // 0x26 , H <- byte 2
        public static void MVI_H(Memory memory, Cpu cpu)
        {
            cpu.State.H = MVI(memory, cpu);
        }

        // 0x29 , HL = HL + HI
        public static void DAD_H(Memory memory, Cpu cpu)
        {
            cpu.State.DAD(cpu.State.HL);
        }

        // 0x2a , L <- (adr); H<-(adr+1)
        public static void LHLD(Memory memory, Cpu cpu)
        {
            cpu.State.HL = Utils.GetValue(memory[cpu.State.ProgramCounter + 2], memory[cpu.State.ProgramCounter + 1]);

            cpu.State.ProgramCounter += 3;
        }

        // 0x2b , 	HL = HL-1
        public static void DCX_H(Memory memory, Cpu cpu)
        {
            cpu.State.HL = (ushort)(cpu.State.HL - 1); //TODO: check
            cpu.State.ProgramCounter++;
        }

        // 0x2c , L <-L+1
        public static void INR_L(Memory memory, Cpu cpu)
        {
            cpu.State.L++;
            cpu.State.Flags.CalcSZPC(cpu.State.L);
            cpu.State.ProgramCounter++;
        }

        // 0x2d , L <- L-1
        public static void DCR_L(Memory memory, Cpu cpu)
        {
            cpu.State.L = Utils.Decrement(cpu.State.L, cpu.State);
            cpu.State.ProgramCounter++;
        }

        // 0x2e , L <- byte 2
        public static void MVI_L(Memory memory, Cpu cpu)
        {
            cpu.State.L = MVI(memory, cpu);
        }

        // 0x2F , A <- !A
        public static void CMA(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte) ~cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x31 , SP.hi <- byte 3, SP.lo <- byte 2
        public static void LXI_SP(Memory memory, Cpu cpu)
        {
            cpu.State.StackPointer = LXI(memory, cpu);
        }

        // 0x32 , (adr) <- A
        public static void STA(Memory memory, Cpu cpu)
        {
            var res = Utils.GetValue(memory[cpu.State.ProgramCounter+2], memory[cpu.State.ProgramCounter+1]);
            memory[res] = cpu.State.A;
            cpu.State.ProgramCounter += 3;
        }

        // 0x33 , SP = SP + 1
        public static void INX_SP(Memory memory, Cpu cpu)
        {
            cpu.State.StackPointer++;
            cpu.State.ProgramCounter++;
        }

        // 0x34 , (HL) <- (HL)+1
        public static void INR_M(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = (byte)(memory[cpu.State.HL] + 1);
            cpu.State.Flags.CalcSZPC(memory[cpu.State.HL]);
            cpu.State.ProgramCounter++;
        }

        // 0x35 , (HL) <- (HL)-1
        public static void DCR_M(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = (byte) (memory[cpu.State.HL] - 1);
            cpu.State.Flags.CalcSZPC(memory[cpu.State.HL]);
            cpu.State.ProgramCounter++;
        }

        // 0x36 , (HL) <- byte 2
        public static void MVI_M(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = memory[cpu.State.ProgramCounter+1];
            cpu.State.ProgramCounter += 2;
        }

        // 0x37 , CY = 1
        public static void STC(Memory memory, Cpu cpu)
        {
            cpu.State.Flags.Carry = true;
            cpu.State.ProgramCounter++;
        }

        // 0x3a , A <- (adr)
        public static void LDA(Memory memory, Cpu cpu)
        {
            var index = Utils.GetValue(memory[cpu.State.ProgramCounter+2], memory[cpu.State.ProgramCounter+1]);
            cpu.State.A = memory[index];
            cpu.State.ProgramCounter += 3;
        }

        // 0x2b , 	SP = SP-1
        public static void DCX_SP(Memory memory, Cpu cpu)
        {
            cpu.State.StackPointer = (ushort)(cpu.State.StackPointer - 1); 
            cpu.State.ProgramCounter++;
        }

        // 0x3c , A <-A+1
        public static void INR_A(Memory memory, Cpu cpu)
        {
            cpu.State.A++;
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x3d , 	A <- A-1
        public static void DCR_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = Utils.Decrement(cpu.State.A, cpu.State);
            cpu.State.ProgramCounter++;
        }

        // 0x3e , A <- byte 2
        public static void MVI_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = MVI(memory, cpu);
        }

        // 0x40 , B <- C
        public static void MOV_B_B(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x41 , B <- C
        public static void MOV_B_C(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.D;
            cpu.State.ProgramCounter++;
        }

        // 0x43 , B <- E
        public static void MOV_B_E(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.E;
            cpu.State.ProgramCounter++;
        }

        // 0x44 , B <- H
        public static void MOV_B_H(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.H;
            cpu.State.ProgramCounter++;
        }

        // 0x45 , B <- L
        public static void MOV_B_L(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.L;
            cpu.State.ProgramCounter++;
        }

        // 0x46 , B <- (HL)
        public static void MOV_B_M(Memory memory, Cpu cpu)
        {
            cpu.State.B = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x47 , B <- A
        public static void MOV_B_A(Memory memory, Cpu cpu)
        {
            cpu.State.B = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x48 , C <- B
        public static void MOV_C_B(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x49 , C <- C
        public static void MOV_C_C(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x4a , C <- D
        public static void MOV_C_D(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.D;
            cpu.State.ProgramCounter++;
        }

        // 0x4b , C <- E
        public static void MOV_C_E(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.E;
            cpu.State.ProgramCounter++;
        }

        // 0x4C , C <- E
        public static void MOV_C_H(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.H;
            cpu.State.ProgramCounter++;
        }

        // 0x4D , C <- L
        public static void MOV_C_L(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.L;
            cpu.State.ProgramCounter++;
        }

        // 0x4e , C <- (HL)
        public static void MOV_C_M(Memory memory, Cpu cpu)
        {
            cpu.State.C = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }
        
        // 0x4f , C <- A
        public static void MOV_C_A(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x50 , D <- B
        public static void MOV_D_B(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x51 , D <- C
        public static void MOV_D_C(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x52 , D <- D
        public static void MOV_D_D(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.D;
            cpu.State.ProgramCounter++;
        }

        // 0x53 , D <- E
        public static void MOV_D_E(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.E;
            cpu.State.ProgramCounter++;
        }

        // 0x54 , D <- H
        public static void MOV_D_H(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.H;
            cpu.State.ProgramCounter++;
        }

        // 0x55 , D <- L
        public static void MOV_D_L(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.L;
            cpu.State.ProgramCounter++;
        }

        // 0x56 , D <- (HL)
        public static void MOV_D_M(Memory memory, Cpu cpu)
        {
            cpu.State.D = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x57 , D <- A
        public static void MOV_D_A(Memory memory, Cpu cpu)
        {
            cpu.State.D = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x58 , E <- B
        public static void MOV_E_B(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x59 , E <- C
        public static void MOV_E_C(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x5a , E <- D
        public static void MOV_E_D(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.D;
            cpu.State.ProgramCounter++;
        }

        // 0x5b , E <- E
        public static void MOV_E_E(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.E;
            cpu.State.ProgramCounter++;
        }

        // 0x5c , E <- H
        public static void MOV_E_H(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.H;
            cpu.State.ProgramCounter++;
        }

        // 0x5d , E <- L
        public static void MOV_E_L(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.L;
            cpu.State.ProgramCounter++;
        }

        // 0x5e , E <- (HL)
        public static void MOV_E_M(Memory memory, Cpu cpu)
        {
            cpu.State.E = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x5f , E <- A
        public static void MOV_E_A(Memory memory, Cpu cpu)
        {
            cpu.State.E = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x61 , H <- C
        public static void MOV_H_C(Memory memory, Cpu cpu)
        {
            cpu.State.H = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x66 , H <- (HL)
        public static void MOV_H_M(Memory memory, Cpu cpu)
        {
            cpu.State.H = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x67 , H <- A
        public static void MOV_H_A(Memory memory, Cpu cpu)
        {
            cpu.State.H = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x68 , L <- C
        public static void MOV_L_B(Memory memory, Cpu cpu)
        {
            cpu.State.L = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x69 , L <- C
        public static void MOV_L_C(Memory memory, Cpu cpu)
        {
            cpu.State.L = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x6f , 	L <- (HL)
        public static void MOV_L_A(Memory memory, Cpu cpu)
        {
            cpu.State.L = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x70 , (HL) <- B
        public static void MOV_M_B(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x71 , (HL) <- C
        public static void MOV_M_C(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x72 , (HL) <- D
        public static void MOV_M_D(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.D;
            cpu.State.ProgramCounter++;
        }

        // 0x73 , (HL) <- E
        public static void MOV_M_E(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.E;
            cpu.State.ProgramCounter++;
        }

        // 0x74 , (HL) <- H
        public static void MOV_M_H(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.H;
            cpu.State.ProgramCounter++;
        }

        // 0x75 , (HL) <- L
        public static void MOV_M_L(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.L;
            cpu.State.ProgramCounter++;
        }

        // 0x76 
        public static void HLT(Memory memory, Cpu cpu)
        {
            cpu.State.ProgramCounter++;
        }

        // 0x77 , (HL) <- A
        public static void MOV_M_A(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x78 , A <- B
        public static void MOV_A_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = cpu.State.B;
            cpu.State.ProgramCounter++;
        }

        // 0x79 , A <- C
        public static void MOV_A_C(Memory memory, Cpu cpu)
        {
            cpu.State.A = cpu.State.C;
            cpu.State.ProgramCounter++;
        }

        // 0x7a , A <- D
        public static void MOV_A_D(Memory memory, Cpu cpu)
        {
            cpu.State.A = cpu.State.D;
            cpu.State.ProgramCounter++;
        }

        // 0x7a , A <- E
        public static void MOV_A_E(Memory memory, Cpu cpu)
        {
            cpu.State.A = cpu.State.E;
            cpu.State.ProgramCounter++;
        }


        // 0x7c , A <- H
        public static void MOV_A_H(Memory memory, Cpu cpu)
        {
            cpu.State.A = cpu.State.H;
            cpu.State.ProgramCounter++;
        }

        // 0x7d , A <- L
        public static void MOV_A_L(Memory memory, Cpu cpu)
        {
            cpu.State.A = cpu.State.L;
            cpu.State.ProgramCounter++;
        }

        // 0x7e , A <- (HL)
        public static void MOV_A_M(Memory memory, Cpu cpu)
        {
            cpu.State.A = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x80 , A <- A + B
        public static void ADD_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.B) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x81 , A <- A + C
        public static void ADD_C(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.C) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x82 , A <- A + D
        public static void ADD_D(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.D) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x83 , A <- A + E
        public static void ADD_E(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.E) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x84 , A <- A + H
        public static void ADD_H(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.H) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x85 , A <- A + L
        public static void ADD_L(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.L) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x86 , A <- A + M
        public static void ADD_M(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + memory[cpu.State.HL]) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x87 , A <- A + A
        public static void ADD_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.A) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x88 , 	A <- A + B + CY
        public static void ADC_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A + cpu.State.B + (cpu.State.Flags.Carry?1:0)) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0x90 , A <- A - B
        public static void SUB_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = SUB(cpu, cpu.State.A, cpu.State.B);
        }

        // 0x9e , A <- A - (HL) - CY
        public static void SBB_M(Memory memory, Cpu cpu)
        {
            var mem = (ushort)memory[cpu.State.HL];

            ushort res = (ushort) ((ushort)cpu.State.A - mem - (cpu.State.Flags.Carry ? 1 : 0));
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.A = (byte)res;
            
            cpu.State.ProgramCounter++;
        }

        // 0xa0 , A <- A & B
        public static void ANA_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = AND(cpu, cpu.State.A, cpu.State.B);
        }

        // 0xa6 , A <- A & (HL)
        public static void ANA_M(Memory memory, Cpu cpu)
        {
            cpu.State.A = AND(cpu, cpu.State.A, memory[cpu.State.HL]);
        }

        // 0xa7 , A <- A & A
        public static void ANA_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = AND(cpu, cpu.State.A, cpu.State.A);
        }

        // 0xa8 , A <- A ^ B
        public static void XRA_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.B);
        }

        // 0xa9 , A <- A ^ C
        public static void XRA_C(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.C);
        }

        // 0xaa , A <- A ^ D
        public static void XRA_D(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.D);
        }

        // 0xab , A <- A ^ E
        public static void XRA_E(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.E);
        }

        // 0xac , A <- A ^ H
        public static void XRA_H(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.H);
        }

        // 0xad , A <- A ^ L
        public static void XRA_L(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.L);
        }

        // 0xae , A <- A ^ M
        public static void XRA_M(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.State.HL];
            cpu.State.A = XOR(cpu, cpu.State.A, data);
        }

        // 0xaf , A <- A ^ A
        public static void XRA_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = XOR(cpu, cpu.State.A, cpu.State.A);
        }

        // 0xb0 , A <- A | B - ok
        public static void ORA_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.B) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb1 , A <- A | C 
        public static void ORA_C(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.C) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb2 , A <- A | D
        public static void ORA_D(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.D) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb3 , A <- A | E
        public static void ORA_E(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.E) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb4 , A <- A | H
        public static void ORA_H(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.H) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb5 , A <- A | L
        public static void ORA_L(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.L) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb6 , A <- A | (HL)
        public static void ORA_M(Memory memory, Cpu cpu)
        {
            var m = memory[cpu.State.HL];
            cpu.State.A = (byte)((cpu.State.A | m) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb7 , A <- A | A
        public static void ORA_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A | cpu.State.A) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb8 , A - B
        public static void CMP_B(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.B);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xb9 , A - C
        public static void CMP_C(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.C);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xba , A - D
        public static void CMP_D(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.D);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xbb , A - E
        public static void CMP_E(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.E);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xbc , A - H
        public static void CMP_H(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.H);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xbd , A - L
        public static void CMP_L(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.L);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xbe , A - (HL)
        public static void CMP_M(Memory memory, Cpu cpu)
        {
            var mem = (ushort)memory[cpu.State.HL];
            ushort res = (ushort) (cpu.State.A - mem);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xbf , A - A
        public static void CMP_A(Memory memory, Cpu cpu)
        {
            ushort res = (ushort)(cpu.State.A - cpu.State.A);
            cpu.State.Flags.CalcSZPC(res);
            cpu.State.ProgramCounter++;
        }

        // 0xc0 , if NZ, RET
        public static void RNZ(Memory memory, Cpu cpu)
        {
            if(!cpu.State.Flags.Zero)
                RET(memory, cpu);
            else
                cpu.State.ProgramCounter++;
        }

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static void POP_BC(Memory memory, Cpu cpu)
        {
            cpu.State.BC = POP(memory, cpu);
        }

        // 0xc2 , if NZ, ProgramCounter <- adr - ok
        public static void JNZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.State.Flags.Zero);
        }

        // 0xc3 , PC <= adr - ok
        public static void JMP(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, true);
        }

        // 0xc4, if NZ, CALL adr
        public static void CNZ(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, !cpu.State.Flags.Zero);
        }

        // 0xc5 , (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static void PUSH_CD(Memory memory, Cpu cpu)
        {
            PUSH(cpu.State.BC, memory, cpu);
        }

        public static void PUSH_PC(Memory memory, Cpu cpu)
        {
            PUSH(cpu.State.ProgramCounter, memory, cpu);
        }

        // 0xc6 , A <- A + byte - ok
        public static void ADI(Memory memory, Cpu cpu)
        {
            cpu.State.A = SUM(cpu.State.A, memory[cpu.State.ProgramCounter + 1], cpu);
        }

        // 0xc8 , if Z, RET
        public static void RZ(Memory memory, Cpu cpu)
        {
            if (cpu.State.Flags.Zero)
                RET(memory, cpu);
            else
                cpu.State.ProgramCounter++;
        }

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static void RET(Memory memory, Cpu cpu)
        {
            byte lo = memory[cpu.State.StackPointer];
            byte hi = memory[cpu.State.StackPointer + 1];
            cpu.State.ProgramCounter = Utils.GetValue(hi, lo);
            cpu.State.StackPointer += 2;
        }

        // 0xca , if Z, PC <- adr - ok
        public static void JZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.State.Flags.Zero);
        }

        // 0xcc, 	if Z, CALL adr
        public static void CZ(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, cpu.State.Flags.Zero);
        }

        // 0xcd , (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static void CALL(Memory memory, Cpu cpu)
        {
            var retAddr = cpu.State.ProgramCounter + 3;

            memory[cpu.State.StackPointer - 1] = retAddr.GetHigh();
            memory[cpu.State.StackPointer - 2] = retAddr.GetLow();

            cpu.State.StackPointer -= 2;

            cpu.State.SetCounterToAddr(memory);
        }

        // 0xce , A <- A + data + CY
        public static void ACI(Memory memory, Cpu cpu)
        {
            cpu.State.A = ADD_With_Carry(cpu, cpu.State.A, memory[cpu.State.ProgramCounter + 1]);

            // reading from memory, need to increment PC again
            cpu.State.ProgramCounter++;
        }

        // 0xd0 , if NCY, RET
        public static void RNC(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, !cpu.State.Flags.Carry);
        }

        // 0xd1 , E <- (sp); D <- (sp+1); sp <- sp+2
        public static void POP_DE(Memory memory, Cpu cpu)
        {
            cpu.State.DE = POP(memory, cpu);
        }

        // 0xd2 , 	if NCY, PC<-adr - ok
        public static void JNC(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.State.Flags.Carry);
        }

        // 0xd3
        public static void OUT(Memory memory, Cpu cpu)
        {
            //TODO accumulator is written out to the port
            cpu.State.ProgramCounter+=2;
        }

        // 0xd4, if NCY, CALL adr
        public static void CNC(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, !cpu.State.Flags.Carry);
        }

        // 0xd5 , (sp-2)<-E; (sp-1)<-D; sp <- sp - 2 - ok
        public static void PUSH_DE(Memory memory, Cpu cpu)
        {
            PUSH(cpu.State.DE, memory, cpu);
        }

        // 0xd6 , A <- A - data
        public static void SUI(Memory memory, Cpu cpu)
        {
            cpu.State.A = SUB(cpu, cpu.State.A, memory[cpu.State.ProgramCounter + 1]);
            cpu.State.ProgramCounter++; // reading from memory, need to increment PC again
        }

        // 0xd8 , if CY, RET
        public static void RC(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, cpu.State.Flags.Carry);
        }

        // 0xda , if CY, PC<-adr - ok
        public static void JC(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.State.Flags.Carry);
        }

        // 0xdc , if CY, CALL adr
        public static void CC(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, cpu.State.Flags.Carry);
        }

        // 0xdb
        public static void IN_D8(Memory memory, Cpu cpu)
        {
            //TODO ?
            cpu.State.ProgramCounter++;
        }

        // 0xde , A <- A - data - CY
        public static void SBI(Memory memory, Cpu cpu)
        {
            ushort second = memory[cpu.State.ProgramCounter + 1];
            if (cpu.State.Flags.Carry) second++;
            cpu.State.A = SUB(cpu, cpu.State.A, second);
            cpu.State.ProgramCounter++; // reading from memory, need to increment PC again
        }

        // 0xe0 , if PO, RET
        public static void RPO(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, !cpu.State.Flags.Parity);
        }

        // 0xe1 , L <- (sp); H <- (sp+1); sp <- sp+2
        public static void POP_HL(Memory memory, Cpu cpu)
        {
            cpu.State.HL = POP(memory, cpu);
        }

        // 0xe2 , if PO, PC <- adr - ok
        public static void JPO(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.State.Flags.Parity);
        }

        // 0xe3 , L <-> (SP); H <-> (SP+1)
        public static void XTHL(Memory memory, Cpu cpu)
        {
            byte t = cpu.State.L;
            cpu.State.L = memory[cpu.State.StackPointer];
            memory[cpu.State.StackPointer] = t;

            t = cpu.State.H;
            cpu.State.H = memory[cpu.State.StackPointer+1];
            memory[cpu.State.StackPointer+1] = t;

            cpu.State.ProgramCounter++;
        }

        // 0xe4 , if PO, CALL adr
        public static void CPO(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, !cpu.State.Flags.Parity);
        }

        // 0xe5 , (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
        public static void PUSH_HL(Memory memory, Cpu cpu)
        {
            PUSH(cpu.State.HL, memory, cpu);
        }

        // 0xe6 , A <- A & data
        public static void ANI(Memory memory, Cpu cpu)
        {
            var second = memory[cpu.State.ProgramCounter + 1];
            cpu.State.A = AND(cpu, cpu.State.A, second);
            cpu.State.ProgramCounter++; // reading from memory so need to increment PC again
        }

        // 0xe8 , if PE, RET
        public static void RPE(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, cpu.State.Flags.Parity);
        }

        // 0xe9 , PC.hi <- H; PC.lo <- L - ok
        public static void PCHL(Memory memory, Cpu cpu)
        {
            cpu.State.ProgramCounter = cpu.State.HL;
        }

        // 0xea , if PE, PC <- adr
        public static void JPE(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.State.Flags.Parity);
        }

        // 0xeb , H <-> D; L <-> E - ok
        public static void XCHG(Memory memory, Cpu cpu)
        {
            var tmp = cpu.State.DE;
            cpu.State.DE = cpu.State.HL;
            cpu.State.HL = tmp;

            cpu.State.ProgramCounter++;
        }

        // 0xec , 	if PE, CALL adr
        public static void CPE(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, cpu.State.Flags.Parity);
        }

        // 0xee , A <- A ^ data
        public static void XRI(Memory memory, Cpu cpu)
        {
            var second = memory[cpu.State.ProgramCounter];
            cpu.State.A = XOR(cpu, cpu.State.A, second);
            cpu.State.ProgramCounter++;
        }

        // 0xf0 , if P, RET
        public static void RP(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, !cpu.State.Flags.Sign);
        }

        // 0xf1 , flags <- (sp); A <- (sp+1); sp <- sp+2
        public static void POP_PSW(Memory memory, Cpu cpu)
        {
            cpu.State.A = memory[cpu.State.StackPointer + 1];
            var psw = memory[cpu.State.StackPointer];

            cpu.State.Flags.PSW = psw;

            cpu.State.StackPointer += 2;
            cpu.State.ProgramCounter++;
        }

        // 0xf2 , if P=1 PC <- adr - ok
        public static void JP(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.State.Flags.Sign);
        }

        // 0xfa , if M, PC <- adr - ok
        public static void JM(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.State.Flags.Sign);
        }

        // 0xfe
        public static void DI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(false);
            cpu.State.ProgramCounter++;
        }

        // 0xf4, if P, CALL adr
        public static void CP(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, !cpu.State.Flags.Sign);
        }

        // 0xf5 , (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
        public static void PUSH_PSW(Memory memory, Cpu cpu)
        {
            memory[cpu.State.StackPointer - 1] = cpu.State.A;
            memory[cpu.State.StackPointer - 2] = cpu.State.Flags.PSW;

            cpu.State.StackPointer -= 2;
            cpu.State.ProgramCounter++;
        }

        // 0xf6 , A <- A | data - ok
        public static void ORI(Memory memory, Cpu cpu)
        {
            var m = memory[cpu.State.ProgramCounter + 1];
            cpu.State.A = (byte)((cpu.State.A | m) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter+=2;
        }

        // 0xf8 , if M, RET
        public static void RM(Memory memory, Cpu cpu)
        {
            RET_FLAG(memory, cpu, cpu.State.Flags.Sign);
        }

        // 0xfb 
        public static void EI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(true);
            cpu.State.ProgramCounter++;
        }

        // 0xfc, 	if M, CALL adr
        public static void CM(Memory memory, Cpu cpu)
        {
            CALL_FLAG(memory, cpu, cpu.State.Flags.Sign);
        }

        // 0xfe , A - data - ok
        public static void CPI(Memory memory, Cpu cpu)
        {
            SUB(cpu, cpu.State.A, memory[cpu.State.ProgramCounter + 1]);

            // CPI is reading for memory, need to increment PC again
            cpu.State.ProgramCounter++;
        }
        
        // 0xff , CALL $38
        public static void RST_7(Memory memory, Cpu cpu)
        {
            CALL(memory, cpu);
            cpu.State.ProgramCounter = 0x38;
        }

    }
}