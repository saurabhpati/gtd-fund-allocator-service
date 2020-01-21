using GTDFundAllocatorService.Domain.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GTDFundAllocatorService.Domain.Implementation
{
    public static class ServiceInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IFundManager, IFundManager>();
            return services;
        }
    }
}
