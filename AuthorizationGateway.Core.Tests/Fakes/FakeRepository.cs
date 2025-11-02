using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;

namespace AuthorizationGateway.Core.Tests.Services
{
    public partial class TransactionServiceTest
    {
        private class FakeRepository : ITransactionRepository
        {

            public List<TransactionResult>? Transactions { get; private set; } = new List<TransactionResult>();
            public TransactionResult? LastSaved { get; private set; }

            public void Save(TransactionResult transaction)
            {
                LastSaved = transaction;
                Transactions.Add(transaction);
            }

            public TransactionResult? Get(Guid id)
            {
                if (LastSaved is null)
                {
                    return null;
                }

                return LastSaved.TransactionId == id ? LastSaved : null;
            }
            public List<TransactionResult> GetAll()
            {
                return Transactions ?? new List<TransactionResult>();
            }
        }
    }
}
