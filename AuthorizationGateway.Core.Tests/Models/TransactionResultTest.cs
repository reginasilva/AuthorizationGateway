using System;
using Xunit;
using AuthorizationGateway.Core.Models;
using AuthorizationGateway.Core.Enums;

namespace AuthorizationGateway.Core.Tests.Models
{
    public class TransactionResultTest
    {
        [Fact]
        public void NewTransactionResult_HasDefaults()
        {
            var before = DateTime.UtcNow;
            var tr = new TransactionResult();
            var after = DateTime.UtcNow;

            Assert.NotEqual(Guid.Empty, tr.TransactionId);
            Assert.Equal(TransactionStatus.Declined, tr.Status);

            // AuthorizedAtUtc is initialized to DateTime.UtcNow at object creation time.
            Assert.InRange(tr.AuthorizedAtUtc, before.AddSeconds(-1), after.AddSeconds(1));

            // CreatedAtUtc not set by default (struct default)
            Assert.Equal(default(DateTime), tr.CreatedAtUtc);

            Assert.Null(tr.MaskedPan);
            Assert.Null(tr.Reason);
        }

        [Fact]
        public void InitProperties_CanBeSet()
        {
            var now = DateTime.UtcNow;
            var tx = new TransactionResult
            {
                TransactionId = Guid.NewGuid(),
                Status = TransactionStatus.Approved,
                MaskedPan = "1234********5678",
                CreatedAtUtc = now,
                Reason = "none"
            };

            Assert.Equal(TransactionStatus.Approved, tx.Status);
            Assert.Equal("1234********5678", tx.MaskedPan);
            Assert.Equal(now, tx.CreatedAtUtc);
            Assert.Equal("none", tx.Reason);
            Assert.NotEqual(Guid.Empty, tx.TransactionId);
        }
    }
}