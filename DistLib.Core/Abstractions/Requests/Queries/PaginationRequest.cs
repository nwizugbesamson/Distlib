using System.Linq.Expressions;

namespace DistLib;

/// <summary>
/// Represents the pagination parameters for a query or request.
/// </summary>
/// <param name="PageNumber">
/// The current page number. Defaults to 1. Must be greater than or equal to 1.
/// </param>
/// <param name="PageSize">
/// The number of items per page. Defaults to 10. Should be a positive number.
/// </param>
public record PaginatedRequest(int PageNumber = 1, int PageSize = 10);
