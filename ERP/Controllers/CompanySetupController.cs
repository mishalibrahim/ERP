using ERP.DTOs.CompanySetup;
using ERP.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ERP.Controllers
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

        [HttpPost]
        public async Task<IActionResult> CreateDraft([FromBody] CreateCompanyDto dto)
        {
            var id = await _companySetupService.CreateDraftAsync(dto);
            return Ok(new { id = id, message = "Company draft created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(string id, [FromBody] UpdateCompanyDto dto)
        {
            var success = await _companySetupService.UpdateCompanyAsync(id, dto);
            
            if (!success)
            {
                return NotFound("Company not found or access denied.");
            }

            return Ok(new { message = "Company draft updated successfully." });
        }
    }
}
