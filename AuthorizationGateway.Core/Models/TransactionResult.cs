using AuthorizationGateway.Core.Enums;

namespace AuthorizationGateway.Core.Models
{
    public class TransactionResult
    {
        public Guid TransactionId { get; init; }
        public TransactionStatus Status { get; init; }
        public string? MaskedPan { get; init; }
    }
}
