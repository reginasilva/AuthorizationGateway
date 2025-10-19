using AuthorizationGateway.Core.Models;
using System.Collections.Concurrent;

namespace AuthorizationGateway.Infra.Persistence
{
    public class InMemoryTransactionRepository : Core.Interfaces.ITransactionRepository
    {
        private readonly ConcurrentDictionary<Guid, TransactionResult> _transactions = new();

        public void Save(TransactionResult transaction)
        {
            _transactions[transaction.TransactionId] = transaction;
        }

        public TransactionResult? Get(Guid id)
        {
            _transactions.TryGetValue(id, out var transaction);

            return transaction;
        }

        public List<TransactionResult> GetAll()
        {
            return _transactions.Values.ToList();
        }
    }
}
