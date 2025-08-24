using Microsoft.Extensions.DependencyInjection;

namespace DistLib;

public interface IDistLibBuilder
{
    IServiceCollection Services { get; }
    
    IServiceCollection Build();
    T GetRequiredService<T>() where T : notnull;
}