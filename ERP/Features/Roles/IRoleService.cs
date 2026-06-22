using ERP.Features.Roles.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.Roles
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetRolesAsync();
    }
}
