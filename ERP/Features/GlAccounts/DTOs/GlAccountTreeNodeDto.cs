using System.Collections.Generic;

namespace ERP.Features.GlAccounts.DTOs
{
    public class GlAccountTreeNodeDto : GlAccountDto
    {
        public List<GlAccountTreeNodeDto> Children { get; set; } = new();
    }
}
