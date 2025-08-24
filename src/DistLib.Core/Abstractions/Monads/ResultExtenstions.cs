using System.Collections.Concurrent;
using System.Reflection;

namespace DistLib;

/// <summary>
/// Provides extension methods for <see cref="Result{T}"/> for functional transformations and chaining.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Transforms the value of a successful <see cref="Result{T}"/> into another type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="U">The type of the output value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="func">The transformation function.</param>
    /// <returns>
    /// A new <see cref="Result{U}"/> containing the transformed value if successful;
    /// otherwise, a failure <see cref="Result{U}"/> with the same <see cref="Result.Status"/> and <see cref="Result.Error"/>.
    /// </returns>
    public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> func)
    {
        if (result.IsSuccess)
            return Result.Success(func(result.Value));

        return new Result<U>(default!, result.Status, result.Error);
    }

    /// <summary>
    /// Chains another <see cref="Result{U}"/>-returning operation if the current result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="U">The type of the resulting value.</typeparam>
    /// <param name="result">The result to bind from.</param>
    /// <param name="func">The function producing the next <see cref="Result{U}"/>.</param>
    /// <returns>
    /// The next <see cref="Result{U}"/> if successful; otherwise, a failure <see cref="Result{U}"/> with the same <see cref="Result.Status"/> and <see cref="Result.Error"/>.
    /// </returns>
    public static Result<U> Bind<T, U>(this Result<T> result, Func<T, Result<U>> func)
    {
        if (result.IsSuccess)
            return func(result.Value);

        return new Result<U>(default!, result.Status, result.Error);
    }
    
    private static readonly ConcurrentDictionary<Type, MethodInfo> _failureMethodCache = new();

    public static TResponse FailureFor<TResponse>(this Error error)
        where TResponse : Result
    {
        // Direct case for non-generic Result
        if (typeof(TResponse) == typeof(Result))
            return (Result.Failure(error) as TResponse)!;

        // For Result<T>, get the inner type
        var innerType = typeof(TResponse).GetGenericArguments()[0]!;

        // Try get MethodInfo from cache
        var genericMethod = _failureMethodCache.GetOrAdd(innerType, t =>
        {
            var methodInfo = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "Failure"
                    && m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(Error));

            if (methodInfo == null)
                throw new InvalidOperationException($"Cannot find Failure<T>(Error) method on Result for type {t.Name}");

            return methodInfo.MakeGenericMethod(t);
        });

        // Invoke the generic method
        return (TResponse)genericMethod.Invoke(null, new object[] { error })!;
    }
}

/// <summary>
/// Provides asynchronous extension methods for <see cref="Result{T}"/> to support <see cref="Task"/>-based workflows.
/// </summary>
public static class ResultAsyncExtensions
{
    /// <summary>
    /// Asynchronously transforms the value of a successful <see cref="Result{T}"/> inside a <see cref="Task"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="U">The type of the output value.</typeparam>
    /// <param name="task">The asynchronous <see cref="Result{T}"/> task.</param>
    /// <param name="func">The transformation function.</param>
    /// <returns>
    /// A <see cref="Task{Result{U}}"/> containing the transformed value if successful;
    /// otherwise, a failure <see cref="Result{U}"/> with the same <see cref="Result.Status"/> and <see cref="Result.Error"/>.
    /// </returns>
    public static async Task<Result<U>> MapAsync<T, U>(
        this Task<Result<T>> task,
        Func<T, U> func)
    {
        var result = await task.ConfigureAwait(false);
        return result.Map(func);
    }

    /// <summary>
    /// Asynchronously chains another <see cref="Result{U}"/>-returning operation if the current result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="U">The type of the resulting value.</typeparam>
    /// <param name="task">The asynchronous <see cref="Result{T}"/> task.</param>
    /// <param name="func">The function producing the next <see cref="Task{Result{U}}"/>.</param>
    /// <returns>
    /// A <see cref="Task{Result{U}}"/> representing the next operation if successful;
    /// otherwise, a failure <see cref="Result{U}"/> with the same <see cref="Result.Status"/> and <see cref="Result.Error"/>.
    /// </returns>
    public static async Task<Result<U>> BindAsync<T, U>(
        this Task<Result<T>> task,
        Func<T, Task<Result<U>>> func)
    {
        var result = await task.ConfigureAwait(false);
        return result.IsSuccess
            ? await func(result.Value).ConfigureAwait(false)
            : new Result<U>(default!, result.Status, result.Error);
    }
    
    
}
