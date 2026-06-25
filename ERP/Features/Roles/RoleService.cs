using Erp.Module.Core.Data;
using Erp.Shared.Interfaces;
using ERP.Features.Roles.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Features.Roles
{
    public class RoleService : IRoleService
    {
        private readonly CoreDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RoleService(CoreDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            var tenantId = _currentUserService.TenantId;

            // Return roles that belong to the current tenant OR system roles (TenantId == null)
            return await _context.Roles
                .Where(r => r.TenantId == tenantId || r.TenantId == null)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole
                })
                .ToListAsync();
        }
    }
}
