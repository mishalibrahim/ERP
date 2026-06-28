using ERP.Features.GeneralLedger.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP.Features.GeneralLedger
{
    [ApiController]
    [Route("api/general-ledger")]
    [Authorize]
    public class GeneralLedgerController : ControllerBase
    {
        private readonly IGeneralLedgerService _glService;

        public GeneralLedgerController(IGeneralLedgerService glService)
        {
            _glService = glService;
        }

        // --- Existing Endpoints ---

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] GlLedgerFilterParams filters)
        {
            var result = await _glService.GetTransactionsAsync(filters);
            return Ok(result);
        }

        [HttpGet("opening-balance")]
        public async Task<IActionResult> GetOpeningBalance([FromQuery] Guid? accountId, [FromQuery] string? periodValue)
        {
            var result = await _glService.GetOpeningBalanceAsync(accountId, periodValue);
            return Ok(result);
        }

        [HttpGet("closing-balance")]
        public async Task<IActionResult> GetClosingBalance([FromQuery] Guid? accountId, [FromQuery] string? periodValue)
        {
            var result = await _glService.GetClosingBalanceAsync(accountId, periodValue);
            return Ok(result);
        }

        [HttpGet("periods")]
        public async Task<IActionResult> GetPeriods()
        {
            var result = await _glService.GetPeriodsAsync();
            return Ok(result);
        }

        [HttpGet("cost-centers")]
        public async Task<IActionResult> GetCostCenters()
        {
            var result = await _glService.GetCostCentersAsync();
            return Ok(result);
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var result = await _glService.GetAccountsAsync();
            return Ok(result);
        }

        // --- Trial Balance ---

        [HttpGet("trial-balance")]
        public async Task<IActionResult> GetTrialBalance([FromQuery] string? periodValue)
        {
            var result = await _glService.GetTrialBalanceAsync(periodValue);
            return Ok(result);
        }

        // --- CSV Export ---

        [HttpGet("export")]
        public async Task<IActionResult> ExportCsv([FromQuery] GlLedgerFilterParams filters)
        {
            var bytes = await _glService.ExportTransactionsCsvAsync(filters);
            var fileName = $"GL_Ledger_{DateTime.UtcNow:yyyyMMdd}.csv";
            return File(bytes, "text/csv; charset=utf-8", fileName);
        }

        // --- Period Locking ---

        [HttpGet("period-locks")]
        public async Task<IActionResult> GetPeriodLocks()
        {
            var result = await _glService.GetPeriodLocksAsync();
            return Ok(result);
        }

        [HttpPut("period-locks/{periodValue}/lock")]
        public async Task<IActionResult> LockPeriod(string periodValue)
        {
            try
            {
                var result = await _glService.SetPeriodLockAsync(periodValue, true);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("period-locks/{periodValue}/unlock")]
        public async Task<IActionResult> UnlockPeriod(string periodValue)
        {
            try
            {
                var result = await _glService.SetPeriodLockAsync(periodValue, false);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
