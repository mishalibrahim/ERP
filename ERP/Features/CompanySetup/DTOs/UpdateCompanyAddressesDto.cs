using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanyAddressesDto : UpdateCompanyBaseDto
        {
            public AddressDetailsDto? RegisteredAddress { get; set; }
            public AddressDetailsDto? BillingAddress { get; set; }
        }
}

