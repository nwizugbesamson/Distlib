using MediatR;

namespace DistLib.Requests.MediaR;

public class QueryDispatcher(IMediator mediator): IQueryDispatcher
{
    public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken ) 
        where TResponse : Result
    {
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<TResponse> DispatchAsync<TItem, TResponse>(IPaginatedQuery<TItem, TResponse> query, 
        CancellationToken cancellationToken ) where TResponse : Result<PaginatedResult<TItem>>
    {
        return await mediator.Send(query, cancellationToken);
    }
}