using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.CompanySetup
{
    public class CreateCompanyDto
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? TradeName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string CompanyCode { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LicenseType { get; set; } = string.Empty;
        
        public DateTime RegistrationDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = "UAE";
        
        [MaxLength(100)]
        public string Emirate { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? PlaceOfIncorporation { get; set; }
        
        public bool IsFreeZoneEntity { get; set; }
        public bool IsDesignatedZone { get; set; }
    }
}
