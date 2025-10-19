using AuthorizationGateway.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationGateway.Infra.Crypto
{
    /// <summary>
    /// Provides methods for encrypting and decrypting data using the AES (Advanced Encryption Standard) algorithm.
    /// </summary>
    public class AesEncryptionService : IEncryptionService
    {
        /// <summary>
        /// Represents the cryptographic key used for encryption or decryption operations.
        /// </summary>
        private readonly byte[] _key;

        /// <summary>
        /// Represents the initialization vector (IV) used for cryptographic operations.
        /// </summary>
        /// <remarks>
        /// The initialization vector is a fixed-size input used to ensure that encryption
        /// produces unique outputs  even when the same plaintext and key are used.
        /// </remarks>
        private readonly byte[] _iv;

        private readonly ILogger<AesEncryptionService> _logger;

        public AesEncryptionService(string key, string iv, ILogger<AesEncryptionService> logger)
        {
            _key = Convert.FromBase64String(key);
            _iv = Convert.FromBase64String(iv);
            _logger = logger;
        }

        public string Decrypt(string cipherText)
        {
            if (cipherText == null)
            {
                _logger.LogError("Decrypt called with null cipherText");

                throw new ArgumentNullException(nameof(cipherText));
            }

            try
            {
                using var aes = Aes.Create();

                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                var inputBytes = Convert.FromBase64String(cipherText);

                var decrypted = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AES Decrypt.");

                throw;
            }
        }

        public string Encrypt(string plainText)
        {
            if (plainText == null)
            {
                _logger.LogError("Encrypt called with null plainText");

                throw new ArgumentNullException(nameof(plainText));
            }

            try
            {
                using var aes = Aes.Create();

                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                var inputBytes = Encoding.UTF8.GetBytes(plainText);

                var encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AES Encrypt.");

                throw;
            }
        }
    }
}
