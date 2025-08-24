using MediatR;
using Microsoft.Extensions.Logging;

namespace DistLib.Requests.MediaR;

public class SideEffectRollBackBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : Result
{
    private readonly IRollBackService _rollBackService;
    private readonly ILogger<SideEffectRollBackBehaviour<TRequest, TResponse>> _logger;

    public SideEffectRollBackBehaviour(IRollBackService rollBackService, 
        ILogger<SideEffectRollBackBehaviour<TRequest, TResponse>> logger)
    {
        _rollBackService = rollBackService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next();
        if (result.IsSuccess)
        {
            _rollBackService.ClearRollBackActions();
            return result;
        }
        
        foreach (var rollBackAction in _rollBackService.RollBackActions)
        {
            try
            {
                await rollBackAction.RollBackAsync(cancellationToken);
                _logger.LogTrace("Successfully Rolled Back {RollBackAction}", nameof(rollBackAction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to rollback side effect {RollBackAction}.", 
                    nameof(rollBackAction));
            }
        }
        _rollBackService.ClearRollBackActions();
        return result;
    }
}