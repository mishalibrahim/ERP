using Erp.Module.GL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.Module.GL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGlModuleServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GlDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(GlDbContext).Assembly.FullName)));

            // Register GL specific services here later

            return services;
        }
    }
}
