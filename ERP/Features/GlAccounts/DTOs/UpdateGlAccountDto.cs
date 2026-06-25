using Erp.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.GlAccounts.DTOs
{
    public class UpdateGlAccountDto
    {
        [Required]
        public string AccountName { get; set; } = string.Empty;
        public GlAccountType AccountType { get; set; }
        public GlAccountCategory AccountCategory { get; set; }
        public GlPostingType PostingType { get; set; }
        public bool AllowManualEntry { get; set; }
        public bool MandatoryDimensions { get; set; }
        public Guid? ParentAccountId { get; set; }
        public Guid? DefaultTaxGroupId { get; set; }
        public bool IsActive { get; set; }
    }
}
