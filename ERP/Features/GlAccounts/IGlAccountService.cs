using ERP.Features.GlAccounts.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.GlAccounts
{
    public interface IGlAccountService
    {
        Task<List<GlAccountDto>> GetAllAsync();
        Task<List<GlAccountTreeNodeDto>> GetTreeAsync();
        Task<string> GetNextAccountNumberAsync(Erp.Shared.Enums.GlAccountCategory category, Guid? parentId);
        Task<GlAccountDto?> GetByIdAsync(Guid id);
        Task<GlAccountDto> CreateAsync(CreateGlAccountDto dto);
        Task<GlAccountDto?> UpdateAsync(Guid id, UpdateGlAccountDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
