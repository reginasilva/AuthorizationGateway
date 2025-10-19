using AuthorizationGateway.Core.Models;

namespace AuthorizationGateway.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for managing transaction results in a persistent storage.
    /// </summary>
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
