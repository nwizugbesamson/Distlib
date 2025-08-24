using System.Linq.Expressions;

namespace DistLib;

/// <summary>
/// Represents a search request for type <typeparamref name="T"/>, allowing specification of properties to search on.
/// </summary>
/// <typeparam name="T">The type whose properties will be searched.</typeparam>
/// <remarks>
/// Example usage:
/// <code>
/// var search = new SearchRequest&lt;User&gt;("Alice");
/// search.AddSearchProperties(new[] { "FirstName", "LastName", "Email" });
/// 
/// foreach (var selector in search.ToSelectors())
/// {
///     // Use selector in LINQ queries
/// }
/// 
/// Console.WriteLine($"Number of properties: {search.Count}");
/// </code>
/// </remarks>
public sealed class SearchRequest<T>(string searchTerm)
{
    private readonly HashSet<string> _searchPropertyFields = new();

    /// <summary>
    /// The search term to look for in the specified properties.
    /// </summary>
    public string SearchTerm { get; init; } = searchTerm;

    /// <summary>
    /// The names of the properties included in this search request.
    /// </summary>
    public IEnumerable<string> PropertyNames => _searchPropertyFields;

    /// <summary>
    /// The number of properties included in this search request.
    /// </summary>
    public int Count => _searchPropertyFields.Count;

    /// <summary>
    /// Adds property names to be included in the search.
    /// </summary>
    /// <param name="properties">The collection of property names to search on.</param>
    public void AddSearchProperties(IEnumerable<string> properties)
    {
        foreach (var property in properties)
        {
            _searchPropertyFields.Add(property);
        }
    }

    /// <summary>
    /// Converts the property names into LINQ expression selectors for use in queries.
    /// </summary>
    /// <returns>An enumerable of <see cref="Expression{Func}"/> for each property name.</returns>
    public IEnumerable<Expression<Func<T, object>>> ToSelectors()
    {
        foreach (var propertyName in _searchPropertyFields)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            yield return Expression.Lambda<Func<T, object>>(property, parameter);
        }
    }
}
