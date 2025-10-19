using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Models;
using AuthorizationGateway.Infra.Persistence;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorizationGateway.Infra.Tests.Persistence
{
    public class InMemoryTransactionRepositoryTest
    {
        private static InMemoryTransactionRepository CreateRepository()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            return new InMemoryTransactionRepository(memoryCache);
        }

        [Fact]
        public void Save_ThenGet_ReturnsSavedTransaction()
        {
            var repo = CreateRepository();
            var id = Guid.NewGuid();
            var tx = new TransactionResult
            {
                TransactionId = id,
                Status = TransactionStatus.Approved,
                MaskedPan = "1234********5678",
                CreatedAtUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                AuthorizedAtUtc = new DateTime(2025, 1, 1, 0, 1, 0, DateTimeKind.Utc),
                Reason = "Test transaction"
            };

            repo.Save(tx);

            var got = repo.Get(id);

            Assert.NotNull(got);
            Assert.Equal(tx.TransactionId, got!.TransactionId);
            Assert.Equal(tx.Status, got.Status);
            Assert.Equal(tx.MaskedPan, got.MaskedPan);
            Assert.Equal(tx.CreatedAtUtc, got.CreatedAtUtc);
            Assert.Equal(tx.AuthorizedAtUtc, got.AuthorizedAtUtc);
            Assert.Equal(tx.Reason, got.Reason);
        }

        [Fact]
        public void Get_ReturnsNull_WhenTransactionNotFound()
        {
            var repo = CreateRepository();
            var missingId = Guid.NewGuid();

            var got = repo.Get(missingId);

            Assert.Null(got);
        }

        [Fact]
        public void Save_OverwritesExistingEntry_WithSameId()
        {
            var repo = CreateRepository();
            var id = Guid.NewGuid();

            var first = new TransactionResult
            {
                TransactionId = id,
                Status = TransactionStatus.Declined,
                MaskedPan = "AAAA",
                CreatedAtUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                AuthorizedAtUtc = new DateTime(2025, 1, 1, 0, 1, 0, DateTimeKind.Utc),
                Reason = "Test transaction"
            };

            var second = new TransactionResult
            {
                TransactionId = id,
                Status = TransactionStatus.Approved,
                MaskedPan = "BBBB",
                CreatedAtUtc = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                AuthorizedAtUtc = new DateTime(2025, 1, 2, 1, 2, 0, DateTimeKind.Utc),
                Reason = "Second test transaction"
            };

            repo.Save(first);
            repo.Save(second);

            var got = repo.Get(id);

            Assert.NotNull(got);
            Assert.Equal(second.TransactionId, got!.TransactionId);
            Assert.Equal(second.Status, got.Status);
            Assert.Equal(second.MaskedPan, got.MaskedPan);
            Assert.Equal(second.CreatedAtUtc, got.CreatedAtUtc);
            Assert.Equal(second.AuthorizedAtUtc, got.AuthorizedAtUtc);
            Assert.Equal(second.Reason, got.Reason);
        }
    }
}
