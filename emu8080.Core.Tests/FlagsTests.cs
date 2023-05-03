using FluentAssertions;

namespace emu8080.Core.Tests
{
    public class FlagsTests
    {
        [Theory]
        [InlineData(1, false)]
        [InlineData(0, true)]
        public void CalcZeroFlag(byte val, bool expected)
        {
            var flags = new Flags();
            flags.CalcZeroFlag(val);
            flags.Zero.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(0, true)]
        public void CalcParityFlag(byte val, bool expected)
        {
            var flags = new Flags();
            flags.CalcParityFlag(val);
            flags.Parity.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(0xff, true)]
        public void CalcSignFlag(byte val, bool expected)
        {
            var flags = new Flags();
            flags.CalcSignFlag(val);
            flags.Sign.Should().Be(expected);
        }
    }
}
