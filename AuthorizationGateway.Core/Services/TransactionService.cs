using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;
using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Services
{
    /// <summary>
    /// Provides services for processing and retrieving transaction data.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public TransactionResult Process(string emvHex, DateTime createdAtUtc)
        {
            var tags = TlvParser.Parse(emvHex);
            var amount = int.Parse(tags.GetValueOrDefault("9F02", "0"));

            var status = amount <= 1000
                ? TransactionStatus.Approved
                : TransactionStatus.Declined;

            var maskedPan = SensitiveDataMasker.Mask(tags.GetValueOrDefault("5A"));
            var maskedTrack2 = SensitiveDataMasker.Mask(tags.GetValueOrDefault("57"));

            var result = new TransactionResult
            {
                CreatedAtUtc = createdAtUtc,
                MaskedPan = maskedPan,
                MaskedTrack2 = maskedTrack2,
                Reason = status == TransactionStatus.Declined ? "Amount exceeds limit" : null,
                Status = status,
            };

            _repository.Save(result);

            return result;
        }

        public TransactionResult? GetById(Guid id)
        {
            return _repository.Get(id);
        }

        public List<TransactionResult> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
