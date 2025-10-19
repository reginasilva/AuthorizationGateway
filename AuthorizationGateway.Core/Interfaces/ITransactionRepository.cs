using AuthorizationGateway.Core.Models;

namespace AuthorizationGateway.Core.Interfaces
{
    public interface ITransactionRepository
    {
        /// <summary>
        /// Saves the specified transaction result to the underlying storage.
        /// </summary>
        /// <param name="transaction">The transaction result to save. Cannot be null.</param>
        void Save(TransactionResult transaction);

        /// <summary>
        /// Gets the transaction result by its unique identifier.
        /// </summary>
        TransactionResult? Get(Guid id);
    }
}
