using AuthorizationGateway.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorizationGateway.Infra.Persistence
{
    public class InMemoryTransactionRepository : Core.Interfaces.ITransactionRepository
    {
        private readonly IMemoryCache _cache;

        public InMemoryTransactionRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Save(TransactionResult transaction)
        {
            _cache.Set(transaction.TransactionId, transaction, TimeSpan.FromMinutes(10));
        }

        public TransactionResult? Get(Guid id)
        {
            _cache.TryGetValue(id, out TransactionResult? transaction);

            return transaction;
        }
    }
}
