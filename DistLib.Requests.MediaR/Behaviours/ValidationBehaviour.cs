using MediatR;
using FluentValidation;

namespace DistLib.Requests.MediaR;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
{
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
                CancellationToken cancellationToken)
        {
                if (!validators.Any())
                {
                        return await next(cancellationToken);
                }
                
                var context = new ValidationContext<TRequest>(request);
                var failures = validators
                        .Select(v => v.Validate(context))
                        .SelectMany(r => r.Errors)
                        .Where(f => f != null)
                        .ToList();

                if (failures.Count == 0)
                {
                       return await next(cancellationToken);
                }
                
                var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));

                // Return a single Error instance
                var error = new Error(request.GetType().Name, errorMessage);

                return ResultExtensions.FailureFor<TResponse>(error);
        }
}