using ERP.Features.Dimensions.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.Dimensions
{
    public interface IDimensionService
    {
        Task<List<DimensionDto>> GetAllAsync();
        Task<DimensionDto?> GetByIdAsync(Guid id);
        Task<DimensionDto> CreateAsync(CreateDimensionDto dto);
        Task<DimensionDto?> UpdateAsync(Guid id, UpdateDimensionDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
