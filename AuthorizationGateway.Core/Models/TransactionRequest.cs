using AuthorizationGateway.Core.Enums;

namespace AuthorizationGateway.Core.Models
{
    /// <summary>
    /// Represents a transaction request with EMV data and payload protection details.
    /// </summary>
    public class TransactionRequest
    {
        public required string EmvHex { get; set; }
        public PayloadProtectionMode PayloadProtection { get; set; }
        public required string Signature { get; set; }
    }
}
