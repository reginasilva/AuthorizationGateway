using AuthorizationGateway.Core.Interfaces;

namespace AuthorizationGateway.Api.Tests.Controllers
{
    internal class FakeEncryptionService : IEncryptionService
    {
        private readonly string _result;
        public string? LastCipherText { get; private set; }
        public FakeEncryptionService(string result) => _result = result;
        public string Decrypt(string cipherText)
        {
            LastCipherText = cipherText;
            return _result;
        }
        public string Encrypt(string plainText) => _result;
    }
}