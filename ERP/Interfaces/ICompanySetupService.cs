using ERP.DTOs.CompanySetup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Interfaces
{
    public interface ICompanySetupService
    {
        Task<List<CompanyListItemDto>> GetAllAsync();
        Task<CompanyListItemDto?> GetByIdAsync(string id);
        Task<string> CreateDraftAsync(CreateCompanyDto dto);
        Task<bool> UpdateCompanyAsync(string id, UpdateCompanyDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
