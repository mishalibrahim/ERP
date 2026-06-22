using ERP.DTOs.CompanySetup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Interfaces
{
    public interface ICompanySetupService
    {
        Task<List<CompanyListItemDto>> GetAllAsync();
        Task<CompanyDetailsDto?> GetByIdAsync(string id);
        Task<(string Id, byte[] RowVersion)> CreateDraftAsync(CreateCompanyDto dto);
        Task<byte[]?> UpdateGeneralInfoAsync(string id, UpdateCompanyGeneralDto dto);
        Task<byte[]?> UpdateFinancialsAsync(string id, UpdateCompanyFinancialsDto dto);
        Task<byte[]?> UpdateLocalizationAsync(string id, UpdateCompanyLocalizationDto dto);
        Task<byte[]?> UpdateAddressesAsync(string id, UpdateCompanyAddressesDto dto);
        Task<byte[]?> UpdateTaxesAsync(string id, UpdateCompanyTaxesDto dto);
        Task<byte[]?> UpdateSystemControlsAsync(string id, UpdateCompanySystemControlsDto dto);
        Task<byte[]?> UpdateBankAccountsAsync(string id, UpdateCompanyBankAccountsDto dto);
        Task<byte[]?> UpdateUsersAsync(string id, UpdateCompanyUsersDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
