using ERP.DTOs.CompanySetup;
using System.Threading.Tasks;

namespace ERP.Interfaces
{
    public interface ICompanySetupService
    {
        Task<string> CreateDraftAsync(CreateCompanyDto dto);
        Task<bool> UpdateCompanyAsync(string id, UpdateCompanyDto dto);
    }
}
