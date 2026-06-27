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
                var result = await _journalEntryService.SaveAsync(dto, null);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateJournalEntryDto dto)
        {
            try
            {
                var result = await _journalEntryService.SaveAsync(dto, id);
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
            try
            {
                var success = await _journalEntryService.DeleteAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/validate")]
        public async Task<IActionResult> ValidateVoucher(Guid id)
        {
            try
            {
                var (success, errors) = await _journalEntryService.ValidateVoucherAsync(id);
                return Ok(new { success, errors });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/simulate")]
        public async Task<IActionResult> SimulateVoucher(Guid id)
        {
            try
            {
                var result = await _journalEntryService.SimulateVoucherAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/submit")]
        public async Task<IActionResult> SendForApproval(Guid id)
        {
            try
            {
                var result = await _journalEntryService.SendForApprovalAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalRequest request)
        {
            try
            {
                var result = await _journalEntryService.ApproveAsync(id, request.Remarks);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] ApprovalRequest request)
        {
            try
            {
                var result = await _journalEntryService.RejectAsync(id, request.Remarks);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/post")]
        public async Task<IActionResult> PostVoucher(Guid id)
        {
            try
            {
                var result = await _journalEntryService.PostVoucherAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/reverse")]
        public async Task<IActionResult> ReverseVoucher(Guid id)
        {
            try
            {
                var result = await _journalEntryService.ReverseVoucherAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> AddAttachment(Guid id, [FromBody] AttachmentRequest request)
        {
            try
            {
                var attachment = new JournalVoucherAttachment
                {
                    Name = request.Name,
                    Size = request.Size,
                    Type = request.Type,
                    UploadedAt = DateTime.UtcNow
                };
                var result = await _journalEntryService.AddAttachmentAsync(id, attachment);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/attachments/{fileName}")]
        public async Task<IActionResult> RemoveAttachment(Guid id, string fileName)
        {
            try
            {
                var result = await _journalEntryService.RemoveAttachmentAsync(id, fileName);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ApprovalRequest
    {
        public string? Remarks { get; set; }
    }

    public class AttachmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
