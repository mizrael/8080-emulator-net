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
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeFalse();
            cpu.Registers.ProgramCounter.Should().Be(1);

            cpu.Registers.B = 0x41;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0x42);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeFalse();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(2);

            cpu.Registers.B = 0xFE;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xFF);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
            cpu.Registers.Flags.Zero.Should().BeFalse();
            cpu.Registers.Flags.Sign.Should().BeTrue();
            cpu.Registers.Flags.Parity.Should().BeTrue();
            cpu.Registers.ProgramCounter.Should().Be(3);

            cpu.Registers.B = 0xFD;
            cpu.Step(memory);
            cpu.Registers.B.Should().Be(0xFE);
            cpu.Registers.Flags.AuxCarry.Should().BeFalse();
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