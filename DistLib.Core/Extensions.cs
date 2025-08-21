using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistLib.Core;

public interface IDistLibBuilder
{
    IServiceCollection Services { get; }
}

public sealed class DistLibBuilder(IServiceCollection services) : IDistLibBuilder
{
    
    public static IDistLibBuilder Create(IServiceCollection services)
    {
        return new DistLibBuilder(services);
    }

    public IServiceCollection Services => services;
}

public static class Extensions
{
    public static IDistLibBuilder AddDistLib(this IServiceCollection services, IConfiguration configuration)
    {
        // bind IApplicationOptions
       services.Configure<ApplicationOptions>(configuration.GetSection("ApplicationOptions"));
       return DistLibBuilder.Create(services);
    }
}

public class ApplicationOptions
{
    public string Name { get; set;  }
    public string Version { get;  set; }
    public string Description { get; set;  }
    public string Identifier { get;  set; }
}
