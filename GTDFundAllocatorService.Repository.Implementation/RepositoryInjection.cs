using GTDFundAllocatorService.Repository.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GTDFundAllocatorService.Repository.Implementation
{
    public static class RepositoryInjection
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(IGeneralRepository<,>), typeof(GeneralRepository<,>))
                .AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));

            return services;
        }
    }
}
