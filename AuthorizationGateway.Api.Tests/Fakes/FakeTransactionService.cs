using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;

namespace AuthorizationGateway.Api.Tests.Controllers
{
    internal class FakeTransactionService : ITransactionService
    {
        public TransactionResult? NextResult { get; set; }
        public string? LastEmvHex { get; private set; }
        public DateTime LastCreatedAtUtc { get; private set; }

        public TransactionResult Process(string emvHex, DateTime createdAtUtc)
        {
            LastEmvHex = emvHex;
            LastCreatedAtUtc = createdAtUtc;
            return NextResult ?? throw new InvalidOperationException("NextResult not configured");
        }

        public TransactionResult? GetById(Guid id) => NextResult?.TransactionId == id ? NextResult : null;
    }
}
