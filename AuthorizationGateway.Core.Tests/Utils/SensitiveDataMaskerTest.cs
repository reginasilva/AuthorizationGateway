using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Tests.Utils
{
    public class SensitiveDataMaskerTest
    {
        [Fact]
        public void Mask_ReturnsNull_WhenInputIsNull()
        {
            string? pan = null;
            var masked = SensitiveDataMasker.Mask(pan);
            Assert.Null(masked);
        }

        [Fact]
        public void Mask_ReturnsSame_WhenInputIsWhitespaceOrTooShort()
        {
            Assert.Equal("", SensitiveDataMasker.Mask(""));
            Assert.Equal("   ", SensitiveDataMasker.Mask("   "));

            // shorter than 8 characters should be returned unchanged
            Assert.Equal("1234567", SensitiveDataMasker.Mask("1234567"));
        }

        [Fact]
        public void Mask_PreservesFirstAndLastFour_WithFixedAsterisks()
        {
            var pan = "1234567812345678";
            var masked = SensitiveDataMasker.Mask(pan);

            // Implementation uses fixed 8 asterisks between first 4 and last 4
            Assert.Equal("1234********5678", masked);
        }

        [Fact]
        public void Mask_Works_WhenInputIsExactlyEightChars()
        {
            var pan = "ABCDEFGH";
            var masked = SensitiveDataMasker.Mask(pan);

            // first 4 + 8 asterisks + last 4
            Assert.Equal("ABCD********EFGH", masked);
        }
    }
}