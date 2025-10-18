using AuthorizationGateway.Core.Enums;
using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;
using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Services
{
    public class TransactionService : ITransactionService
    {
        public TransactionResult Process(string emvHex)
        {
            var tags = TlvParser.Parse(emvHex);
            var amount = int.Parse(tags.GetValueOrDefault("9F02", "0"));

            var status = amount <= 1000
                ? TransactionStatus.Approved
                : TransactionStatus.Declined;

            var maskedPan = SensitiveDataMasker.Mask(tags.GetValueOrDefault("5A"));

            return new TransactionResult
            {
                Status = status,
                MaskedPan = maskedPan
            };
        }
    }
}
