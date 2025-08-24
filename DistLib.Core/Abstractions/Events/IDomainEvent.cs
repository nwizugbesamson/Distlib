namespace DistLib;

public interface IDomainEvent
{
    public Guid Id { get; }
}

public abstract record DomainEventBase : IDomainEvent
{
    public Guid Id { get; init; }

    protected DomainEventBase()
    {
        Id = Guid.NewGuid();
    }
}