using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

namespace DistLib.Serializers;

/// <summary>
/// Defines a generic JSON serializer interface.
/// </summary>
public interface IJsonSerializer
{
    T Deserialize<T>(string? json);
    string Serialize<T>(T? model);
}

/// <summary>
/// A JSON serializer implementation using System.Text.Json.
/// </summary>
public sealed class SystemJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="SystemJsonSerializer"/> with default options.
    /// </summary>
    public SystemJsonSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    /// <inheritdoc/>
    public T Deserialize<T>(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return Activator.CreateInstance<T>()!; // return default object

        return System.Text.Json.JsonSerializer.Deserialize<T>(json, _options) ?? Activator.CreateInstance<T>()!;
    }

    /// <inheritdoc/>
    public string Serialize<T>(T? model)
    {
        if (model == null)
            model = Activator.CreateInstance<T>(); // default instance

        return System.Text.Json.JsonSerializer.Serialize(model, _options);
    }
}

/// <summary>
/// A JSON serializer implementation using Newtonsoft.Json.
/// </summary>
public sealed class NewtonsoftJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerSettings _settings;

    /// <summary>
    /// Initializes a new instance of <see cref="NewtonsoftJsonSerializer"/> with default settings.
    /// </summary>
    public NewtonsoftJsonSerializer()
    {
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter(new CamelCaseNamingStrategy(),true) },
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    /// <inheritdoc/>
    public T Deserialize<T>(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return Activator.CreateInstance<T>()!; // return default object

        return JsonConvert.DeserializeObject<T>(json, _settings) ?? Activator.CreateInstance<T>()!;
    }

    /// <inheritdoc/>
    public string Serialize<T>(T? model)
    {
        if (model == null)
            model = Activator.CreateInstance<T>(); // default instance

        return JsonConvert.SerializeObject(model, _settings);
    }
}
