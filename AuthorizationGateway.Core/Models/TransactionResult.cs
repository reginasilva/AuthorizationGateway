using AuthorizationGateway.Core.Enums;

namespace AuthorizationGateway.Core.Models
{
    /// <summary>
    /// Represents the result of a financial transaction, including its status, identifiers, and related metadata.
    /// </summary>
    public class TransactionResult
    {
        public Guid TransactionId { get; init; } = Guid.NewGuid();

        public TransactionStatus Status { get; init; } = TransactionStatus.Declined;

        public string? MaskedPan { get; init; }

        public string? MaskedTrack2 { get; init; }

        public DateTime CreatedAtUtc { get; init; }

        public DateTime AuthorizedAtUtc { get; init; } = DateTime.UtcNow;

        public string? Reason { get; init; }
    }
}
