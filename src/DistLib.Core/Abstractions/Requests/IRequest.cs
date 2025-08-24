using MediatR;

namespace DistLib;

/// <summary>
/// Empty Marker for requests to application use cases through a pipeline
/// </summary>
public interface IRequest<out TResponse> : MediatR.IRequest<TResponse>
{
    
}

public interface IRequestHandler<in TRequest, TResponse> : MediatR.IRequestHandler<TRequest, TResponse> 
    where TRequest : MediatR.IRequest<TResponse>
{
}

public interface IPipelineBehavior<in TRequest,  TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{

}