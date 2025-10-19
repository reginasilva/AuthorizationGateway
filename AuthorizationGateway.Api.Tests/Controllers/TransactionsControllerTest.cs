using AuthorizationGateway.Api.Controllers;
using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationGateway.Api.Tests.Controllers
{
    public partial class TransactionsControllerTest
    {
        [Fact]
        public void AuthorizeTransaction_ReturnsBadRequest_WhenIntegrityInvalid_Plaintext()
        {
            var encrypt = new FakeEncryptionService("decryptedData");
            var integrity = new FakeIntegrityService(valid: false);
            var txService = new FakeTransactionService { NextResult = null };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(encrypt, integrity, txService, logger);

            var req = new TransactionRequest { EmvHex = "AA", Signature = "sig", PayloadProtection = PayloadProtectionMode.Plaintext };

            var result = controller.AuthorizeTransaction(req);

            Assert.IsType<BadRequestObjectResult>(result);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);

            // Ensure no decryption happened for plaintext mode
            Assert.Null(encrypt.LastCipherText);
            Assert.Equal("AA", integrity.LastValidatedData);
        }

        [Fact]
        public void AuthorizeTransaction_ReturnsOk_WhenIntegrityValid_Plaintext_AndProcessesTransaction()
        {
            var expected = new TransactionResult
            {
                CreatedAtUtc = DateTime.UtcNow,
                MaskedPan = "1234********5678",
                Status = TransactionStatus.Approved
            };

            var encrypt = new FakeEncryptionService("unused");
            var integrity = new FakeIntegrityService(valid: true);
            var txService = new FakeTransactionService { NextResult = expected };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(encrypt, integrity, txService, logger);

            var req = new TransactionRequest { EmvHex = "AA", Signature = "sig", PayloadProtection = PayloadProtectionMode.Plaintext };

            var actionResult = controller.AuthorizeTransaction(req);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var returned = Assert.IsType<TransactionResult>(ok.Value);
            Assert.Same(expected, returned);

            // ensure controller passed the EMV hex to the service
            Assert.Equal("AA", txService.LastEmvHex);
        }

        [Fact]
        public void AuthorizeTransaction_Decrypts_WhenPayloadIsEncrypted_AndPassesDecryptedToIntegrityAndService()
        {
            // Arrange
            var decryptedEmv = "DECRYPTED_HEX";
            var encrypt = new FakeEncryptionService(decryptedEmv);
            var integrity = new FakeIntegrityService(valid: true);
            var txResult = new TransactionResult { CreatedAtUtc = DateTime.UtcNow, MaskedPan = "X", Status = TransactionStatus.Approved };
            var txService = new FakeTransactionService { NextResult = txResult };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(encrypt, integrity, txService, logger);

            var req = new TransactionRequest
            {
                EmvHex = "ENCRYPTED_PAYLOAD_BASE64",
                Signature = "sig",
                PayloadProtection = PayloadProtectionMode.Encrypted
            };

            // Act
            var actionResult = controller.AuthorizeTransaction(req);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var returned = Assert.IsType<TransactionResult>(ok.Value);
            Assert.Same(txResult, returned);

            // Decrypt should have been called with the original EmvHex
            Assert.Equal("ENCRYPTED_PAYLOAD_BASE64", encrypt.LastCipherText);

            // Integrity should have validated the crypted payload
            Assert.Equal("ENCRYPTED_PAYLOAD_BASE64", integrity.LastValidatedData);

            // TransactionService should have received the decrypted EMV hex
            Assert.Equal(decryptedEmv, txService.LastEmvHex);
        }

        [Fact]
        public void AuthorizeTransaction_ReturnsBadRequest_WhenIntegrityInvalid_AndPayloadEncrypted()
        {
            var decryptedEmv = "DECRYPTED_HEX";
            var encrypt = new FakeEncryptionService(decryptedEmv);
            var integrity = new FakeIntegrityService(valid: false);
            var txService = new FakeTransactionService { NextResult = null };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(encrypt, integrity, txService, logger);

            var req = new TransactionRequest
            {
                EmvHex = "ENCRYPTED_PAYLOAD",
                Signature = "sig",
                PayloadProtection = PayloadProtectionMode.Encrypted
            };

            var result = controller.AuthorizeTransaction(req);

            Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("ENCRYPTED_PAYLOAD", integrity.LastValidatedData);
        }

        [Fact]
        public void GetTransaction_ReturnsNotFound_WhenServiceReturnsNull()
        {
            var encrypt = new FakeEncryptionService("decryptedData");
            var integrity = new FakeIntegrityService(valid: true);
            var txService = new FakeTransactionService { NextResult = null };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(encrypt, integrity, txService, logger);

            var actionResult = controller.GetTransaction(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public void GetTransaction_ReturnsOk_WhenServiceReturnsTransaction()
        {
            var tx = new TransactionResult
            {
                CreatedAtUtc = DateTime.UtcNow,
                MaskedPan = "1234********5678",
                Status = TransactionStatus.Approved
            };

            var encrypt = new FakeEncryptionService("decryptedData");
            var integrity = new FakeIntegrityService(valid: true);
            var txService = new FakeTransactionService { NextResult = tx };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(encrypt, integrity, txService, logger);

            var actionResult = controller.GetTransaction(tx.TransactionId);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var returned = Assert.IsType<TransactionResult>(ok.Value);
            Assert.Same(tx, returned);
        }
    }
}