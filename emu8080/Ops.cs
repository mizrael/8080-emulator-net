namespace emu8080
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        // 0x00
        public static void NOP(ProgramInstructions programData, State state)
        {
            //nopnopnopnopnopnopnopnopnopnop
            state.ProgramCounter++;
        }
        
        private static ushort LXI(ProgramInstructions programData, State state) {
            var res = NumbersUtils.GetValue(programData[state.ProgramCounter+2], programData[state.ProgramCounter+1]);
            state.ProgramCounter += 3;
            return res;
        }

        private static byte MVI(ProgramInstructions programData, State state) {
            var res = programData[state.ProgramCounter+1];
            state.ProgramCounter += 2;
            return res;
        }

        private static void LDAX(ushort stackIndex, ProgramInstructions programData, State state)
        {
            state.A = programData[stackIndex];
            state.ProgramCounter++;
        }

        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(ProgramInstructions programData, State state)
        {
            state.BC = LXI(programData, state);
        }

        // 0x05 , B <- B-1
        public static void DCR_B(ProgramInstructions programData, State state)
        {
            state.B = MathUtils.Decrement(state.B, state);
        }

        // 0x06 , B <- byte 2
        public static void MVI_B(ProgramInstructions programData, State state)
        {
            state.B = MVI(programData, state);
        }

        // 0x09 , HL = HL + BC
        public static void DAD_B(ProgramInstructions programData, State state)
        {
            state.DAD(state.BC);
        }

        // 0x0d , C <-C-1
        public static void DCR_C(ProgramInstructions programData, State state)
        {
            state.C = MathUtils.Decrement(state.C, state);
        }

        // 0x0e , C <- byte 2
        public static void MVI_C(ProgramInstructions programData, State state)
        {
            state.C = MVI(programData, state);
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static void RRC(ProgramInstructions programData, State state)
        {
            state.Flags.CalcCarryFlag(state.A);

            state.A = (byte) ((state.A >> 1) & 0xff);

            if (state.Flags.Carry)
                state.A = (byte)(state.A | 0x80);
        }

        // 0x11 , D <- byte 3, E <- byte 2
        public static void LXI_D(ProgramInstructions programData, State state)
        {
            state.DE = LXI(programData, state);
        }

        // 0x13 , DE <- DE + 1
        public static void INX_D(ProgramInstructions programData, State state)
        {
            state.DE++;
        }

        // 0x19 , HL = HL + DE
        public static void DAD_D(ProgramInstructions programData, State state)
        {
            state.DAD(state.DE);
        }

        // 0x1a , A <- (DE)
        public static void LDAX_D(ProgramInstructions programData, State state)
        {
            LDAX(state.DE, programData, state);
        }

        // 0x1f , A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0
        public static void RAR(ProgramInstructions programData, State state)
        {
            byte temp = (byte)(state.A >> 1);
            
            if (state.Flags.Carry)
                temp = (byte)(temp | 0x80);
            
            state.Flags.CalcCarryFlag(state.A);
            
            state.A = temp;
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(ProgramInstructions programData, State state)
        {
            state.HL = LXI(programData, state);
        }

        // 0x23 , HL <- HL + 1
        public static void INX_H(ProgramInstructions programData, State state)
        {
            state.HL++;
        }

        // 0x26 , H <- byte 2
        public static void MVI_H(ProgramInstructions programData, State state)
        {
            state.H = MVI(programData, state);
        }

        // 0x29 , HL = HL + HI
        public static void DAD_H(ProgramInstructions programData, State state)
        {
            state.DAD(state.HL);
        }
        
        // 0x2F , A <- !A
        public static void CMA(ProgramInstructions programData, State state)
        {
            state.A = (byte) ~state.A;
        }

        // 0x31 , SP.hi <- byte 3, SP.lo <- byte 2
        public static void LXI_SP(ProgramInstructions programData, State state)
        {
            state.StackPointer = LXI(programData, state);
        }

        // 0x41 , B <- C
        public static void MOV_B_C(ProgramInstructions programData, State state)
        {
            state.B = state.C;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(ProgramInstructions programData, State state)
        {
            state.B = state.D;
        }

        // 0x43 , B <- E
        public static void MOV_B_E(ProgramInstructions programData, State state)
        {
            state.B = state.E;
        }

        // 0xc0 , if NZ, RET
        public static void RNZ(ProgramInstructions programData, State state)
        {
            if(!state.Flags.Zero)
                RET(programData, state);
        }

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static void POP(ProgramInstructions programData, State state)
        {
            state.C = programData[state.StackPointer+1];
            state.B = programData[state.StackPointer+2];
            state.StackPointer += 2;
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static void JNZ(ProgramInstructions programData, State state)
        {
            if (!state.Flags.Zero)
            {
                state.SetCounterToAddr(programData);
            }
            else
            {
                state.ProgramCounter += 2;
            }
        }

        // 0xc3 , PC <= adr
        public static void JMP(ProgramInstructions programData, State state)
        {
            state.SetCounterToAddr(programData);
        }

        // 0xc5 , (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static void PUSH(ProgramInstructions programData, State state)
        {
            programData[state.StackPointer-1] = state.B;
            programData[state.StackPointer-2] = state.C;
            state.StackPointer -= 2;
        }

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static void RET(ProgramInstructions programData, State state)
        {
            byte lo = programData[state.StackPointer];
            byte hi = programData[state.StackPointer + 1];
            state.ProgramCounter = NumbersUtils.GetValue(hi, lo);
            state.StackPointer += 2;
        }

        // 0xcd , (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static void CALL(ProgramInstructions programData, State state)
        {
            var retAddr = state.ProgramCounter + 2;

            programData[state.StackPointer - 1] = retAddr.GetHigh();
            programData[state.StackPointer - 2] = retAddr.GetLow();

            state.StackPointer -= 2;

            state.SetCounterToAddr(programData);
        }

        // 0xf1 , flags <- (sp); A <- (sp+1); sp <- sp+2
        public static void POP_PSW(ProgramInstructions programData, State state)
        {
            state.A = programData[state.StackPointer + 1];
            var psw = programData[state.StackPointer];

            state.Flags.PSW(psw);

            state.StackPointer += 2;
        }

        // 0xfe , A - data
        public static void CPI(ProgramInstructions programData, State state)
        {
            MathUtils.Compare(state.A, programData[state.ProgramCounter + 1], state);
        }

    }
}