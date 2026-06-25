using ERP.Features.CompanySetup.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.CompanySetup
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanySetupController : ControllerBase
    {
        private readonly ICompanySetupService _companySetupService;

        public CompanySetupController(ICompanySetupService companySetupService)
        {
            _companySetupService = companySetupService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CompanyListItemDto>>> GetAll()
        {
            var tenants = await _companySetupService.GetAllAsync();
            return Ok(tenants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDetailsDto>> GetById(Guid id)
        {
            var tenant = await _companySetupService.GetByIdAsync(id);
            if (tenant == null)
            {
                return NotFound("Company not found or access denied.");
            }
            return Ok(tenant);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDraft([FromBody] CreateCompanyDto dto)
        {
            var result = await _companySetupService.CreateDraftAsync(dto);
            return Ok(new { id = result.Id, message = "Company draft created successfully.", rowVersion = Convert.ToBase64String(result.RowVersion) });
        }

        [HttpPut("{id}/general")]
        public async Task<IActionResult> UpdateGeneral(Guid id, [FromBody] UpdateCompanyGeneralDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateGeneralInfoAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "General info updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/financials")]
        public async Task<IActionResult> UpdateFinancials(Guid id, [FromBody] UpdateCompanyFinancialsDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateFinancialsAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "Financials updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/localization")]
        public async Task<IActionResult> UpdateLocalization(Guid id, [FromBody] UpdateCompanyLocalizationDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateLocalizationAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "Localization updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/addresses")]
        public async Task<IActionResult> UpdateAddresses(Guid id, [FromBody] UpdateCompanyAddressesDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateAddressesAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "Addresses updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/taxes")]
        public async Task<IActionResult> UpdateTaxes(Guid id, [FromBody] UpdateCompanyTaxesDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateTaxesAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "Taxes updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/system-controls")]
        public async Task<IActionResult> UpdateSystemControls(Guid id, [FromBody] UpdateCompanySystemControlsDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateSystemControlsAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "System controls updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/bank-accounts")]
        public async Task<IActionResult> UpdateBankAccounts(Guid id, [FromBody] UpdateCompanyBankAccountsDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateBankAccountsAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "Bank accounts updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> UpdateUsers(Guid id, [FromBody] UpdateCompanyUsersDto dto)
        {
            try
            {
                var newRowVersion = await _companySetupService.UpdateUsersAsync(id, dto);
                if (newRowVersion == null) return NotFound("Company not found or access denied.");
                return Ok(new { message = "Users updated successfully.", rowVersion = Convert.ToBase64String(newRowVersion) });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Another user has updated this record. Please refresh and try again." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var success = await _companySetupService.DeleteAsync(id);
            
            if (!success)
            {
                return NotFound("Company not found or access denied.");
            }

            return NoContent();
        }
    }
}
