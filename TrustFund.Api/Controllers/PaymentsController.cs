using Microsoft.AspNetCore.Mvc;
using TrustFund.Services.Contracts;
using TrustFund.Services.Dtos.Payment;
using TrustFund.Services.Exceptions;

namespace TrustFund.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto createPaymentDto)
        {
            try
            {
                var paymentDto = await _paymentService.CreatePaymentAsync(createPaymentDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = paymentDto.Id }, paymentDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid id)
        {
            try
            {
                var paymentDto = await _paymentService.GetPaymentByIdAsync(id);
                return Ok(paymentDto);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("loan/{loanId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByLoanId(Guid loanId)
        {
            var payments = await _paymentService.GetPaymentsByLoanIdAsync(loanId);
            return Ok(payments);
        }
    }
}