namespace DistLib;

/// <summary>
    /// This interface is a core component of the Domain-Driven Design (DDD) event sourcing pattern.
    /// It allows aggregates to maintain a collection of domain events that represent state changes
    /// and provides a mechanism to clear these events after they have been processed or persisted.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface should be used by aggregate roots to track domain events
    /// that occur during business operations, enabling event sourcing, audit trails, and integration
    /// with external systems through event publishing.
    /// </remarks>
public interface IHasDomainEvent
{
    /// <summary>
    /// Clears all domain events from the aggregate's event collection.
    /// </summary>
    /// <remarks>
    /// This method should be called after domain events have been successfully processed,
    /// published, or persisted to prevent duplicate event handling. It is typically invoked
    /// by the application layer after the aggregate has been saved to the repository.
    /// 
    /// Clearing events ensures that subsequent operations on the same aggregate instance
    /// do not include previously processed events in the event collection.
    /// </remarks>
    void ClearEvents();

    /// <summary>
    /// Gets a read-only collection of all domain events that have occurred within this aggregate.
    /// </summary>
    /// <value>
    /// A read-only collection containing all <see cref="IDomainEvent"/> instances that represent state changes
    /// or business operations that have occurred within the aggregate's boundary.
    /// </value>
    /// <remarks>
    /// This property provides access to all domain events that have been raised by the aggregate
    /// since its last event clearing operation. The collection is read-only to prevent external
    /// modification of the event collection, ensuring that only the aggregate itself can add
    /// new events through its business logic.
    /// 
    /// Domain events in this collection are persisted to an outbox during an aggregate storage transaction
    /// and processed by the current mechanism of message transport implemented
    /// </remarks>
    IReadOnlyCollection<IDomainEvent> Events { get; }
}