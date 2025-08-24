namespace DistLib;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<T>(T domainEvent, CancellationToken cancellationToken) 
        where T : IDomainEvent;
}

public delegate Task DomainEventHandlerDelegate();