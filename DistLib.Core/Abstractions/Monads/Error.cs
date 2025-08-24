namespace DistLib;

/// <summary>
/// Represents an error with an associated value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value associated with the error.</typeparam>
public class Error<T> : Error
{
    /// <summary>
    /// Gets the value associated with the error.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error{T}"/> class with the specified error details and associated value.
    /// </summary>
    /// <param name="value">The value associated with the error.</param>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public Error(T value, string code, string message)
        : base(code, message)
    {
        Value = value;
    }
}

/// <summary>
/// Represents a concrete domain error.
/// </summary>
public class Error : IEquatable<Error>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Gets the empty error instance.
    /// </summary>
    /// <remarks>
    /// This represents the absence of an error and is used in success results.
    /// </remarks>
    public static Error None => new(string.Empty, string.Empty);

    /// <summary>
    /// Gets the error code.
    /// </summary>
    /// <remarks>
    /// The code is a string identifier for the error type, which can be used for error handling and localization.
    /// </remarks>
    public string Code { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <remarks>
    /// The message provides a human-readable description of the error.
    /// </remarks>
    public string Message { get; }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a string by returning its code.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>The error code, or an empty string if the error is null.</returns>
    public static implicit operator string(Error error) => error?.Code ?? string.Empty;

    /// <summary>
    /// Determines whether two specified errors are equal.
    /// </summary>
    /// <param name="a">The first error to compare.</param>
    /// <param name="b">The second error to compare.</param>
    /// <returns>true if the errors are equal; otherwise, false.</returns>
    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two specified errors are not equal.
    /// </summary>
    /// <param name="a">The first error to compare.</param>
    /// <param name="b">The second error to compare.</param>
    /// <returns>true if the errors are not equal; otherwise, false.</returns>
    public static bool operator !=(Error a, Error b) => !(a == b);

    /// <summary>
    /// Determines whether the specified error is equal to the current error.
    /// </summary>
    /// <param name="other">The error to compare with the current error.</param>
    /// <returns>true if the specified error is equal to the current error; otherwise, false.</returns>
    public bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current error.
    /// </summary>
    /// <param name="obj">The object to compare with the current error.</param>
    /// <returns>true if the specified object is equal to the current error; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not Error error)
        {
            return false;
        }

        return Equals(error);
    }

    /// <summary>
    /// Returns the hash code for this error.
    /// </summary>
    /// <returns>A hash code for the current error.</returns>
    public override int GetHashCode() => HashCode.Combine(Code, Message);
}

