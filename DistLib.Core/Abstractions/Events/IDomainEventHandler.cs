namespace DistLib;

public interface IDomainEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken);
}

public interface IEagerDomainEventHandler<T> : IDomainEventHandler<T> where T : IDomainEvent
{
}