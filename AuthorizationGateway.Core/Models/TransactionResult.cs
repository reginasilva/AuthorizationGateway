using AuthorizationGateway.Core.Enums;

namespace AuthorizationGateway.Core.Models
{
    public class TransactionResult
    {
        public Guid TransactionId { get; init; } = Guid.NewGuid();

        public TransactionStatus Status { get; init; } = TransactionStatus.Declined;

        public string? MaskedPan { get; init; }

        public DateTime CreatedAtUtc { get; init; }

        public DateTime AuthorizedAtUtc { get; init; } = DateTime.UtcNow;

        public string? Reason { get; init; }
    }
}
