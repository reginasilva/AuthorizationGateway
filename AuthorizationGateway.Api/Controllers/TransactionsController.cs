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

        //private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IIntegrityService integrityService)
        {
            _integrityService = integrityService;
        }

        /// <summary>
        /// Processes a transaction request by validating its integrity.
        /// </summary>
        [HttpPost]
        public IActionResult ProcessTransaction([FromBody] TransactionRequest request)
        {
            // Step 1: Validate integrity to avoid cheating.
            var valid = _integrityService.Validate(request.EmvHex, request.Signature);

            if (!valid)
                return BadRequest(new { error = "Invalid signature or tampered data" });

            // Step 2: Placeholder for next step (parsing + decision)
            return Ok(new { status = "IntegrityValidated" });
        }
    }
}
