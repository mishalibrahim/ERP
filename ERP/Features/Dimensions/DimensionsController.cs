using ERP.Features.Dimensions.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP.Features.Dimensions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DimensionsController : ControllerBase
    {
        private readonly IDimensionService _dimensionService;

        public DimensionsController(IDimensionService dimensionService)
        {
            _dimensionService = dimensionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dimensionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _dimensionService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDimensionDto dto)
        {
            try
            {
                var result = await _dimensionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDimensionDto dto)
        {
            try
            {
                var result = await _dimensionService.UpdateAsync(id, dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _dimensionService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
