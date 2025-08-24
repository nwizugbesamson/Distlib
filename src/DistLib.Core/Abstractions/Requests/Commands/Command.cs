namespace DistLib;

/// <summary>
/// Represents a command that returns a response of type <typeparamref name="TResponse"/>.
/// This interface is part of the **Command** side of the **CQRS (Command Query Responsibility Segregation)** pattern.
/// Commands are operations that **modify** the system's state, such as creating, updating, or deleting data.
/// This interface is used to define input required for an operation that changes the application's state.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command handler.
/// This must be derived from <see cref="Result"/>, and it can represent success, failure, or data returned by the command execution.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : Result
{
    // This interface does not define any methods itself,
    // but serves as a marker for commands to indicate that they expect a response.
}

/// <summary>
/// Defines a handler that processes a specific command of type <typeparamref name="TCommand"/> 
/// and returns a response of type <typeparamref name="TResponse"/>.
/// This handler is part of the **Mediator pattern**, which helps decouple the command logic from the components that dispatch the command.
/// Instead of directly invoking a command handler, the system dispatches the command to a mediator, which then routes it to the appropriate handler.
/// </summary>
/// <typeparam name="TCommand">The type of the command to be handled, which must implement <see cref="ICommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response produced by the handler. This type must be derived from <see cref="Result"/> and represents the outcome of the command execution.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : Result
{
        
}

  
/// <summary>
/// Defines a dispatcher responsible for dispatching commands to their respective handlers.
/// This interface is part of the **Mediator pattern** and serves as an intermediary for sending commands to their appropriate handlers.
/// The dispatcher decouples the code that sends the command from the logic that handles the command, improving maintainability and flexibility.
/// </summary>
public interface ICommandDispatcher

{
    /// <summary>
    /// Dispatches a command of type <typeparamref name="TCommand"/> to the appropriate handler 
    /// and returns the response of type <typeparamref name="TResponse"/>.
    /// The dispatcher uses the Mediator pattern to ensure that the command is routed to the correct handler 
    /// without the sender needing to know anything about the handler's implementation.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to be dispatched, which must implement <see cref="ICommand{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response expected from the handler, which must be derived from <see cref="Result"/>.</typeparam>
    /// <param name="command">The command to be dispatched, containing the information needed to perform the operation.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete. This allows the operation to be canceled if necessary.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the response of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TResponse>
        where TResponse : Result;
}