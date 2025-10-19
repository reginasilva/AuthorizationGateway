using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Models;
using AuthorizationGateway.Core.Services;
using AuthorizationGateway.Core.Tests.TestUtils;
using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Tests.Services
{
    public partial class TransactionServiceTest
    {

        [Fact]
        public void Process_WhenAmountMissing_ReturnsApprovedAndMasksPan_AndSaves()
        {
            var panAsciiHex = "3132333435363738";
            var createdAtUtc = DateTime.UtcNow;

            var tlv = TlvHelper.BuildTlv("5A", panAsciiHex);

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
        public void Process_SavesMaskedSensitiveFields_BeforePersisting()
        {
            // Arrange
            var amountValue = "1001"; // > 1000 => Declined
            var panAsciiHex = "3132333435363738"; // "12345678"
            var track2 = "1234567890123456";

            var tlv = string.Concat(
                TlvHelper.BuildTlv("9F02", amountValue),
                TlvHelper.BuildTlv("5A", panAsciiHex),
                TlvHelper.BuildTlv("57", track2)
            );

            var repo = new FakeRepository();
            var svc = new TransactionService(repo);
            var createdAtUtc = DateTime.UtcNow;

            // Act
            var result = svc.Process(tlv, createdAtUtc);

            // Assert result basic expectations
            Assert.Equal(TransactionStatus.Declined, result.Status);
            Assert.Equal("Amount exceeds limit", result.Reason);
            Assert.Equal(createdAtUtc, result.CreatedAtUtc);

            // Repository must have been called
            Assert.Same(result, repo.LastSaved);
            Assert.NotNull(repo.LastSaved);

            // Masked PAN should not equal raw and should match masker output
            var expectedMaskedPan = SensitiveDataMasker.Mask(panAsciiHex);
            Assert.Equal(expectedMaskedPan, repo.LastSaved!.MaskedPan);
            Assert.NotEqual(panAsciiHex, repo.LastSaved.MaskedPan);

            var expectedMaskedTrack2 = SensitiveDataMasker.Mask(track2);
            Assert.Equal(expectedMaskedTrack2, repo.LastSaved.MaskedTrack2);
            Assert.NotEqual(track2, repo.LastSaved.MaskedTrack2);
        }

        [Fact]
        public void Process_WithMissingPan_LeavesMaskedPanNull_AndSaves()
        {
            // Arrange: TLV contains amount only
            var amountValue = "10";
            var tlv = TlvHelper.BuildTlv("9F02", amountValue);

            var repo = new FakeRepository();
            var svc = new TransactionService(repo);
            var createdAtUtc = DateTime.UtcNow;

            // Act
            var result = svc.Process(tlv, createdAtUtc);

            // Assert
            Assert.Equal(TransactionStatus.Approved, result.Status);
            Assert.Equal(createdAtUtc, result.CreatedAtUtc);
            Assert.Same(result, repo.LastSaved);

            Assert.Null(result.MaskedPan);
            Assert.Null(result.MaskedTrack2);
        }

        [Fact]
        public void Process_Throws_FormatException_ForNonNumericAmount()
        {
            var tlv = string.Concat(
                TlvHelper.BuildTlv("9F02", "ZZZZ"),
                TlvHelper.BuildTlv("5A", "3132333435363738")
            );

            var repo = new FakeRepository();
            var svc = new TransactionService(repo);

            Assert.Throws<FormatException>(() => svc.Process(tlv, DateTime.UtcNow));
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
