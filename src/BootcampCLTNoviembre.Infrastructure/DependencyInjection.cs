using BootcampCLTNoviembre.Application.Abstractions.Data;
using BootcampCLTNoviembre.Application.Abstractions.Messaging;
using BootcampCLTNoviembre.Domain.Products;
using BootcampCLTNoviembre.Infrastructure.Persistence;
using BootcampCLTNoviembre.Infrastructure.ReadModels;
using Microsoft.Extensions.DependencyInjection;

namespace BootcampCLTNoviembre.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<InMemoryProducts>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductsReadStore, ProductsReadStore>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(InMemoryAccessBehavior<,>));

        return services;
    }
}
