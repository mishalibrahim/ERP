using Erp.Module.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.Module.Core
{
    public static class CoreModuleExtensions
    {
        public static IServiceCollection AddCoreModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CoreDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(CoreDbContext).Assembly.FullName)
                ));

            return services;
        }
    }
}
