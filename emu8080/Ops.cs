using System;

namespace emu8080
{

    /// <summary>
    /// http://www.emulator101.com/reference/8080-by-opcode.html
    /// </summary>
    public class Ops
    {
        // 0x00
        public static void NOP(ProgramData programData, State state)
        {
            Console.Write("NOP");
        }

        // 0x01 , B <- byte 3, C <- byte 2
        public static void LXI_B(ProgramData programData, State state)
        {
            state.C = programData[++state.ProgramCounter];
            state.B = programData[++state.ProgramCounter];
        }

        // 0x0f , A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static void RRC(ProgramData programData, State state)
        {
            state.ConditionalFlags.Carry = (state.A & 0x01) == 0x01;
            
            state.A = (byte) ((state.A >> 1) & 0xff);

            if (state.ConditionalFlags.Carry)
                state.A = (byte)(state.A | 0x80);
        }

        // 0x21 , H <- byte 3, L <- byte 2
        public static void LXI_H(ProgramData programData, State state)
        {
            state.L = programData[++state.ProgramCounter];
            state.H = programData[++state.ProgramCounter];
        }

        // 0x2F , A <- !A
        public static void CMA(ProgramData programData, State state)
        {
            state.A = (byte) ~state.A;
        }

        // 0x41 , B <- C
        public static void MOV_B_C(ProgramData programData, State state)
        {
            state.B = state.C;
        }

        // 0x42 , B <- D
        public static void MOV_B_D(ProgramData programData, State state)
        {
            state.B = state.D;
        }

        // 0x42 , B <- E
        public static void MOV_B_E(ProgramData programData, State state)
        {
            state.B = state.E;
        }

        // 0xc2 , if NZ, ProgramCounter <- adr
        public static void JNZ(ProgramData programData, State state)
        {
            if (!state.ConditionalFlags.Zero)
            {
                SetCounterToAddr(programData, state);
            }
            else
            {
                state.ProgramCounter += 2;
            }
        }

        // 0xc3
        public static void JMP(ProgramData programData, State state)
        {
            SetCounterToAddr(programData, state);
        }

        // 0xc9 , PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static void RET(ProgramData programData, State state)
        {
            byte lo = state.Stack[state.StackPointer];
            byte hi = state.Stack[state.StackPointer + 1];
            state.ProgramCounter = NumbersUtils.GetValue(hi, lo);
            state.StackPointer += 2;
        }

        // 0xcd , (StackPointer-1)<-ProgramCounter.hi;(StackPointer-2)<-ProgramCounter.lo;StackPointer<-StackPointer-2;ProgramCounter=adr
        public static void CALL(ProgramData programData, State state)
        {
            var retAddr = state.ProgramCounter + 2;

            state.Stack[state.StackPointer - 1] = retAddr.GetHigh();
            state.Stack[state.StackPointer - 2] = retAddr.GetLow();

            state.StackPointer -= 2;

            SetCounterToAddr(programData, state);
        }
        
        private static void SetCounterToAddr(ProgramData programData, State state)
        {
            var lo = programData[++state.ProgramCounter];
            var hi = programData[++state.ProgramCounter];
            state.ProgramCounter = NumbersUtils.GetValue(hi, lo);
        }
    }
}