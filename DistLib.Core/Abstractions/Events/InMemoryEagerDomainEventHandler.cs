using Microsoft.Extensions.DependencyInjection;

namespace DistLib;

public class InMemoryEagerDomainEventHandler(IServiceScopeFactory serviceScopeFactory) : IDomainEventDispatcher
{
    private readonly IServiceScopeFactory  _serviceScopeFactory = serviceScopeFactory;

    public async Task DispatchAsync<T>(T domainEvent, CancellationToken cancellationToken) where T : IDomainEvent
    {
        var handlerType = typeof(IEagerDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        DomainEventHandlerDelegate handlerDelegate = async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var handlerObjects = serviceProvider.GetServices(handlerType);
                  
            var handlers = handlerObjects?.Cast<object>().ToList() ?? new List<object>();

            if ( !handlers.Any())
            {
                return;
            }

            foreach (var handler in handlers)
            {
                var method = handler.GetType().GetMethod("HandleAsync");
                if (method == null) continue;
                // Invoke each handler's HandleAsync method
                await (Task)method.Invoke(handler, new object[] { domainEvent, cancellationToken });
            }
        };
        
        await handlerDelegate();
    }
}