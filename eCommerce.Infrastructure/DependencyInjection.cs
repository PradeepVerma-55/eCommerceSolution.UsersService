using eCommerce.Core.RepositoryContracts;
using eCommerce.Infrastructure.DbContext;
using eCommerce.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            
            services.AddScoped<DapperDbContext>();

            // Add infrastructure services here
            // Changed from Singleton to Scoped to avoid consuming a scoped service from a singleton
            services.AddScoped<IUsersRepository, UsersRepository>();
         

            return services;
        }

    }
}

