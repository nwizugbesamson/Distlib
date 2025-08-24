using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DistLib.WebApi;

public static class Extensions
{
    // add json serializer
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
        return services;
    }   
    
    /// <summary>
    /// Binds a value to a property of the model using an expression.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="model">The model instance to bind the value to.</param>
    /// <param name="expression">An expression selecting the property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <returns>The updated model instance.</returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var user = new User();
    /// user.Bind(u =&gt; u.Name, "Alice");
    /// </code>
    /// </remarks>
    public static T Bind<T>(this T model, Expression<Func<T, object>> expression, object value)
    {
        return model.Bind<T, object>(expression, value);
    }

    /// <summary>
    /// Binds a new <see cref="Guid"/> to a property of type <see cref="Guid"/>.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="model">The model instance to bind the value to.</param>
    /// <param name="expression">An expression selecting the <see cref="Guid"/> property to set.</param>
    /// <returns>The updated model instance with a new <see cref="Guid"/> assigned.</returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var user = new User();
    /// user.BindId(u =&gt; u.Id); // Id is Guid, gets a new Guid
    /// </code>
    /// </remarks>
    public static T BindId<T>(this T model, Expression<Func<T, Guid>> expression)
    {
        return model.Bind<T, Guid>(expression, (object) Guid.NewGuid());
    }

    /// <summary>
    /// Binds a new <see cref="string"/> ID (formatted GUID) to a property of type <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="model">The model instance to bind the value to.</param>
    /// <param name="expression">An expression selecting the <see cref="string"/> property to set.</param>
    /// <returns>The updated model instance with a new string ID assigned.</returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var user = new User();
    /// user.BindId(u =&gt; u.StringId); // StringId gets new GUID string
    /// </code>
    /// </remarks>
    public static T BindId<T>(this T model, Expression<Func<T, string>> expression)
    {
        return model.Bind<T, string>(expression, (object) Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Sets the value of a property on the model using reflection, based on the property expression.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="model">The model instance to update.</param>
    /// <param name="expression">An expression selecting the property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <returns>The updated model instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the expression does not refer to a valid property.</exception>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// var user = new User();
    /// user.Bind(u =&gt; u.Name, "Bob");
    /// user.BindId(u =&gt; u.Id);
    /// user.BindId(u =&gt; u.StringId);
    /// </code>
    /// </remarks>
    private static TModel Bind<TModel, TProperty>(
        this TModel model,
        Expression<Func<TModel, TProperty>> expression,
        object value)
    {
        if (!(expression.Body is MemberExpression memberExpression))
            memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;

        if (memberExpression == null)
            throw new InvalidOperationException("Invalid member expression.");

        string propertyName = memberExpression.Member.Name.ToLowerInvariant();

        FieldInfo fieldInfo = model.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith($"<{propertyName}>"));

        if (fieldInfo == null)
            return model;

        fieldInfo.SetValue(model, value);
        return model;
    }
    public static IResult ToApiResult(
        this Result result,
        Func<IResult>? successMapper = null,
        string? successMessage = null)
    {
        switch (result.Status)
        {
            case ResultStatus.Success:
                if (successMapper != null)
                    return successMapper();

                if (!string.IsNullOrWhiteSpace(successMessage))
                    return Results.Ok(successMessage);

                return Results.NoContent();

            case ResultStatus.NotFound:
                return Results.NotFound(result.Error);

            case ResultStatus.UnAuthenticated:
                return Results.Unauthorized();

            case ResultStatus.Unauthorized:
                return Results.Forbid();

            case ResultStatus.Failure:
                return Results.BadRequest(result.Error);

            default:
                return Results.Problem("Unknown error", statusCode: 500);
        }
    }

    public static IResult ToApiResult<T>(
        this Result<T> result,
        Func<T?, IResult>? successMapper = null)
    {
        switch (result.Status)
        {
            case ResultStatus.Success:
                if (successMapper != null)
                    return successMapper(result.Value);

                // Default mapping
                if (result.Value is null)
                    return Results.NoContent(); // default for empty success
                return Results.Ok(result.Value);

            case ResultStatus.NotFound:
                return Results.NotFound(result.Error);
            
            case ResultStatus.UnAuthenticated:
                return Results.Unauthorized();

            case ResultStatus.Unauthorized:
                return Results.Forbid();

            case ResultStatus.Failure:
                return Results.BadRequest(result.Error);

            default:
                return Results.Problem("Unknown error", statusCode: 500);
        }
    }
}
