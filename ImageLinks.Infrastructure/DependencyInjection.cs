using ImageLinks.Application.Interfaces;
using ImageLinks.Infrastructure.Persistence.Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace ImageLinks.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IGenericService, GenericService>();
            services.AddScoped<IUsersService, UsersService>();
            return services;
        }
    }
}
