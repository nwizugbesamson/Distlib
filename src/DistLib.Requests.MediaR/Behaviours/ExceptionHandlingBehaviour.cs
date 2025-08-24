using MediatR;
using Microsoft.Extensions.Logging;

namespace DistLib.Requests.MediaR;

public class ExceptionHandlingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : Result
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehaviour(ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> logger, 
        IEnvironmentProvider environmentProvider)
    {
        _logger = logger;
        _environmentProvider = environmentProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(
                ex,
                "Request: Unhandled Exception for Request {Name} {@Request}",
                requestName,
                request);
            
            Error error;
            if (_environmentProvider.IsDevelopment())
            {
                error = new Error($"Error Processing Request {typeof(TRequest).Name}", 
                    $"{ex.Message}{_environmentProvider.NewLine}{ex.StackTrace}");
                return ResultExtensions.FailureFor<TResponse>(error);
            }
            
            error = new Error("Internal Server Error", "Something went wrong");
            return ResultExtensions.FailureFor<TResponse>(error);
        }

        
    }
}