using ERP.Features.JournalEntries.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP.Features.JournalEntries
{
    [ApiController]
    [Route("api/journal-entries")]
    [Authorize]
    public class JournalEntriesController : ControllerBase
    {
        private readonly IJournalEntryService _journalEntryService;

        public JournalEntriesController(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _journalEntryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _journalEntryService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJournalEntryDto dto)
        {
            try
            {
                var result = await _journalEntryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/post")]
        public async Task<IActionResult> PostJournal(Guid id)
        {
            try
            {
                var success = await _journalEntryService.PostAsync(id);
                if (!success) return NotFound();
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
