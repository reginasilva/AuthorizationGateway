using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;
using AuthorizationGateway.Core.Services;
using AuthorizationGateway.Core.Tests.TestUtils;
using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Tests.Services
{
    public class TransactionServiceTest
    {
        private class FakeRepository : ITransactionRepository
        {
            public TransactionResult? LastSaved { get; private set; }

            public void Save(TransactionResult transaction)
            {
                LastSaved = transaction;
            }

            public TransactionResult? Get(Guid id)
            {
                if (LastSaved is null)
                {
                    return null;
                }

                return LastSaved.TransactionId == id ? LastSaved : null;
            }
        }

        [Fact]
        public void Process_WhenAmountMissing_ReturnsApprovedAndMasksPan_AndSaves()
        {
            var panAsciiHex = "3132333435363738"; // "12345678"
            var tlv = TlvHelper.BuildTlv("5A", panAsciiHex);
            var createdAtUtc = DateTime.UtcNow;

            var repo = new FakeRepository();
            var svc = new TransactionService(repo);
            var result = svc.Process(tlv, createdAtUtc);

            Assert.Equal(TransactionStatus.Approved, result.Status);
            Assert.Equal(SensitiveDataMasker.Mask(panAsciiHex), result.MaskedPan);
            Assert.Equal(createdAtUtc, result.CreatedAtUtc);
            Assert.Same(result, repo.LastSaved);
        }

        [Fact]
        public void Process_WhenAmountExceedsLimit_ReturnsDeclinedWithReason_AndSaves()
        {
            var amountValue = "000000100100";
            var panAsciiHex = "3132333435363738";
            var tracked2 = "1234567890123456";

            var tlv = string.Concat(
                TlvHelper.BuildTlv("9F02", amountValue),
                TlvHelper.BuildTlv("5A", panAsciiHex),
                TlvHelper.BuildTlv("57", tracked2) 
            );

            var createdAtUtc = DateTime.UtcNow;
            var repo = new FakeRepository();
            var svc = new TransactionService(repo);

            var result = svc.Process(tlv, createdAtUtc);

            Assert.Equal(TransactionStatus.Declined, result.Status);
            Assert.Equal("Amount exceeds limit", result.Reason);
            Assert.Equal(SensitiveDataMasker.Mask(panAsciiHex), result.MaskedPan);
            Assert.Equal(SensitiveDataMasker.Mask(tracked2), result.MaskedTrack2);
            Assert.Equal(createdAtUtc, result.CreatedAtUtc);
            Assert.Same(result, repo.LastSaved);
        }

        [Fact]
        public void GetById_DelegatesToRepository()
        {
            var repo = new FakeRepository();

            var preSaved = new TransactionResult
            {
                CreatedAtUtc = DateTime.UtcNow,
                MaskedPan = "1234********5678",
                Status = TransactionStatus.Approved
            };

            repo.Save(preSaved);

            var svc = new TransactionService(repo);

            var got = svc.GetById(preSaved.TransactionId);

            Assert.NotNull(got);
            Assert.Equal(preSaved.TransactionId, got!.TransactionId);
            Assert.Same(preSaved, got);
        }
    }
}
