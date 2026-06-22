using System;

namespace ERP.Features.CompanySetup.DTOs
{
    public class CompanyListItemDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Emirate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? RegistrationDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

