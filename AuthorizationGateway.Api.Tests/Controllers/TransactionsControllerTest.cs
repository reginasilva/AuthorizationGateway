using AuthorizationGateway.Api.Controllers;
using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthorizationGateway.Api.Tests.Controllers
{
    public class TransactionsControllerTest
    {
        private class FakeIntegrityService : IIntegrityService
        {
            private readonly bool _valid;
            public FakeIntegrityService(bool valid) => _valid = valid;
            public bool Validate(string data, string signature) => _valid;
        }

        private class FakeTransactionService : ITransactionService
        {
            public TransactionResult? NextResult { get; set; }
            public string? LastEmvHex { get; private set; }
            public DateTime LastCreatedAtUtc { get; private set; }

            public TransactionResult Process(string emvHex, DateTime createdAtUtc)
            {
                LastEmvHex = emvHex;
                LastCreatedAtUtc = createdAtUtc;
                return NextResult ?? throw new InvalidOperationException("NextResult not configured");
            }

            public TransactionResult? GetById(Guid id) => NextResult?.TransactionId == id ? NextResult : null;
        }

        private class NoopLogger<T> : ILogger<T>
        {
            public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
            private class NullScope : IDisposable
            {
                public static NullScope Instance { get; } = new NullScope();
                public void Dispose() { }
            }
        }

        [Fact]
        public void AuthorizeTransaction_ReturnsBadRequest_WhenIntegrityInvalid()
        {
            var integrity = new FakeIntegrityService(valid: false);
            var txService = new FakeTransactionService { NextResult = null };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(integrity, txService, logger);

            var req = new TransactionRequest { EmvHex = "AA", Signature = "sig", PayloadProtection = PayloadProtectionMode.Plaintext };

            var result = controller.AuthorizeTransaction(req);

            Assert.IsType<BadRequestObjectResult>(result);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }

        [Fact]
        public void AuthorizeTransaction_ReturnsOk_WhenIntegrityValid_AndProcessesTransaction()
        {
            var expected = new TransactionResult
            {
                CreatedAtUtc = DateTime.UtcNow,
                MaskedPan = "1234********5678",
                Status = TransactionStatus.Approved
            };

            var integrity = new FakeIntegrityService(valid: true);
            var txService = new FakeTransactionService { NextResult = expected };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(integrity, txService, logger);

            var req = new TransactionRequest { EmvHex = "AA", Signature = "sig", PayloadProtection = PayloadProtectionMode.Plaintext };

            var actionResult = controller.AuthorizeTransaction(req);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var returned = Assert.IsType<TransactionResult>(ok.Value);
            Assert.Same(expected, returned);

            // ensure controller passed the EMV hex to the service
            Assert.Equal("AA", txService.LastEmvHex);
        }

        [Fact]
        public void GetTransaction_ReturnsNotFound_WhenServiceReturnsNull()
        {
            var integrity = new FakeIntegrityService(valid: true);
            var txService = new FakeTransactionService { NextResult = null };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(integrity, txService, logger);

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

            var integrity = new FakeIntegrityService(valid: true);
            var txService = new FakeTransactionService { NextResult = tx };
            var logger = new NoopLogger<TransactionsController>();

            var controller = new TransactionsController(integrity, txService, logger);

            var actionResult = controller.GetTransaction(tx.TransactionId);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var returned = Assert.IsType<TransactionResult>(ok.Value);
            Assert.Same(tx, returned);
        }
    }
}