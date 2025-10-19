using AuthorizationGateway.Infra.Crypto;
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationGateway.Infra.Tests.Crypto
{
    public class HmacIntegrityServiceTest
    {
        private static string ComputeSignature(string secretKey, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenSignatureMatches()
        {
            var secretKey = "my-secret-key";
            var data = "important-payload";
            var signature = ComputeSignature(secretKey, data);

            var svc = new HmacIntegrityService(secretKey);

            Assert.True(svc.Validate(data, signature));
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenSignatureDoesNotMatch()
        {
            var secretKey = "my-secret-key";
            var data = "important-payload";
            var badSignature = "invalid-signature";

            var svc = new HmacIntegrityService(secretKey);

            Assert.False(svc.Validate(data, badSignature));
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenDifferentSecretKeyUsedToCreateSignature()
        {
            var serviceKey = "service-key";
            var signerKey = "attacker-key";
            var data = "important-payload";

            // signature computed with a different key than the service uses
            var signatureFromDifferentKey = ComputeSignature(signerKey, data);

            var svc = new HmacIntegrityService(serviceKey);

            Assert.False(svc.Validate(data, signatureFromDifferentKey));
        }
    }
}