using System;
using System.Linq.Expressions;

namespace DistLib;

/// <summary>
/// Represents a request to sort a collection of <typeparamref name="T"/>.
/// Can be specified either by a property name or a property selector expression.
/// </summary>
/// <typeparam name="T">The type of the objects to sort.</typeparam>
/// <remarks>
/// Example usage:
/// <code>
/// // Using property name
/// var sortByName = new SortedRequest&lt;User&gt;("Name", QuerySortDitection.Ascending);
///
/// // Using expression
/// var sortByDate = new SortedRequest&lt;User&gt;(x =&gt; x.CreatedDate, QuerySortDitection.Descending);
///
/// // Convert to expression
/// Expression&lt;Func&lt;User, object&gt;&gt; selector = sortByName.ToSelector();
/// </code>
/// </remarks>
public class SortedRequest<T>
{
    /// <summary>
    /// Optional: name of the property to sort by. Takes priority over <see cref="PropertySelector"/> if both are provided.
    /// </summary>
    public string? PropertyName { get; init; }

    /// <summary>
    /// Optional: expression to select the property to sort by.
    /// </summary>
    public Expression<Func<T, object>>? PropertySelector { get; init; }

    /// <summary>
    /// The direction to sort by: ascending or descending.
    /// </summary>
    public QuerySortDitection SortDirection { get; init; }

    /// <summary>
    /// Initializes a new <see cref="SortedRequest{T}"/> using a property selector expression.
    /// </summary>
    /// <param name="propertySelector">Expression selecting the property to sort by.</param>
    /// <param name="sortDirection">Direction to sort.</param>
    public SortedRequest(Expression<Func<T, object>> propertySelector, QuerySortDitection sortDirection)
    {
        PropertySelector = propertySelector;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Initializes a new <see cref="SortedRequest{T}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">Name of the property to sort by.</param>
    /// <param name="sortDirection">Direction to sort.</param>
    public SortedRequest(string propertyName, QuerySortDitection sortDirection)
    {
        PropertyName = propertyName.PascalCase();
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Converts the request into a property selector expression.
    /// Prioritises <see cref="PropertyName"/> over <see cref="PropertySelector"/>.
    /// </summary>
    /// <returns>An expression selecting the property for sorting.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if neither <see cref="PropertyName"/> nor <see cref="PropertySelector"/> is provided.
    /// </exception>
    public Expression<Func<T, object>> ToSelector()
    {
        if (!string.IsNullOrWhiteSpace(PropertyName))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, PropertyName);

            Expression body = property.Type.IsEnum
                ? Expression.Convert(Expression.Convert(property, typeof(int)), typeof(object))
                : Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(body, parameter);
        }

        if (PropertySelector != null)
            return PropertySelector;

        throw new InvalidOperationException("Either PropertyName or PropertySelector must be provided.");
    }
}

/// <summary>
/// Specifies the direction to sort by: ascending or descending.
/// </summary>
public enum QuerySortDitection
{
    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    Descending
}
