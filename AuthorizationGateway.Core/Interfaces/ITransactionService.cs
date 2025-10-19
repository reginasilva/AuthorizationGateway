using AuthorizationGateway.Core.Models;

namespace AuthorizationGateway.Core.Interfaces
{
    public interface ITransactionService
    {
        /// <summary>
        /// Processes an EMV transaction represented in hexadecimal format.
        /// </summary>
        TransactionResult Process(string emvHex, DateTime createdAtUtc);

        /// <summary>
        /// Get transaction by its identifier.
        /// </summary>
        TransactionResult? GetById(Guid id);
    }
}
