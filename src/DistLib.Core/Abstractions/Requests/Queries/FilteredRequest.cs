using System.Linq.Expressions;
using LinqKit;

namespace DistLib;

/// <summary>
/// Represents a collection of request filters that can be combined into a single predicate expression.
/// </summary>
/// <typeparam name="T">The type the filters are applied to.</typeparam>
/// <remarks>
/// Example usage:
/// <code>
/// var filters = new RequestFilterCollection&lt;User&gt;();
/// filters.AddDateRangeFilter("CreatedDate", DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
/// filters.AddBooleanFilter("IsActive", true);
/// ExpressionStarter&lt;User&gt; predicate = filters.FilterCondition();
/// </code>
/// </remarks>
public class RequestFilterCollection<T>
{
    private readonly HashSet<IRequestFilter<T>> _filters = new(new RequestFilterComparer<T>());

    /// <summary>
    /// Builds a combined predicate from all filters and clears the collection.
    /// </summary>
    /// <returns>An <see cref="ExpressionStarter{T}"/> representing the combined filter conditions.</returns>
    public ExpressionStarter<T> FilterCondition()
    {
        var predicate = PredicateBuilder.New<T>(true);
        foreach (var filter in _filters)
        {
            predicate = predicate.And(filter.BuildCondition());
        }
        _filters.Clear();
        return predicate;
    }

    /// <summary>
    /// Gets the number of filters in the collection.
    /// </summary>
    public int Count => _filters.Count;

    /// <summary>
    /// Returns a copy of the filters and clears the internal collection.
    /// </summary>
    public IEnumerable<IRequestFilter<T>> GetFilters()
    {
        var returnValue = new HashSet<IRequestFilter<T>>(_filters, new RequestFilterComparer<T>());
        _filters.Clear();
        return returnValue;
    }

    /// <summary>
    /// Adds a date range filter for a specified property.
    /// </summary>
    public void AddDateRangeFilter(string propertyName, DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue && !endDate.HasValue) return;
        _filters.Add(new DateRangeFilter<T>
        {
            PropertyName = propertyName,
            StartDate = startDate,
            EndDate = endDate
        });
    }

    /// <summary>
    /// Adds an enum string filter for a specified property.
    /// </summary>
    public void AddEnumStringFilter(string propertyName, string[] allowedValues)
    {
        if (allowedValues.Length < 1) return;
        _filters.Add(new EnumStringFilter<T>
        {
            PropertyName = propertyName,
            AllowedValues = allowedValues
        });
    }

    /// <summary>
    /// Adds a boolean filter for a specified property.
    /// </summary>
    public void AddBooleanFilter(string propertyName, bool? value)
    {
        if (!value.HasValue) return;
        _filters.Add(new BooleanFilter<T>
        {
            PropertyName = propertyName,
            Value = value
        });
    }

    /// <summary>
    /// Adds a numeric range filter for a specified property.
    /// </summary>
    public void AddNumberRangeFilter(string propertyName, int? min, int? max)
    {
        if (!min.HasValue && !max.HasValue) return;
        _filters.Add(new NumberRangeFilter<T>
        {
            PropertyName = propertyName,
            MinValue = min,
            MaxValue = max
        });
    }
}

/// <summary>
/// Represents a single filter applied to a property of <typeparamref name="T"/>.
/// </summary>
public interface IRequestFilter<T>
{
    /// <summary>
    /// The name of the property being filtered.
    /// </summary>
    string PropertyName { get; set; }

    /// <summary>
    /// Builds the filter condition as a LINQ expression.
    /// </summary>
    Expression<Func<T, bool>> BuildCondition();
}

/// <summary>
/// Compares filters based on property names (case-insensitive).
/// </summary>
public class RequestFilterComparer<T> : IEqualityComparer<IRequestFilter<T>>
{
    public bool Equals(IRequestFilter<T>? x, IRequestFilter<T>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return string.Equals(x.PropertyName, y.PropertyName, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(IRequestFilter<T> obj)
    {
        return obj?.PropertyName?.ToLowerInvariant().GetHashCode() ?? 0;
    }
}

/// <summary>
/// A filter for a date range property.
/// </summary>
public sealed class DateRangeFilter<T> : IRequestFilter<T>
{
    public required string PropertyName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Builds a predicate that checks if the property is within the specified date range.
    /// </summary>
    public Expression<Func<T, bool>> BuildCondition()
    {
        var predicate = PredicateBuilder.New<T>(true);
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyExpression = Expression.Property(parameter, PropertyName);

        if (StartDate.HasValue)
        {
            var startExpr = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(StartDate.Value));
            predicate = predicate.And(Expression.Lambda<Func<T, bool>>(startExpr, parameter));
        }

        if (EndDate.HasValue)
        {
            var endExpr = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(EndDate.Value));
            predicate = predicate.And(Expression.Lambda<Func<T, bool>>(endExpr, parameter));
        }

        return predicate;
    }
}

/// <summary>
/// A filter that allows only specific string values for a property (commonly for enums stored as strings).
/// </summary>
public sealed class EnumStringFilter<T> : IRequestFilter<T>
{
    public required string PropertyName { get; set; }
    public required string[] AllowedValues { get; set; }

    public Expression<Func<T, bool>> BuildCondition()
    {
        var predicate = PredicateBuilder.New<T>(true);
        if (AllowedValues.Length == 0) return predicate;

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyExpression = Expression.Property(parameter, PropertyName);
        var containsExpr = Expression.Call(Expression.Constant(AllowedValues),
                                           typeof(string[]).GetMethod("Contains")!,
                                           propertyExpression);
        predicate = predicate.And(Expression.Lambda<Func<T, bool>>(containsExpr, parameter));

        return predicate;
    }
}

/// <summary>
/// A filter for boolean properties.
/// </summary>
public sealed class BooleanFilter<T> : IRequestFilter<T>
{
    public required string PropertyName { get; set; }
    public bool? Value { get; set; }

    public Expression<Func<T, bool>> BuildCondition()
    {
        var predicate = PredicateBuilder.New<T>(true);
        if (!Value.HasValue) return predicate;

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyExpression = Expression.Property(parameter, PropertyName);
        var valueExpr = Expression.Equal(propertyExpression, Expression.Constant(Value.Value));
        predicate = predicate.And(Expression.Lambda<Func<T, bool>>(valueExpr, parameter));

        return predicate;
    }
}

/// <summary>
/// A filter for numeric range properties.
/// </summary>
public sealed class NumberRangeFilter<T> : IRequestFilter<T>
{
    public required string PropertyName { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }

    public Expression<Func<T, bool>> BuildCondition()
    {
        var predicate = PredicateBuilder.New<T>(true);
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyExpression = Expression.Property(parameter, PropertyName);

        if (MinValue.HasValue)
        {
            var minExpr = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(MinValue.Value));
            predicate = predicate.And(Expression.Lambda<Func<T, bool>>(minExpr, parameter));
        }

        if (MaxValue.HasValue)
        {
            var maxExpr = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(MaxValue.Value));
            predicate = predicate.And(Expression.Lambda<Func<T, bool>>(maxExpr, parameter));
        }

        return predicate;
    }
}
