using FluentAssertions;
using Microsoft.Extensions.Logging;

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