namespace emu8080
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        // 0x00
        public static void NOP(Instructions programData, State state)
        {
            //nopnopnopnopnopnopnopnopnopnop
        }

        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(Instructions programData, State state)
        {
            state.C = programData[++state.ProgramCounter];
            state.B = programData[++state.ProgramCounter];
        }

        // 0x05 , B <- B-1
        public static void DCR_B(Instructions programData, State state)
        {
            state.B = MathUtils.Decrement(state.B, state);
        }

        // 0x06 , B <- byte 2
        public static void MVI_B(Instructions programData, State state)
        {
            state.B = programData[++state.ProgramCounter];
        }

        // 0x09 , HL = HL + BC
        public static void DAD_B(Instructions programData, State state)
        {
            state.DAD(state.BC);
        }

        // 0x0d , C <-C-1
        public static void DCR_C(Instructions programData, State state)
        {
            state.C = MathUtils.Decrement(state.C, state);
        }

        // 0x0e , C <- byte 2
        public static void MVI_C(Instructions programData, State state)
        {
            state.C = programData[++state.ProgramCounter];
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static void RRC(Instructions programData, State state)
        {
            state.ConditionalFlags.CalcCarryFlag(state.A);

            state.A = (byte) ((state.A >> 1) & 0xff);

            if (state.ConditionalFlags.Carry)
                state.A = (byte)(state.A | 0x80);
        }

        // 0x1f , A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0
        public static void RAR(Instructions programData, State state)
        {
            byte temp = (byte)(state.A >> 1);
            
            if (state.ConditionalFlags.Carry)
                temp = (byte)(temp | 0x80);
            
            state.ConditionalFlags.CalcCarryFlag(state.A);
            
            state.A = temp;
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(Instructions programData, State state)
        {
            state.L = programData[++state.ProgramCounter];
            state.H = programData[++state.ProgramCounter];
        }

        // 0x2F , A <- !A
        public static void CMA(Instructions programData, State state)
        {
            state.A = (byte) ~state.A;
        }

        // 0x41 , B <- C
        public static void MOV_B_C(Instructions programData, State state)
        {
            state.B = state.C;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(Instructions programData, State state)
        {
            state.B = state.D;
        }

        // 0x43 , B <- E
        public static void MOV_B_E(Instructions programData, State state)
        {
            state.B = state.E;
        }

        // 0xc1 , C <- (sp); B <- (sp+1); sp <- sp+2
        public static void POP(Instructions programData, State state)
        {
            state.C = state.Stack[state.StackPointer++];
            state.B = state.Stack[state.StackPointer++];
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static void JNZ(Instructions programData, State state)
        {
            if (!state.ConditionalFlags.Zero)
            {
                state.SetCounterToAddr(programData);
            }
            else
            {
                state.ProgramCounter += 2;
            }
        }

        // 0xc3
        public static void JMP(Instructions programData, State state)
        {
            state.SetCounterToAddr(programData);
        }

        // 0xc5 , (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static void PUSH(Instructions programData, State state)
        {
            state.Stack[--state.StackPointer] = state.B;
            state.Stack[--state.StackPointer] = state.C;
        }

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static void RET(Instructions programData, State state)
        {
            byte lo = state.Stack[state.StackPointer];
            byte hi = state.Stack[state.StackPointer + 1];
            state.ProgramCounter = NumbersUtils.GetValue(hi, lo);
            state.StackPointer += 2;
        }

        // 0xcd , (StackPointer-1)<-ProgramCounter.hi;(StackPointer-2)<-ProgramCounter.lo;StackPointer<-StackPointer-2;ProgramCounter=adr
        public static void CALL(Instructions programData, State state)
        {
            var retAddr = state.ProgramCounter + 2;

            state.Stack[state.StackPointer - 1] = retAddr.GetHigh();
            state.Stack[state.StackPointer - 2] = retAddr.GetLow();

            state.StackPointer -= 2;

            state.SetCounterToAddr(programData);
        }

        // 0xf1 , flags <- (sp); A <- (sp+1); sp <- sp+2
        public static void POP_PSW(Instructions programData, State state)
        {
            state.A = state.Stack[state.StackPointer + 1];
            state.ConditionalFlags.Flags = state.Stack[state.StackPointer];
            state.StackPointer += 2;
        }

        // 0xfe , A - data
        public static void CPI(Instructions programData, State state)
        {
            MathUtils.Compare(state.A, state.Stack[state.ProgramCounter + 1], state);
        }

    }
}