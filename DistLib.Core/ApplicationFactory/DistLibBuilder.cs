using Microsoft.Extensions.DependencyInjection;

namespace DistLib;

public sealed class DistLibBuilder(IServiceCollection services) : IDistLibBuilder
{
    public static IDistLibBuilder Create(IServiceCollection services)
    {
        return new DistLibBuilder(services);
    }

    public IServiceCollection Services => services;
    
    public IServiceCollection Build()
    {
        return services;
    }

    public T GetRequiredService<T>() where T : notnull
    {
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<T>();;
    }
}