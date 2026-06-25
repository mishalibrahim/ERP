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
        Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto);
        Task<bool> PostAsync(Guid id);
    }
}
