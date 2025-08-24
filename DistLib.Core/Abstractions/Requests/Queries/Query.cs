namespace DistLib;

/// <summary>
/// Represents a query that returns a response of type <typeparamref name="TResponse"/>.
/// Part of the **CQRS (Command Query Responsibility Segregation)** pattern,
/// where queries are used solely to retrieve data and do not modify system state.
/// </summary>
/// <typeparam name="TResponse">
/// The type of response returned by the query handler, derived from <see cref="Result"/>.
/// Can represent success, failure, or any data returned by the query.
/// </typeparam>
/// <remarks>
/// Example usage:
/// <code>
/// public record UserQuery(Guid Id) : DistLib.IQuery&lt;DistLib.Result&gt;;
/// </code>
/// </remarks>
public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : Result
{
}

/// <summary>
/// Marker interface for queries that return paginated results.
/// </summary>
/// <typeparam name="TItem">The type of items in the paginated result.</typeparam>
/// <typeparam name="TResult">The type of the paginated result, derived from <see cref="Result"/>.</typeparam>
/// <remarks>
/// Example usage:
/// <code>
/// var paginatedQuery = new GetUsersQuery
/// {
///     Pagination = new PaginatedRequest { Page = 1, PageSize = 20 },
///     Search = new SearchRequest&lt;UserDto&gt;("John"),
///     Sort = new SortedRequest&lt;UserDto&gt;("Name", QuerySortDitection.Ascending)
/// };
/// 
/// Result&lt;PaginatedResult&lt;UserDto&gt;&gt; paginatedResult = 
///     await queryDispatcher.DispatchAsync&lt;UserDto, Result&lt;PaginatedResult&lt;UserDto&gt;&gt;&gt;(paginatedQuery);
/// </code>
/// </remarks>
public interface IPaginatedQuery<TItem, out TResult> : IQuery<TResult>
    where TResult : Result<PaginatedResult<TItem>>
{
    /// <summary>
    /// The pagination parameters for the query.
    /// </summary>
    PaginatedRequest Pagination { get; set; }

    /// <summary>
    /// Optional search parameters for filtering the result set.
    /// </summary>
    SearchRequest<TItem>? Search { get; set; }

    /// <summary>
    /// Optional collection of filters to apply to the result set.
    /// </summary>
    RequestFilterCollection<TItem>? FilterCollection { get; set; }

    /// <summary>
    /// Optional sorting parameters for the result set.
    /// </summary>
    SortedRequest<TItem>? Sort { get; set; }
}


/// <summary>
/// Defines a handler responsible for processing a specific query of type <typeparamref name="TQuery"/>
/// and returning a response of type <typeparamref name="TResponse"/>.
/// Part of the **Mediator pattern**, decoupling the handler from the code that sends the query.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle, implementing <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler, derived from <see cref="Result"/>.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : Result
{
}

/// <summary>
/// Responsible for dispatching queries to their respective handlers.
/// Implements the **Mediator pattern**, decoupling query senders from handlers.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query to the appropriate handler and returns the response.
    /// Use this method for standard (non-paginated) queries.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected from the handler, derived from <see cref="Result"/>.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">Optional token for cancellation.</param>
    /// <returns>The response from the handler.</returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var userQuery = new GetUserQuery { UserId = 123 };
    /// Result&lt;UserDto&gt; userResult = await queryDispatcher.DispatchAsync(userQuery);
    /// if (userResult.IsSuccess)
    /// {
    ///     var user = userResult.Value;
    /// }
    /// </code>
    /// </remarks>
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : Result;

    /// <summary>
    /// Dispatches a paginated query to the appropriate handler and returns the paginated response.
    /// Use this method for queries that require pagination, sorting, filtering, or searching.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the paginated result.</typeparam>
    /// <typeparam name="TResponse">The type of paginated response, derived from <see cref="Result"/>.</typeparam>
    /// <param name="query">The paginated query to dispatch.</param>
    /// <param name="cancellationToken">Optional token for cancellation.</param>
    /// <returns>The paginated response from the handler.</returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var paginatedQuery = new GetUsersQuery
    /// {
    ///     Pagination = new PaginatedRequest { Page = 1, PageSize = 20 },
    ///     Search = new SearchRequest&lt;UserDto&gt; { Term = "John" },
    ///     Filter = new FilteredRequest&lt;UserDto&gt; { Status = "Active" },
    ///     Sort = new SortedRequest&lt;UserDto&gt; { Property = "Name", Direction = SortDirection.Ascending }
    /// };
    /// Result&lt;PaginatedResult&lt;UserDto&gt;&gt; result = 
    ///     await queryDispatcher.DispatchAsync&lt;UserDto, Result&lt;PaginatedResult&lt;UserDto&gt;&gt;&gt; (paginatedQuery);
    ///
    /// if (result.IsSuccess)
    /// {
    ///     var users = result.Value.Items;
    /// }
    /// </code>
    /// </remarks>
    Task<TResponse> DispatchAsync<TItem, TResponse>(IPaginatedQuery<TItem, TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : Result<PaginatedResult<TItem>>;
}
