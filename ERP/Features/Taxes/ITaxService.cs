using ERP.Features.Taxes.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.Taxes
{
    public interface ITaxService
    {
        Task<List<TaxGroupDto>> GetAllAsync();
        Task<TaxGroupDto?> GetByIdAsync(Guid id);
        Task<TaxGroupDto> CreateAsync(CreateTaxGroupDto dto);
        Task<TaxGroupDto?> UpdateAsync(Guid id, UpdateTaxGroupDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
