using MediatR;

namespace DistLib.Requests.MediaR;

public class CommandDispatcher(IMediator mediator) : ICommandDispatcher
{
    public async Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken) 
        where TCommand : class, ICommand<TResponse> where TResponse : Result
    {
        return await mediator.Send(command, cancellationToken);
    }
}