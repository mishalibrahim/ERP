using ERP.Features.CompanySetup.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.CompanySetup
{
    public interface ICompanySetupService
    {
        Task<List<CompanyListItemDto>> GetAllAsync();
        Task<CompanyDetailsDto?> GetByIdAsync(Guid id);
        Task<(Guid Id, byte[] RowVersion)> CreateDraftAsync(CreateCompanyDto dto);
        Task<byte[]?> UpdateGeneralInfoAsync(Guid id, UpdateCompanyGeneralDto dto);
        Task<byte[]?> UpdateFinancialsAsync(Guid id, UpdateCompanyFinancialsDto dto);
        Task<byte[]?> UpdateLocalizationAsync(Guid id, UpdateCompanyLocalizationDto dto);
        Task<byte[]?> UpdateAddressesAsync(Guid id, UpdateCompanyAddressesDto dto);
        Task<byte[]?> UpdateTaxesAsync(Guid id, UpdateCompanyTaxesDto dto);
        Task<byte[]?> UpdateSystemControlsAsync(Guid id, UpdateCompanySystemControlsDto dto);
        Task<byte[]?> UpdateBankAccountsAsync(Guid id, UpdateCompanyBankAccountsDto dto);
        Task<byte[]?> UpdateUsersAsync(Guid id, UpdateCompanyUsersDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
