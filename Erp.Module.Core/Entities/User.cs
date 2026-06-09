using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Module.Core.Entities
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TenantId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // MVP Roles: "SuperAdmin", "CompanyAdmin", "Accountant", "Sales"
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }
}
