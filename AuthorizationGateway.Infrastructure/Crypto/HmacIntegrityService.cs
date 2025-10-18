using AuthorizationGateway.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationGateway.Infra.Crypto
{
    /// <summary>
    /// Provides functionality to validate data integrity using HMAC SHA-256.
    /// </summary>
    /// <remarks>
    /// This service uses a secret key to compute a hash-based message authentication code (HMAC) for data validation.
    /// </remarks>
    public class HmacIntegrityService: IIntegrityService
    {
        private readonly byte[] _key;

        public HmacIntegrityService(string secretKey)
        {
            _key = Encoding.UTF8.GetBytes(secretKey);
        }

        /// <summary>
        /// Validates if no one has changed the data by comparing the computed HMAC with the provided signature.
        /// </summary>
        /// <returns>True if the computed HMAC is valid.</returns>
        public bool Validate(string data, string signature)
        {
            using var hmac = new HMACSHA256(_key);
            var computed = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
            return computed == signature;
        }
    }
}
