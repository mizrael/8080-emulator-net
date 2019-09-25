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

        // 0x03 , BC <- BC+1
        public static void INX_B(Memory memory, Cpu cpu)
        {
            cpu.State.BC++;
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

        // 0x09 , HL = HL + BC
        public static void DAD_B(Memory memory, Cpu cpu)
        {
            cpu.State.DAD(cpu.State.BC);
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
            cpu.State.E = (byte) (cpu.State.E + 1);
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
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(Memory memory, Cpu cpu)
        {
            cpu.State.HL = LXI(memory, cpu);
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
        
        // 0x2F , A <- !A
        public static void CMA(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte) ~cpu.State.A;
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

        // 0x36 , (HL) <- byte 2
        public static void MVI_M(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = memory[cpu.State.ProgramCounter+1];
            cpu.State.ProgramCounter += 2;
        }

        // 0x3a , A <- (adr)
        public static void LDA(Memory memory, Cpu cpu)
        {
            var index = Utils.GetValue(memory[cpu.State.ProgramCounter+2], memory[cpu.State.ProgramCounter+1]);
            cpu.State.A = memory[index];
            cpu.State.ProgramCounter += 3;
        }

        // 0x3e , A <- byte 2
        public static void MVI_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = MVI(memory, cpu);
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

        // 0x46 , B <- (HL)
        public static void MOV_B_M(Memory memory, Cpu cpu)
        {
            cpu.State.B = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x4f , C <- A
        public static void MOV_C_A(Memory memory, Cpu cpu)
        {
            cpu.State.C = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x56 , D <- (HL)
        public static void MOV_D_M(Memory memory, Cpu cpu)
        {
            cpu.State.D = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }


        // 0x5e , E <- (HL)
        public static void MOV_E_M(Memory memory, Cpu cpu)
        {
            cpu.State.E = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x66 , H <- (HL)
        public static void MOV_H_M(Memory memory, Cpu cpu)
        {
            cpu.State.H = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x6f , 	L <- (HL)
        public static void MOV_L_A(Memory memory, Cpu cpu)
        {
            cpu.State.L = cpu.State.A;
            cpu.State.ProgramCounter++;
        }

        // 0x77 , (HL) <- A
        public static void MOV_M_A(Memory memory, Cpu cpu)
        {
            memory[cpu.State.HL] = cpu.State.A;
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

        // 0x7e , A <- (HL)
        public static void MOV_A_M(Memory memory, Cpu cpu)
        {
            cpu.State.A = memory[cpu.State.HL];
            cpu.State.ProgramCounter++;
        }

        // 0x90 , A <- A - B
        public static void SUB_B(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A - cpu.State.B) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xa7 , A <- A & A
        public static void ANA_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A & cpu.State.A) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xaf , A <- A ^ A
        public static void XRA_A(Memory memory, Cpu cpu)
        {
            cpu.State.A = (byte)((cpu.State.A ^ cpu.State.A) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xb0 , A <- A | B
        public static void ORA_B(Memory memory, Cpu cpu)
        {
            cpu.State.B = (byte)((cpu.State.A | cpu.State.B) & 0xff);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter++;
        }

        // 0xc0 , if NZ, RET
        public static void RNZ(Memory memory, Cpu cpu)
        {
            if(!cpu.State.Flags.Zero)
                RET(memory, cpu);
            else
                cpu.State.StackPointer++;
        }

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static void POP_BC(Memory memory, Cpu cpu)
        {
            cpu.State.BC = POP(memory, cpu);
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static void JNZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, !cpu.State.Flags.Zero);
        }

        // 0xc3 , PC <= adr
        public static void JMP(Memory memory, Cpu cpu)
        {
            cpu.State.SetCounterToAddr(memory);
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

        // 0xc6 , A <- A + byte
        public static void ADI(Memory memory, Cpu cpu)
        {
            byte data = memory[cpu.State.ProgramCounter + 1];
            ushort sum = (ushort) (cpu.State.A + data);
            cpu.State.A = (byte)(sum & 0xFF);
            cpu.State.Flags.CalcSZPC(sum);
            cpu.State.ProgramCounter += 2;
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
            cpu.State.ProgramCounter = (ushort)(Utils.GetValue(hi, lo) + 1);
            cpu.State.StackPointer += 2;
        }

        // 0xca , if Z, PC <- adr
        public static void JZ(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.State.Flags.Zero);
        }

        // 0xcd , (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static void CALL(Memory memory, Cpu cpu)
        {
            var retAddr = cpu.State.ProgramCounter + 2;

            memory[cpu.State.StackPointer - 1] = retAddr.GetHigh();
            memory[cpu.State.StackPointer - 2] = retAddr.GetLow();

            cpu.State.StackPointer -= 2;

            cpu.State.SetCounterToAddr(memory);
        }

        // 0xd1 , E <- (sp); D <- (sp+1); sp <- sp+2
        public static void POP_DE(Memory memory, Cpu cpu)
        {
            cpu.State.DE = POP(memory, cpu);
        }

        // 0xd2 , 	if NCY, PC<-adr
        public static void JNC(Memory memory, Cpu cpu)
        {
            JUMP_FLAG(memory, cpu, cpu.State.Flags.Carry);
        }

        // 0xd3
        public static void OUT(Memory memory, Cpu cpu)
        {
            //TODO accumulator is written out to the port
            cpu.State.ProgramCounter+=2;
        }

        // 0xd5 , (sp-2)<-E; (sp-1)<-D; sp <- sp - 2
        public static void PUSH_DE(Memory memory, Cpu cpu)
        {
            PUSH(cpu.State.DE, memory, cpu);
        }

        // 0xe1 , L <- (sp); H <- (sp+1); sp <- sp+2
        public static void POP_HL(Memory memory, Cpu cpu)
        {
            cpu.State.HL = POP(memory, cpu);
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

        // 0xe5 , (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
        public static void PUSH_HL(Memory memory, Cpu cpu)
        {
            PUSH(cpu.State.HL, memory, cpu);
        }

        // 0xe6 , A <- A & data
        public static void ANI(Memory memory, Cpu cpu)
        {
            var data = memory[cpu.State.ProgramCounter + 1];
            cpu.State.A = (byte)(cpu.State.A & data);
            cpu.State.Flags.CalcSZPC(cpu.State.A);
            cpu.State.ProgramCounter += 2;
        }

        // 0xe9 , PC.hi <- H; PC.lo <- L
        public static void PCHL(Memory memory, Cpu cpu)
        {
            cpu.State.ProgramCounter = cpu.State.HL;
        }

        // 0xeb , H <-> D; L <-> E
        public static void XCHG(Memory memory, Cpu cpu)
        {
            byte t = cpu.State.H;
            cpu.State.H = cpu.State.D;
            cpu.State.D = t;

            t = cpu.State.L;
            cpu.State.L = cpu.State.E;
            cpu.State.E = t;

            cpu.State.ProgramCounter++;
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

        // 0xfe
        public static void DI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(false);
            cpu.State.ProgramCounter++;
        }

        // 0xf5 , (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
        public static void PUSH_PSW(Memory memory, Cpu cpu)
        {
            memory[cpu.State.StackPointer - 1] = cpu.State.A;
            memory[cpu.State.StackPointer - 2] = cpu.State.Flags.PSW;

            cpu.State.StackPointer -= 2;
            cpu.State.ProgramCounter++;
        }

        // 0xfb 
        public static void EI(Memory memory, Cpu cpu)
        {
            cpu.Bus.Interrupt(true);
            cpu.State.ProgramCounter++;
        }

        // 0xfe , A - data
        public static void CPI(Memory memory, Cpu cpu)
        {
            var diff = (byte)((cpu.State.A - memory[cpu.State.ProgramCounter + 1]) & 0xff);
            cpu.State.Flags.CalcSZPC(diff);
            cpu.State.ProgramCounter += 2;
        }

        // 0xff , CALL $38
        public static void RST_7(Memory memory, Cpu cpu)
        {
            //TODO
            //CALL();
        }

    }
}