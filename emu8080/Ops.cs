namespace emu8080
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        #region Private methods

        private static ushort LXI(Memory memory, State state) {
            var res = Utils.GetValue(memory[state.ProgramCounter+2], memory[state.ProgramCounter+1]);
            state.ProgramCounter += 3;
            return res;
        }

        private static byte MVI(Memory memory, State state) {
            var res = memory[state.ProgramCounter+1];
            state.ProgramCounter += 2;
            return res;
        }

        private static void LDAX(ushort stackIndex, Memory memory, State state)
        {
            state.A = memory[stackIndex];
            state.ProgramCounter++;
        }

        private static void PUSH(ushort val, Memory memory, State state) 
        {
            memory[state.StackPointer-1] = val.GetHigh();
            memory[state.StackPointer-2] = val.GetLow();
            state.StackPointer -= 2;
            state.ProgramCounter += 1;
        }

        private static ushort POP(Memory memory, State state)
        {
            ushort res = Utils.GetValue(memory[state.StackPointer+1], memory[state.StackPointer]);
            state.StackPointer += 2;
            state.ProgramCounter++;
            return res;
        }

        #endregion Private methods

        // 0x00
        public static void NOP(Memory memory, State state)
        {
            //nopnopnopnopnopnopnopnopnopnop
            state.ProgramCounter++;
        }


        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(Memory memory, State state)
        {
            state.BC = LXI(memory, state);
        }

        // 0x03 , BC <- BC+1
        public static void INX_B(Memory memory, State state)
        {
            state.BC++;
            state.ProgramCounter++;
        }

        // 0x05 , B <- B-1
        public static void DCR_B(Memory memory, State state)
        {
            state.B = Utils.Decrement(state.B, state);
            state.ProgramCounter++;
        }

        // 0x06 , B <- byte 2
        public static void MVI_B(Memory memory, State state)
        {
            state.B = MVI(memory, state);
        }

        // 0x09 , HL = HL + BC
        public static void DAD_B(Memory memory, State state)
        {
            state.DAD(state.BC);
        }

        // 0x0d , C <-C-1
        public static void DCR_C(Memory memory, State state)
        {
            state.C = Utils.Decrement(state.C, state);
            state.ProgramCounter++;
        }

        // 0x0e , C <- byte 2
        public static void MVI_C(Memory memory, State state)
        {
            state.C = MVI(memory, state);
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static void RRC(Memory memory, State state)
        {
            state.Flags.CalcCarryFlag(state.A);

            state.A = (byte) ((state.A >> 1) & 0xff);

            if (state.Flags.Carry)
                state.A = (byte)(state.A | 0x80);

            state.ProgramCounter++;
        }

        // 0x11 , D <- byte 3, E <- byte 2
        public static void LXI_D(Memory memory, State state)
        {
            state.DE = LXI(memory, state);
        }

        // 0x13 , DE <- DE + 1
        public static void INX_D(Memory memory, State state)
        {
            state.DE++;
            state.ProgramCounter++;
        }

        // 0x19 , HL = HL + DE
        public static void DAD_D(Memory memory, State state)
        {
            state.DAD(state.DE);
        }

        // 0x1a , A <- (DE)
        public static void LDAX_D(Memory memory, State state)
        {
            LDAX(state.DE, memory, state);
        }

        // 0x1f , A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0
        public static void RAR(Memory memory, State state)
        {
            byte temp = (byte)(state.A >> 1);
            
            if (state.Flags.Carry)
                temp = (byte)(temp | 0x80);
            
            state.Flags.CalcCarryFlag(state.A);
            
            state.A = temp;
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(Memory memory, State state)
        {
            state.HL = LXI(memory, state);
        }

        // 0x23 , HL <- HL + 1
        public static void INX_H(Memory memory, State state)
        {
            state.HL++;
            state.ProgramCounter++;
        }

        // 0x26 , H <- byte 2
        public static void MVI_H(Memory memory, State state)
        {
            state.H = MVI(memory, state);
        }

        // 0x29 , HL = HL + HI
        public static void DAD_H(Memory memory, State state)
        {
            state.DAD(state.HL);
        }
        
        // 0x2F , A <- !A
        public static void CMA(Memory memory, State state)
        {
            state.A = (byte) ~state.A;
        }

        // 0x31 , SP.hi <- byte 3, SP.lo <- byte 2
        public static void LXI_SP(Memory memory, State state)
        {
            state.StackPointer = LXI(memory, state);
        }

        // 0x32 , (adr) <- A
        public static void STA(Memory memory, State state)
        {
            var res = Utils.GetValue(memory[state.ProgramCounter+2], memory[state.ProgramCounter+1]);
            memory[res] = state.A;
            state.ProgramCounter += 3;
        }

        // 0x36 , (HL) <- byte 2
        public static void MVI_M(Memory memory, State state)
        {
            memory[state.HL] = memory[state.ProgramCounter+1];
            state.ProgramCounter += 2;
        }

        // 0x3a , A <- (adr)
        public static void LDA(Memory memory, State state)
        {
            var index = Utils.GetValue(memory[state.ProgramCounter+2], memory[state.ProgramCounter+1]);
            state.A = memory[index];
            state.ProgramCounter += 3;
        }

        // 0x3e , A <- byte 2
        public static void MVI_A(Memory memory, State state)
        {
            state.A = MVI(memory, state);
        }

        // 0x41 , B <- C
        public static void MOV_B_C(Memory memory, State state)
        {
            state.B = state.C;
            state.ProgramCounter++;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(Memory memory, State state)
        {
            state.B = state.D;
            state.ProgramCounter++;
        }

        // 0x43 , B <- E
        public static void MOV_B_E(Memory memory, State state)
        {
            state.B = state.E;
            state.ProgramCounter++;
        }

        // 0x56 , D <- (HL)
        public static void MOV_D_M(Memory memory, State state)
        {
            state.D = memory[state.HL];
            state.ProgramCounter++;
        }


        // 0x5e , E <- (HL)
        public static void MOV_E_M(Memory memory, State state)
        {
            state.E = memory[state.HL];
            state.ProgramCounter++;
        }

        // 0x66 , H <- (HL)
        public static void MOV_H_M(Memory memory, State state)
        {
            state.H = memory[state.HL];
            state.ProgramCounter++;
        }

        // 0x6f , 	L <- (HL)
        public static void MOV_L_A(Memory memory, State state)
        {
            state.L = state.A;
            state.ProgramCounter++;
        }

        // 0x77 , (HL) <- A
        public static void MOV_M_A(Memory memory, State state)
        {
            memory[state.HL] = state.A;
            state.ProgramCounter++;
        }

        // 0x7a , A <- D
        public static void MOV_A_D(Memory memory, State state)
        {
            state.A = state.D;
            state.ProgramCounter++;
        }

        // 0x7a , A <- E
        public static void MOV_A_E(Memory memory, State state)
        {
            state.A = state.E;
            state.ProgramCounter++;
        }


        // 0x7c , A <- H
        public static void MOV_A_H(Memory memory, State state)
        {
            state.A = state.H;
            state.ProgramCounter++;
        }

        // 0x7e , A <- (HL)
        public static void MOV_A_M(Memory memory, State state)
        {
            state.A = memory[state.HL];
            state.ProgramCounter++;
        }

        // 0xa7 , A <- A & A
        public static void ANA_A(Memory memory, State state)
        {
            state.A = (byte)((state.A & state.A) & 0xff);
            state.Flags.CalcSZPC(state.A);
            state.ProgramCounter++;
        }

        // 0xaf , A <- A ^ A
        public static void XRA_A(Memory memory, State state)
        {
            state.A = (byte)((state.A ^ state.A) & 0xff);
            state.Flags.CalcSZPC(state.A);
            state.ProgramCounter++;
        }

        // 0xc0 , if NZ, RET
        public static void RNZ(Memory memory, State state)
        {
            if(!state.Flags.Zero)
                RET(memory, state);
        }

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static void POP_BC(Memory memory, State state)
        {
            state.BC = POP(memory, state);
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static void JNZ(Memory memory, State state)
        {
            if (!state.Flags.Zero)
            {
                state.SetCounterToAddr(memory);
            }
            else
            {
                state.ProgramCounter += 3;
            }
        }

        // 0xc3 , PC <= adr
        public static void JMP(Memory memory, State state)
        {
            state.SetCounterToAddr(memory);
        }

        // 0xc5 , (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static void PUSH_CD(Memory memory, State state)
        {
            PUSH(state.BC, memory, state);
        }

        // 0xc6 , A <- A + byte
        public static void ADI(Memory memory, State state)
        {
            byte data = memory[state.ProgramCounter + 1];
            ushort sum = (ushort) (state.A + data);
            state.A = (byte)(sum & 0xFF);
            state.Flags.CalcSZPC(sum);
            state.ProgramCounter += 2;
        }

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static void RET(Memory memory, State state)
        {
            byte lo = memory[state.StackPointer];
            byte hi = memory[state.StackPointer + 1];
            state.ProgramCounter = (ushort)(Utils.GetValue(hi, lo) + 1);
            state.StackPointer += 2;
        }

        // 0xcd , (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static void CALL(Memory memory, State state)
        {
            var retAddr = state.ProgramCounter + 2;

            memory[state.StackPointer - 1] = retAddr.GetHigh();
            memory[state.StackPointer - 2] = retAddr.GetLow();

            state.StackPointer -= 2;

            state.SetCounterToAddr(memory);
        }

        // 0xd1 , E <- (sp); D <- (sp+1); sp <- sp+2
        public static void POP_DE(Memory memory, State state)
        {
            state.DE = POP(memory, state);
        }

        // 0xd3
        public static void OUT(Memory memory, State state)
        {
            //TODO accumulator is written out to the port
            state.ProgramCounter+=2;
        }

        // 0xd5 , (sp-2)<-E; (sp-1)<-D; sp <- sp - 2
        public static void PUSH_DE(Memory memory, State state)
        {
            PUSH(state.DE, memory, state);
        }

        // 0xe1 , L <- (sp); H <- (sp+1); sp <- sp+2
        public static void POP_HL(Memory memory, State state)
        {
            state.HL = POP(memory, state);
        }

        // 0xe5 , (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
        public static void PUSH_HL(Memory memory, State state)
        {
            PUSH(state.HL, memory, state);
        }

        // 0xe6 , A <- A & data
        public static void ANI(Memory memory, State state)
        {
            var data = memory[state.ProgramCounter + 1];
            state.A = (byte)(state.A & data);
            state.Flags.CalcSZPC(state.A);
            state.ProgramCounter += 2;
        }

        // 0xeb , H <-> D; L <-> E
        public static void XCHG(Memory memory, State state)
        {
            byte t = state.H;
            state.H = state.D;
            state.D = t;

            t = state.L;
            state.L = state.E;
            state.E = t;

            state.ProgramCounter++;
        }

        // 0xf1 , flags <- (sp); A <- (sp+1); sp <- sp+2
        public static void POP_PSW(Memory memory, State state)
        {
            state.A = memory[state.StackPointer + 1];
            var psw = memory[state.StackPointer];

            state.Flags.PSW = psw;

            state.StackPointer += 2;
            state.ProgramCounter++;
        }

        // 0xf5 , (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
        public static void PUSH_PSW(Memory memory, State state)
        {
            memory[state.StackPointer - 1] = state.A;
            memory[state.StackPointer - 2] = state.Flags.PSW;

            state.StackPointer -= 2;
            state.ProgramCounter++;
        }

        // 0xfb 
        public static void EI(Memory memory, State state)
        {
            //TODO enable interrupts
            state.ProgramCounter++;
        }

        // 0xfe , A - data
        public static void CPI(Memory memory, State state)
        {
            var diff = (byte)((state.A - memory[state.ProgramCounter + 1]) & 0xff);
            state.Flags.CalcSZPC(diff);
            state.ProgramCounter += 2;
        }

    }
}