using ERP.Features.GlAccounts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP.Features.GlAccounts
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GlAccountsController : ControllerBase
    {
        private readonly IGlAccountService _glAccountService;

        public GlAccountsController(IGlAccountService glAccountService)
        {
            _glAccountService = glAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _glAccountService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var result = await _glAccountService.GetTreeAsync();
            return Ok(result);
        }

        [HttpGet("next-number")]
        public async Task<IActionResult> GetNextAccountNumber([FromQuery] Erp.Shared.Enums.GlAccountCategory category, [FromQuery] Guid? parentId)
        {
            var result = await _glAccountService.GetNextAccountNumberAsync(category, parentId);
            return Ok(new { nextAccountNumber = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _glAccountService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGlAccountDto dto)
        {
            try
            {
                var result = await _glAccountService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGlAccountDto dto)
        {
            var result = await _glAccountService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _glAccountService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
