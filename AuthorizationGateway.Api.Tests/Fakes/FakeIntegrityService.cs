using AuthorizationGateway.Core.Interfaces;

namespace AuthorizationGateway.Api.Tests.Controllers
{
    internal class FakeIntegrityService : IIntegrityService
    {
        private readonly bool _valid;
        public string? LastValidatedData { get; private set; }
        public string? LastValidatedSignature { get; private set; }
        public FakeIntegrityService(bool valid) => _valid = valid;
        public bool Validate(string data, string signature)
        {
            LastValidatedData = data;
            LastValidatedSignature = signature;
            return _valid;
        }
    }
}