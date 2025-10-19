using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Tests.Utils
{
    public class SensitiveDataMaskerTest
    {
        [Fact]
        public void SensitiveDataMasker_Mask_PanNull()
        {
            var masked = SensitiveDataMasker.Mask(null);

            Assert.Null(masked);
        }

        [Fact]
        public void SensitiveDataMasker_Mask_PanEmpty()
        {
            var pan = "";
            var masked = SensitiveDataMasker.Mask(pan);

            Assert.NotNull(masked);
            Assert.Empty(masked);
        }

        [Fact]
        public void SensitiveDataMasker_Mask_PanWhiteSpace()
        {
            var pan = " ";
            var masked = SensitiveDataMasker.Mask(pan);

            Assert.NotNull(masked);
            Assert.Equal(pan, masked);
        }

        [Fact]
        public void SensitiveDataMasker_Mask_LessThanFourCharacters()
        {
            var pan = "123";
            var masked = SensitiveDataMasker.Mask(pan);

            Assert.Equal(pan, masked);
        }

        [Fact]
        public void SensitiveDataMasker_Mask_SevenCharacters()
        {
            var pan = "1234567";
            var masked = SensitiveDataMasker.Mask(pan);

            Assert.Equal(pan, masked);
        }

        [Fact]
        public void SensitiveDataMasker_Mask_PreservesFirstAndLastFour()
        {
            var pan = "1234567812345678";
            var masked = SensitiveDataMasker.Mask(pan);

            Assert.Equal("1234********5678", masked);
        }
    }
}
