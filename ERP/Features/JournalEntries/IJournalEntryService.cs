using ERP.Features.JournalEntries.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.JournalEntries
{
    public interface IJournalEntryService
    {
        Task<List<JournalEntryDto>> GetAllAsync();
        Task<JournalEntryDto?> GetByIdAsync(Guid id);
        Task<JournalEntryDto> SaveAsync(CreateJournalEntryDto dto, Guid? id);
        Task<bool> DeleteAsync(Guid id);
        Task<(bool Success, List<string> Errors)> ValidateVoucherAsync(Guid id);
        Task<List<object>> SimulateVoucherAsync(Guid id);
        Task<JournalEntryDto> SendForApprovalAsync(Guid id);
        Task<JournalEntryDto> ApproveAsync(Guid id, string? remarks);
        Task<JournalEntryDto> RejectAsync(Guid id, string? remarks);
        Task<JournalEntryDto> PostVoucherAsync(Guid id);
        Task<JournalEntryDto> ReverseVoucherAsync(Guid id);
        Task<JournalEntryDto> AddAttachmentAsync(Guid id, JournalVoucherAttachment attachment);
        Task<JournalEntryDto> RemoveAttachmentAsync(Guid id, string fileName);
    }
}
