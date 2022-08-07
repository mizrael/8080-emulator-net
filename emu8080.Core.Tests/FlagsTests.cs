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
    }
}
