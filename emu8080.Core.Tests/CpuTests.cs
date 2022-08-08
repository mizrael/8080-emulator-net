using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace emu8080.Core.Tests
{
    public class CpuTests
    {
        [Fact]
        public void LXI_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x01, 0x78, 0xab
            });

            cpu.Step(memory);

            cpu.Registers.BC.Should().Be(0xab78);
            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void LXI_D()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x11, 0x78, 0xab
            });

            cpu.Step(memory);

            cpu.Registers.DE.Should().Be(0xab78);
            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void LXI_H()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x21, 0x78, 0xab
            });

            cpu.Step(memory);

            cpu.Registers.HL.Should().Be(0xab78);
            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void LXI_SP()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x31, 0x78, 0xab
            });

            cpu.Step(memory);

            cpu.Registers.StackPointer.Should().Be(0xab78);
            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void STAX_B()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.A = 0x71;
            cpu.Registers.BC = 0x2200;

            var memory = Memory.Load(new byte[]
            {
                0x02
            });

            cpu.Step(memory);

            memory[cpu.Registers.BC].Should().Be(0x71);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void STAX_D()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.A = 0x71;
            cpu.Registers.DE = 0x2200;

            var memory = Memory.Load(new byte[]
            {
                0x12
            });

            cpu.Step(memory);

            memory[cpu.Registers.DE].Should().Be(0x71);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void STA()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.A = 0x71;

            var memory = Memory.Load(new byte[]
            {
                0x32, 0x01, 0x20
            });

            cpu.Step(memory);

            memory[0x2001].Should().Be(0x71);

            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void INX_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x03
            });

            cpu.Step(memory);

            cpu.Registers.BC.Should().Be(0x1);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void INX_D()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x13
            });

            cpu.Step(memory);

            cpu.Registers.DE.Should().Be(0x1);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void INX_H()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x23
            });

            cpu.Step(memory);

            cpu.Registers.HL.Should().Be(0x1);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void INR_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x04,
                0x04,
                0x04,
                0x04,
            });

            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x1);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.B = 0x41;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.B = 0xFE;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xFF);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.B = 0xFD;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(4);
        }

        [Fact]
        public void INR_D()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x14,
                0x14,
                0x14,
                0x14,
            });

            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0x1);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.D = 0x41;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.D = 0xFE;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0xFF);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.D = 0xFD;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(4);
        }

        [Fact]
        public void INR_E()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x1c,
                0x1c,
                0x1c,
                0x1c,
            });

            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0x1);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.E = 0x41;
            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.E = 0xFE;
            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0xFF);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.E = 0xFD;
            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(4);
        }

        [Fact]
        public void INR_H()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x24,
                0x24,
                0x24,
                0x24,
            });

            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0x1);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.H = 0x41;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.H = 0xFE;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0xFF);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.H = 0xFD;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(4);
        }

        [Fact]
        public void INR_A()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x3c,
                0x3c,
                0x3c,
                0x3c,
            });

            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x1);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0x41;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.A = 0xFE;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xFF);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.A = 0xFD;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(4);
        }

        [Fact]
        public void DCR_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x05,
                0x05,
                0x05,
            });

            cpu.Registers.B = 0x43;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.B = 0xFF;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void DCR_C()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x0d,
                0x0d,
                0x0d,
            });

            cpu.Registers.C = 0x43;
            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.C = 0xFF;
            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void DCR_D()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x15,
                0x15,
                0x15,
            });

            cpu.Registers.D = 0x43;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.D = 0xFF;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void DCR_H()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x25,
                0x25,
                0x25,
            });

            cpu.Registers.H = 0x43;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.H = 0xFF;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void DCR_M()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x35,
                0x35,
                0x42, 0xFF
            });

            cpu.Registers.HL = 0x02;
            cpu.Step(memory);
            memory[cpu.Registers.HL].Should().Be(0x41);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.HL = 0x03;
            cpu.Step(memory);
            memory[cpu.Registers.HL].Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void DCR_A()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x3d,
                0x3d,
                0x3d,
            });

            cpu.Registers.A = 0x43;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0xFF;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x06, 0x42
            });

            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_C()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x0e, 0x42
            });

            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_E()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x1e, 0x42
            });

            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_H()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x26, 0x42
            });

            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_L()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x2e, 0x42
            });

            cpu.Step(memory);
            cpu.Registers.L.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_M()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.HL = 0x71;

            var memory = Memory.Load(new byte[]
            {
                0x36, 0x42
            });

            cpu.Step(memory);
            memory[cpu.Registers.HL].Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void MVI_A()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x3e, 0x42
            });

            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void RLC()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x07
            });

            cpu.Registers.A = 0xf2;

            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xe5);
            cpu.Registers.Flags.Carry.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void DAD_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x09
            });

            cpu.Registers.B = 0x33;
            cpu.Registers.C = 0x9f;
            cpu.Registers.H = 0xa1;
            cpu.Registers.L = 0x7b;

            cpu.Step(memory);

            cpu.Registers.HL.Should().Be(0xd51a);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void DAD_D()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x19
            });

            cpu.Registers.D = 0x33;
            cpu.Registers.E = 0x9f;
            cpu.Registers.H = 0xa1;
            cpu.Registers.L = 0x7b;

            cpu.Step(memory);

            cpu.Registers.HL.Should().Be(0xd51a);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void DAD_H()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x29
            });

            cpu.Registers.H = 0x12;
            cpu.Registers.L = 0x15;

            cpu.Step(memory);

            cpu.Registers.HL.Should().Be(0x242a);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void LDAX_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x0a,
                0x0, 0x0, 0x0, 0x0, 0x42
            });

            cpu.Registers.BC = 0x5;

            cpu.Step(memory);

            cpu.Registers.A.Should().Be(0x42);

            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void LDAX_D()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x1a,
                0x0, 0x0, 0x0, 0x0, 0x42
            });

            cpu.Registers.DE = 0x5;

            cpu.Step(memory);

            cpu.Registers.A.Should().Be(0x42);

            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void LDA()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x3a,
                0x01, 0x20
            });
            memory[0x2001] = 0x71;

            cpu.Step(memory);

            cpu.Registers.A.Should().Be(0x71);

            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void DCX_B()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x0b
            });

            cpu.Registers.BC = 0x43;

            cpu.Step(memory);

            cpu.Registers.BC.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void MOV_R1_R2()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x40, // MOV_B_B
                0x41, // MOV_B_C
                0x42, // MOV_B_D
                0x43, // MOV_B_E
                0x44, // MOV_B_H
                0x45, // MOV_B_L
                0x46, // MOV_B_M
                0x49, // MOV_C_C
                0x4a, // MOV_C_D
                0x4e, // MOV_C_M
                0x4f, // MOV_C_A
                0x54, // MOV_D_H
                0x56, // MOV_D_M
                0x57, // MOV_D_A
                0x5f, // MOV_E_A
                0x5e, // MOV_E_M
                0x61, // MOV_H_C
                0x66, // MOV_H_M
                0x67, // MOV_H_A
                0x69, // MOV_L_C
                0x6f, // MOV_L_A
                0x77, // MOV_M_A
                0x79, // MOV_A_C
                0x7a, // MOV_A_D
                0x7b, // MOV_A_E
                0x7c, // MOV_A_H
                0x7d, // MOV_A_L
                0x7e, // MOV_A_M
            });

            cpu.Registers.B = 0x42;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x42);
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.C = 0x71;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x71);
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.D = 0xFF;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xFF);
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.E = 0xa2;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xa2);
            cpu.Registers.ProgramCounter.Should().Be(4);

            cpu.Registers.H = 0xb3;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xb3);
            cpu.Registers.ProgramCounter.Should().Be(5);

            cpu.Registers.L = 0xd5;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xd5);
            cpu.Registers.ProgramCounter.Should().Be(6);

            cpu.Registers.HL = 0x2001;
            memory[0x2001] = 0xd2;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xd2);
            cpu.Registers.ProgramCounter.Should().Be(7);

            cpu.Registers.C = 0x22;
            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0x22);
            cpu.Registers.ProgramCounter.Should().Be(8);

            cpu.Registers.D = 0xe8;
            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0xe8);
            cpu.Registers.ProgramCounter.Should().Be(9);

            cpu.Registers.HL = 0x3001;
            memory[0x3001] = 0xf1;
            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0xf1);
            cpu.Registers.ProgramCounter.Should().Be(10);

            cpu.Registers.A = 0x2a;
            cpu.Step(memory);
            cpu.Registers.C.Should().Be(0x2a);
            cpu.Registers.ProgramCounter.Should().Be(11);

            cpu.Registers.H = 0xd4;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0xd4);
            cpu.Registers.ProgramCounter.Should().Be(12);

            cpu.Registers.HL = 0x4001;
            memory[0x4001] = 0xf3;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0xf3);
            cpu.Registers.ProgramCounter.Should().Be(13);

            cpu.Registers.A = 0xe3;
            cpu.Step(memory);
            cpu.Registers.D.Should().Be(0xe3);
            cpu.Registers.ProgramCounter.Should().Be(14);

            cpu.Registers.A = 0xe7;
            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0xe7);
            cpu.Registers.ProgramCounter.Should().Be(15);

            cpu.Registers.HL = 0x5002;
            memory[0x5002] = 0xf1;
            cpu.Step(memory);
            cpu.Registers.E.Should().Be(0xf1);
            cpu.Registers.ProgramCounter.Should().Be(16);

            cpu.Registers.C = 0xe8;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0xe8);
            cpu.Registers.ProgramCounter.Should().Be(17);

            cpu.Registers.HL = 0x6003;
            memory[0x6003] = 0xd1;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0xd1);
            cpu.Registers.ProgramCounter.Should().Be(18);

            cpu.Registers.A = 0xe8;
            cpu.Step(memory);
            cpu.Registers.H.Should().Be(0xe8);
            cpu.Registers.ProgramCounter.Should().Be(19);

            cpu.Registers.C = 0xe9;
            cpu.Step(memory);
            cpu.Registers.L.Should().Be(0xe9);
            cpu.Registers.ProgramCounter.Should().Be(20);

            cpu.Registers.A = 0xb9;
            cpu.Step(memory);
            cpu.Registers.L.Should().Be(0xb9);
            cpu.Registers.ProgramCounter.Should().Be(21);

            cpu.Registers.A = 0xb1;
            cpu.Registers.HL = 0x4042;
            cpu.Step(memory);
            memory[cpu.Registers.HL].Should().Be(0xb1);
            cpu.Registers.ProgramCounter.Should().Be(22);

            cpu.Registers.C = 0xa9;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xa9);
            cpu.Registers.ProgramCounter.Should().Be(23);

            cpu.Registers.D = 0xa2;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xa2);
            cpu.Registers.ProgramCounter.Should().Be(24);

            cpu.Registers.E = 0xa4;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xa4);
            cpu.Registers.ProgramCounter.Should().Be(25);

            cpu.Registers.H = 0xa5;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xa5);
            cpu.Registers.ProgramCounter.Should().Be(26);

            cpu.Registers.L = 0xa6;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xa6);
            cpu.Registers.ProgramCounter.Should().Be(27);

            cpu.Registers.HL = 0x7004;
            memory[0x7004] = 0xd2;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xd2);
            cpu.Registers.ProgramCounter.Should().Be(28);
        }

        [Fact]
        public void ADD()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x80, //ADD_B
                0x81, //ADD_C
                0x87, //ADD_A
            });

            cpu.Registers.A = 0x6c;
            cpu.Registers.B = 0x2e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x9a);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0x03;
            cpu.Registers.C = 0x40;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x43);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.A = 0x04;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x08);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void SUB()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x90, // SUB_B
            });

            cpu.Registers.A = 0x6c;
            cpu.Registers.B = 0x2e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x3e);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void SBB()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x99, 0x99, // SBB_C
                0x9a, 0x9a, // SBB_D
                0x9b, 0x9b, // SBB_E
                0x9c, 0x9c, // SBB_H
                0x9d, 0x9d, // SBB_L
                0x9e, 0x9e, // SBB_M
                0x9f, 0x9f, // SBB_A
            });

            cpu.Registers.A = 0x6c;
            cpu.Registers.C = 0x2e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x3e);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0x6c;
            cpu.Registers.C = 0x2e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x3d);

            cpu.Registers.A = 0x6c;
            cpu.Registers.D = 0x3e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x2e);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.A = 0x6c;
            cpu.Registers.D = 0x3e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x2d);

            cpu.Registers.A = 0x6d;
            cpu.Registers.E = 0x3e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x2f);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(5);

            cpu.Registers.A = 0x6d;
            cpu.Registers.E = 0x3e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x2e);

            cpu.Registers.A = 0x4d;
            cpu.Registers.H = 0x3e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x0f);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(7);

            cpu.Registers.A = 0x4d;
            cpu.Registers.H = 0x3e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x0e);

            cpu.Registers.A = 0x4d;
            cpu.Registers.L = 0x2e;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x1f);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(9);

            cpu.Registers.A = 0x4d;
            cpu.Registers.L = 0x2e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x1e);

            cpu.Registers.A = 0x4d;
            cpu.Registers.HL = 0x202e;
            memory[cpu.Registers.HL] = 0x1d;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x30);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(11);

            cpu.Registers.A = 0xa2;
            cpu.Registers.HL = 0x202f;
            memory[cpu.Registers.HL] = 0x4d;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x54);

            cpu.Registers.A = 0x4d;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(13);

            cpu.Registers.A = 0x4d;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xFF);
            cpu.Registers.Flags.Carry.Should().BeTrue();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeTrue();
        }

        [Fact]
        public void ANA()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xa0, // ANA_B
                0xa7, // ANA_A
            });

            cpu.Registers.A = 0xfc;
            cpu.Registers.B = 0x0f;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x0c);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0xc1;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xc1);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void XRA()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xaa, // XRA_D
                0xaf, // XRA_A
            });

            cpu.Registers.A = 0x5c;
            cpu.Registers.D = 0x78;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x24);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0xc3;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void ORA()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xb0, // ORA_B
                0xb6, // ORA_M
            });

            cpu.Registers.A = 0x33;
            cpu.Registers.B = 0x0f;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x3f);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0x33;
            cpu.Registers.HL = 0x2071;
            memory[cpu.Registers.HL] = 0xe1;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xf3);
            cpu.Registers.Flags.Carry.Should().BeTrue();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void CMP()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xbe, // CMP_M
            });

            cpu.Registers.A = 0x0a;
            cpu.Registers.HL = 0x2042;
            memory[cpu.Registers.HL] = 0x05;
            cpu.Step(memory);

            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void ADC()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0x88, 0x88, // ADC_B
                0x89, 0x89, // ADC_C
                0x8b, 0x8b, // ADC_E
                0x8c, 0x8c, // ADC_H
                0x8d, 0x8d, // ADC_L
            });

            cpu.Registers.A = 0x6c;
            cpu.Registers.B = 0x2e;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x9a);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.A = 0x6c;
            cpu.Registers.B = 0x2e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x9b);
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.A = 0x23;
            cpu.Registers.C = 0x3e;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x61);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.A = 0x23;
            cpu.Registers.C = 0x3e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x62);
            cpu.Registers.ProgramCounter.Should().Be(4);

            cpu.Registers.A = 0x42;
            cpu.Registers.E = 0xa1;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xe3);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(5);

            cpu.Registers.A = 0xa3;
            cpu.Registers.E = 0x3e;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xe2);
            cpu.Registers.ProgramCounter.Should().Be(6);

            cpu.Registers.A = 0x42;
            cpu.Registers.H = 0xb2;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xf4);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(7);

            cpu.Registers.A = 0xa3;
            cpu.Registers.H = 0x21;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xc5);
            cpu.Registers.ProgramCounter.Should().Be(8);

            cpu.Registers.A = 0x42;
            cpu.Registers.L = 0xb5;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xf7);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(9);

            cpu.Registers.A = 0xa3;
            cpu.Registers.L = 0x26;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xca);
            cpu.Registers.ProgramCounter.Should().Be(10);
        }

        [Fact]
        public void RRC()
        {
            Cpu cpu = BuildSut();

            cpu.Registers.A = 0xf2;

            var memory = Memory.Load(new byte[]
            {
                0x0f
            });

            cpu.Step(memory);

            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.A.Should().Be(0x79);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void RAR()
        {
            Cpu cpu = BuildSut();

            cpu.Registers.A = 0x6a;
            cpu.Registers.Flags.Carry = true;

            var memory = Memory.Load(new byte[]
            {
                0x1f
            });

            cpu.Step(memory);

            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.A.Should().Be(0xb5);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void SHLD()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.H = 0x42;
            cpu.Registers.L = 0x71;

            var memory = Memory.Load(new byte[]
            {
                0x22, 0x01, 0x20
            });

            cpu.Step(memory);

            memory[0x2001].Should().Be(0x71);
            memory[0x2002].Should().Be(0x42);

            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void CMA()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.A = 0xFF;

            var memory = Memory.Load(new byte[]
            {
                0x2f
            });

            cpu.Step(memory);

            cpu.Registers.A.Should().Be(0);

            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void CMC()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.Flags.Carry = true;

            var memory = Memory.Load(new byte[]
            {
                0x3f,
                0x3f
            });

            cpu.Step(memory);

            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Step(memory);

            cpu.Registers.Flags.Carry.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void STC()
        {
            Cpu cpu = BuildSut();
            cpu.Registers.Flags.Carry = false;

            var memory = Memory.Load(new byte[]
            {
                0x37
            });

            cpu.Step(memory);

            cpu.Registers.Flags.Carry.Should().BeTrue();

            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void RET()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xc0, 0xc0, // RNZ
                0xc8, // RZ
                0xc9 // RET
            });

            cpu.Registers.Flags.CalcZeroFlag(1);
            cpu.Registers.StackPointer = 0x2001;
            memory[cpu.Registers.StackPointer] = 0x71;
            memory[cpu.Registers.StackPointer + 1] = 0x42;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4271);
            cpu.Registers.StackPointer.Should().Be(0x2003);

            cpu.Registers.Flags.CalcZeroFlag(0);
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4272);
            cpu.Registers.StackPointer.Should().Be(0x2003);

            // RZ
            cpu.Registers.ProgramCounter = 2;
            cpu.Registers.Flags.CalcZeroFlag(0);
            cpu.Registers.StackPointer = 0x2042;
            memory[cpu.Registers.StackPointer] = 0x71;
            memory[cpu.Registers.StackPointer + 1] = 0x31;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x3171);
            cpu.Registers.StackPointer.Should().Be(0x2044);

            cpu.Registers.ProgramCounter = 2;
            cpu.Registers.Flags.CalcZeroFlag(1);
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(3);

            // RET
            cpu.Registers.ProgramCounter = 3;
            cpu.Registers.StackPointer = 0x3071;
            memory[cpu.Registers.StackPointer] = 0x12;
            memory[cpu.Registers.StackPointer + 1] = 0x51;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x5112);
            cpu.Registers.StackPointer.Should().Be(0x3073);

            memory = Memory.Load(new byte[]
            {
                0xd8 // RC
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.Carry = true;
            cpu.Registers.StackPointer = 0x5001;
            memory[cpu.Registers.StackPointer] = 0x71;
            memory[cpu.Registers.StackPointer + 1] = 0x42;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4271);
            cpu.Registers.StackPointer.Should().Be(0x2003);
        }

        [Fact]
        public void POP()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xc1, // POP_BC
            });

            // POP_BC
            cpu.Registers.StackPointer = 0x2001;
            memory[cpu.Registers.StackPointer] = 0x71;
            memory[cpu.Registers.StackPointer + 1] = 0x42;
            cpu.Step(memory);
            cpu.Registers.BC.Should().Be(0x4271);
            cpu.Registers.StackPointer.Should().Be(0x2003);
            cpu.Registers.ProgramCounter.Should().Be(1);

            // POP_DE
            memory = Memory.Load(new byte[]
            {              
                0xd1, // POP_DE
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.StackPointer = 0x2001;
            memory[cpu.Registers.StackPointer] = 0x71;
            memory[cpu.Registers.StackPointer + 1] = 0x42;
            cpu.Step(memory);
            cpu.Registers.DE.Should().Be(0x4271);
            cpu.Registers.StackPointer.Should().Be(0x2003);
            cpu.Registers.ProgramCounter.Should().Be(1);

            // POP_HL
            memory = Memory.Load(new byte[]
            {
                0xe1,
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.StackPointer = 0x4001;
            memory[cpu.Registers.StackPointer] = 0x71;
            memory[cpu.Registers.StackPointer + 1] = 0x42;
            cpu.Step(memory);
            cpu.Registers.HL.Should().Be(0x4271);
            cpu.Registers.StackPointer.Should().Be(0x4003);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void JMP()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xc2, 0x71,0x42, // JNZ
                0xc2,
                0xc3, 0x42,0x71, // JMP
                0xca, 0x71,0x42, // JZ
            });

            cpu.Registers.Flags.CalcZeroFlag(1);
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4271);

            cpu.Registers.Flags.CalcZeroFlag(0);
            cpu.Registers.ProgramCounter = 3;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x06);

            cpu.Registers.ProgramCounter = 4;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x7142);

            cpu.Registers.ProgramCounter = 7;
            cpu.Registers.Flags.CalcZeroFlag(0);
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4271);

            cpu.Registers.ProgramCounter = 7;
            cpu.Registers.Flags.CalcZeroFlag(1);            
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(10);

            memory = Memory.Load(new byte[]
            {
                0xd2, 0x71,0x42, // JNC
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4271);

            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(3);

            // JC
            memory = Memory.Load(new byte[]
            {
                0xda, 0x71,0x42,
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.Carry = true;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x4271);

            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(3);
        }

        [Fact]
        public void CALL()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xc4, 0x42, 0x71, // CNZ
                0xcc, 0x42, 0x71 // CZ
            });

            // CNZ
            cpu.Registers.Flags.CalcZeroFlag(1);
            cpu.Registers.StackPointer = 5;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x7142);
            cpu.Registers.StackPointer.Should().Be(3);
            memory[3].Should().Be(0x03);
            memory[4].Should().Be(0x0);

            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.CalcZeroFlag(0);
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(3);

            // CZ
            memory = Memory.Load(new byte[]
            {                
                0xcc, 0x42, 0x71 // CZ
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.CalcZeroFlag(1);
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.Flags.CalcZeroFlag(0);
            cpu.Registers.StackPointer = 5;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x7142);
            cpu.Registers.StackPointer.Should().Be(3);
            memory[3].Should().Be(0x03);
            memory[4].Should().Be(0x0);

            // CALL
            memory = Memory.Load(new byte[]
            {
                0xcc, 0x42, 0x71 // CZ
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.StackPointer = 5;
            cpu.Step(memory);
            cpu.Registers.ProgramCounter.Should().Be(0x7142);
            cpu.Registers.StackPointer.Should().Be(3);
            memory[3].Should().Be(0x03);
            memory[4].Should().Be(0x0);
        }

        [Fact]
        public void PUSH()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xc5, // PUSH_B
            });

            cpu.Registers.BC = 0x4271;
            cpu.Registers.StackPointer = 2000;
            cpu.Step(memory);
            cpu.Registers.StackPointer.Should().Be(1998);
            memory[cpu.Registers.StackPointer+1].Should().Be(0x42);
            memory[cpu.Registers.StackPointer].Should().Be(0x71);
            cpu.Registers.ProgramCounter.Should().Be(1);

            memory = Memory.Load(new byte[]
            {
                0xd5, // PUSH_D
            });
            cpu.Registers.ProgramCounter = 0;
            cpu.Registers.DE = 0x6244;
            cpu.Registers.StackPointer = 3000;
            cpu.Step(memory);
            cpu.Registers.StackPointer.Should().Be(2998);
            memory[cpu.Registers.StackPointer + 1].Should().Be(0x62);
            memory[cpu.Registers.StackPointer].Should().Be(0x44);
            cpu.Registers.ProgramCounter.Should().Be(1);
        }

        [Fact]
        public void ADI()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xc6, 0x42 // ADI
            });

            cpu.Registers.A = 0x14;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x56);
            cpu.Registers.Flags.Carry.Should().BeFalse();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void ACI()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xce, 0xbe  // ADI
            });

            cpu.Registers.A = 0x56;
            cpu.Registers.Flags.Carry = false;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0x14);
            cpu.Registers.Flags.Carry.Should().BeTrue();
            cpu.Registers.Flags.AuxCarry.Should().BeTrue();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        [Fact]
        public void SUI()
        {
            Cpu cpu = BuildSut();

            var memory = Memory.Load(new byte[]
            {
                0xd6, 0x01 // ADI
            });

            cpu.Registers.A = 0x00;
            cpu.Step(memory);
            cpu.Registers.A.Should().Be(0xff);
            cpu.Registers.Flags.Carry.Should().BeTrue();
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);
        }

        private static Cpu BuildSut()
        {
            var registers = new Registers();
            var bus = new Bus();
            var logger = NSubstitute.Substitute.For<ILogger<Cpu>>();
            var cpu = new Cpu(registers, bus, logger);
            return cpu;
        }
    }
}
