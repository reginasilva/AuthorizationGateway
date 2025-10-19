using AuthorizationGateway.Core.Models;

namespace AuthorizationGateway.Core.Interfaces
{
    /// <summary>
    /// Defines methods for processing and retrieving transactions.
    /// </summary>
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

        /// <summary>
        /// Retrieves all transaction results.
        /// </summary>
        /// <returns>
        /// A list of <see cref="TransactionResult"/> objects representing all transactions. <br/>
        /// The list will be empty if no transactions are available.
        /// </returns>
        List<TransactionResult> GetAll();
    }
}
