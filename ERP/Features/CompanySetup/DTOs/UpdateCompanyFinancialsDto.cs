using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanyFinancialsDto : UpdateCompanyBaseDto
        {
            public DateTime? FinancialYearStart { get; set; }
            public DateTime? FinancialYearEnd { get; set; }
            public DateTime? BooksStartDate { get; set; }
            public string? AccountingMethod { get; set; }
            public string? FiscalYear { get; set; }
            public string? BaseCurrency { get; set; }
            public string? ReportingCurrency { get; set; }
        }
}

