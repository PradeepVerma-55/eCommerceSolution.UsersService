using eCommerce.Core.ServiceContracts;
using eCommerce.Core.Services;
using eCommerce.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Core
{
    public static class DependencyInjection
    {

        /// <summary>
        /// Extension method to add infrastructure services to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // Add infrastructure services here

            services.AddTransient<IUserService, UserService>();

            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            return services;
        }

    }
}
