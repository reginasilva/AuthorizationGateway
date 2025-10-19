using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationGateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IIntegrityService _integrityService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IIntegrityService integrityService,
                                      ITransactionService transactionService,
                                      ILogger<TransactionsController> logger)
        {
            _integrityService = integrityService;
            _transactionService = transactionService;
            _logger = logger;
        }

        /// <summary>
        /// Authorizes a transaction by validating its integrity and ensuring the request data has not been tampered with.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the authorization process.  <br/>
        /// - Returns a 200 OK response with a status of "IntegrityValidated" if the request passes validation <br/>
        /// - Returns 400 Bad Request response with an error message if the validation fails.
        /// </returns>
        [HttpPost("authorize")]
        public IActionResult AuthorizeTransaction([FromBody] TransactionRequest request)
        {
            var createdAtUtc = DateTime.UtcNow;

            var valid = _integrityService.Validate(request.EmvHex, request.Signature);

            if (!valid)
            {
                _logger.LogWarning("Integrity validation failed for request received at {CreateAt} with signature {Signature}", createdAtUtc, request.Signature);

                return BadRequest(new { error = "Invalid signature or tampered data" });
            }

            var result = _transactionService.Process(request.EmvHex, createdAtUtc);

            _logger.LogInformation("Transaction {Id} created at {CreatedAt} authorized at {AuthorizedAt} with status {Status}",
                                    result.TransactionId, result.CreatedAtUtc, result.AuthorizedAtUtc, result.Status);

            return Ok(result);
        }

        /// <summary>
        /// Gets the transaction info by its unique identifier.
        /// </summary>
        [HttpGet("{id:guid}")]
        public IActionResult GetTransaction(Guid id)
        {
            var transaction = _transactionService.GetById(id);
           
            if (transaction == null)
            {
                _logger.LogWarning("Transaction {Id} not found", id);

                return NotFound(new { error = "Transaction not found" });
            }

            _logger.LogInformation("Transaction {Id} retrieved with status {Status}", id, transaction.Status);

            return Ok(transaction);
        }
    }
}
