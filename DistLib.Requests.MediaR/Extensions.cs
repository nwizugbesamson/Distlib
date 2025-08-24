using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DistLib.Requests.MediaR;

public static class Extensions
{

    public static IServiceCollection UseMediaRRequestPipeline(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SideEffectRollBackBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        
        
        return services;
    }
}