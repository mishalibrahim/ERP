using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanyBankAccountsDto : UpdateCompanyBaseDto
        {
            public List<BankAccountDto>? BankAccounts { get; set; }
        }
}

