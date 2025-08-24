using System.Reflection;
using DistLib.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistLib;


public static class Extensions
{
    public static IServiceCollection AddDistLib(this IServiceCollection services, IConfiguration configuration)
    {
        // bind IApplicationOptions
       services.Configure<ApplicationOptions>(configuration.GetSection("ApplicationOptions"));
       services.AddSingleton<IServiceId, ServiceId>();
       services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
       return services;
    }


    public static IServiceCollection AddInMemoryDomainEventDispatcher(this IServiceCollection services,
        Assembly[] assemblies)
    {
        services.AddScoped<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();
        services.AddKeyedScoped<IDomainEventDispatcher, InMemoryDomainEventDispatcher>("Eager");
        
        foreach (var interfaceType in new[] { typeof(IDomainEventHandler<>), typeof(IEagerDomainEventHandler<>) })
        {
            foreach (var assembly in assemblies)
            {
                var internalTypes = assembly.GetTypes()
                    .Where(t => !t.IsAbstract
                                && !t.IsGenericTypeDefinition
                                && interfaceType.IsAssignableFrom(t) );

                foreach (var impl in internalTypes)
                {
                    foreach (var serviceType in impl.GetInterfaces()
                                 .Where(i => i.IsGenericType &&
                                             i.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition()))
                    {
                        services.AddTransient(serviceType, impl);
                    }
                }
            }
        }
        return services;
    }

    /// <summary>
    /// Overrides the default json serializer
    /// </summary>
    /// <param name="services"></param>
    /// <param name="jsonSerializer"></param>
    /// <returns></returns>
    public static IServiceCollection WithJsonSerializer(this IServiceCollection services, IJsonSerializer jsonSerializer)
    {
        services.AddSingleton<IJsonSerializer>(jsonSerializer);
        return services;
    }
    public static string Underscore(this string value)
    {
        return string.Concat(value.Select<char, string>((Func<char, int, string>) ((x, i) => i <= 0 || !char.IsUpper(x) ? x.ToString() : "_" + x.ToString()))).ToLowerInvariant();
    }
    
    public static string PascalCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;

        var parts = str.Split(new[] { '_', '.' }, StringSplitOptions.RemoveEmptyEntries);

        var result = parts
            .Select(part =>
                part.Length == 0
                    ? part
                    : char.ToUpper(part[0]) + part.Substring(1))
            .ToArray();

        return string.Join("", result);
    }
}


