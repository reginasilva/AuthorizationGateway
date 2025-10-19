using AuthorizationGateway.Api.Tests.Controllers;
using AuthorizationGateway.Infra.Crypto;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace AuthorizationGateway.Infra.Tests.Crypto
{
    public class AesEncryptionServiceTest
    {
        private ILogger<AesEncryptionService> logger = new NoopLogger<AesEncryptionService>();

        private static (string Key, string Iv) CreateKeyIv()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();

            var keyString = Convert.ToBase64String(aes.Key);
            var ivString = Convert.ToBase64String(aes.IV);

            return (keyString, ivString);
        }

        [Fact]
        public void Encrypt_ThenDecrypt_ReturnsOriginalPlaintext()
        {
            var (key, iv) = CreateKeyIv();
            var svc = new AesEncryptionService(key, iv, logger);

            var plain = "The quick brown fox jumps over the lazy dog";
            var cipher = svc.Encrypt(plain);

            Assert.False(string.IsNullOrWhiteSpace(cipher));
            Assert.NotEqual(plain, cipher);

            var decrypted = svc.Decrypt(cipher);
            Assert.Equal(plain, decrypted);
        }

        [Fact]
        public void Decrypt_ThrowsArgumentNullException_WhenInputIsNull()
        {
            var (key, iv) = CreateKeyIv();
            var svc = new AesEncryptionService(key, iv, logger);

            Assert.Throws<ArgumentNullException>(() => svc.Decrypt(null!));
        }

        [Fact]
        public void Encrypt_ThrowsArgumentNullException_WhenInputIsNull()
        {
            var (key, iv) = CreateKeyIv();
            var svc = new AesEncryptionService(key, iv, logger);

            Assert.Throws<ArgumentNullException>(() => svc.Encrypt(null!));
        }

        [Fact]
        public void Decrypt_ThrowsFormatException_ForInvalidBase64()
        {
            var (key, iv) = CreateKeyIv();
            var svc = new AesEncryptionService(key, iv, logger);

            Assert.Throws<FormatException>(() => svc.Decrypt("not-base64!!!"));
        }
    }
}