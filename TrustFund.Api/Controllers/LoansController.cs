using Microsoft.AspNetCore.Mvc;
using TrustFund.Services.Contracts;
using TrustFund.Services.Dtos.Loan;
using TrustFund.Services.Exceptions;

namespace TrustFund.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLoan(CreateLoanDto createLoanDto)
        {
            try
            {
                var loanDto = await _loanService.CreateLoanAsync(createLoanDto);
                return CreatedAtAction(nameof(GetLoanById), new { id = loanDto.Id }, loanDto);
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
        public async Task<ActionResult<LoanDto>> GetLoanById(Guid id)
        {
            try
            {
                var loanDto = await _loanService.GetLoanByIdAsync(id);
                return Ok(loanDto);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoansByUserId(Guid userId)
        {
            var loans = await _loanService.GetLoansByUserIdAsync(userId);
            return Ok(loans);
        }

        [HttpGet("overdue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans()
        {
            var overdueLoans = await _loanService.GetOverdueLoansAsync();
            return Ok(overdueLoans);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLoan(Guid id, UpdateLoanDto updateLoanDto)
        {
            try
            {
                await _loanService.UpdateLoanAsync(id, updateLoanDto);
                return NoContent();
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLoan(Guid id)
        {
            try
            {
                await _loanService.DeleteLoanAsync(id);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }
    }
}