using Erp.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERP.Infrastructure.Authorization
{
    /// <summary>
    /// Attribute-based permission check. Apply to controllers or actions:
    ///   [RequirePermission(Permissions.Invoices.Approve)]
    /// 
    /// SuperAdmins bypass this check automatically.
    /// Returns 403 Forbidden if the user lacks the required permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permissionKey;

        public RequirePermissionAttribute(string permissionKey)
        {
            _permissionKey = permissionKey;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentUser = context.HttpContext.RequestServices
                .GetRequiredService<ICurrentUserService>();

            // Not authenticated at all
            if (currentUser.UserId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check permission (SuperAdmin bypass is handled inside HasPermission)
            if (!currentUser.HasPermission(_permissionKey))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

