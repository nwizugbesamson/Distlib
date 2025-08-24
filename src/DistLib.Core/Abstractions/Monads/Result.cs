namespace DistLib
{
    /// <summary>
    /// Represents the result of an operation, including a status, optional error, 
    /// and allows categorisation of outcomes such as success, failure, or specific error types.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class with the specified status and error.
        /// </summary>
        /// <param name="status">The <see cref="ResultStatus"/> of the operation.</param>
        /// <param name="error">The <see cref="Error"/> associated with a failure. Must be <see cref="Error.None"/> if <paramref name="status"/> is <see cref="ResultStatus.Success"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a successful result contains an error, or a failure result contains no error.
        /// </exception>
        protected Result(ResultStatus status, Error error)
        {
            if (status == ResultStatus.Success && error != Error.None)
            {
                throw new InvalidOperationException(
                    "A successful result cannot contain an error.");
            }

            if (status != ResultStatus.Success && error == Error.None)
            {
                throw new InvalidOperationException(
                    "A failed result must contain an error.");
            }

            Status = status;
            Error = error;
        }

        /// <summary>
        /// Gets the status of the result.
        /// </summary>
        public ResultStatus Status { get; }

        /// <summary>
        /// Indicates whether the result represents a success.
        /// </summary>
        public bool IsSuccess => Status == ResultStatus.Success;

        /// <summary>
        /// Indicates whether the result represents a failure.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the <see cref="Error"/> associated with a failure result. Returns <see cref="Error.None"/> if successful.
        /// </summary>
        public Error Error { get; }

        #region Factory Methods

        /// <summary>
        /// Creates a successful <see cref="Result"/>.
        /// </summary>
        public static Result Success() => new(ResultStatus.Success, Error.None);

        /// <summary>
        /// Creates a failure <see cref="Result"/> with a specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The error describing the failure.</param>
        public static Result Failure(Error error) => new(ResultStatus.Failure, error);

        /// <summary>
        /// Creates a <see cref="Result"/> indicating a not found failure with the specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The error describing the not found condition.</param>
        public static Result NotFound(Error error) => new(ResultStatus.NotFound, error);

        /// <summary>
        /// Creates a <see cref="Result"/> indicating an unauthorized failure with the specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The error describing the unauthorized condition.</param>
        public static Result Unauthorized(Error error) => new(ResultStatus.Unauthorized, error);

        /// <summary>
        /// Creates a <see cref="Result"/> indicating an unauthenticated failure with the specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The error describing the unauthenticated condition.</param>
        public static Result Unauthenticated(Error error) => new(ResultStatus.UnAuthenticated, error);

        /// <summary>
        /// Creates a successful <see cref="Result{T}"/> containing a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value of the successful result.</param>
        public static Result<T> Success<T>(T value) => new(value, ResultStatus.Success, Error.None);

        /// <summary>
        /// Creates a failed <see cref="Result{T}"/> with a specified <see cref="Error"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value (ignored).</typeparam>
        /// <param name="error">The error describing the failure.</param>
        public static Result<T> Failure<T>(Error error) => new(default!, ResultStatus.Failure, error);

        /// <summary>
        /// Creates a <see cref="Result{T}"/> indicating not found with a specified <see cref="Error"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value (ignored).</typeparam>
        /// <param name="error">The error describing the not found condition.</param>
        public static Result<T> NotFound<T>(Error error) => new(default!, ResultStatus.NotFound, error);

        /// <summary>
        /// Creates a <see cref="Result{T}"/> indicating unauthorized with a specified <see cref="Error"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value (ignored).</typeparam>
        /// <param name="error">The error describing the unauthorized condition.</param>
        public static Result<T> Unauthorized<T>(Error error) => new(default!, ResultStatus.Unauthorized, error);

        /// <summary>
        /// Creates a <see cref="Result{T}"/> indicating unauthenticated with a specified <see cref="Error"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value (ignored).</typeparam>
        /// <param name="error">The error describing the unauthenticated condition.</param>
        public static Result<T> Unauthenticated<T>(Error error) => new(default!, ResultStatus.UnAuthenticated, error);

        /// <summary>
        /// Returns the first failure from a set of results, or success if none exist.
        /// </summary>
        /// <param name="results">An array of <see cref="Result"/> instances to evaluate.</param>
        /// <returns>The first failure <see cref="Result"/> encountered, or a success result if none exist.</returns>
        public static Result FirstFailureOrSuccess(params Result[] results)
        {
            foreach (var result in results)
            {
                if (result.IsFailure) return result;
            }
            return Success();
        }

        #endregion
    }

    /// <summary>
    /// Represents a result of some operation with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    public class Result<T> : Result
    {
        private readonly T? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the result.</param>
        /// <param name="status">The status of the result.</param>
        /// <param name="error">The error describing a failure (if any).</param>
        protected internal Result(T? value, ResultStatus status, Error error)
            : base(status, error)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of a successful result.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a failure.</exception>
        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access the value of a failure result.");

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator Result<T>(T value) => Success(value);
    }

    /// <summary>
    /// Enumerates the possible statuses of a <see cref="Result"/> or <see cref="Result{T}"/>.
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>Indicates the operation succeeded.</summary>
        Success,

        /// <summary>Indicates the requested entity was not found.</summary>
        NotFound,

        /// <summary>Indicates the caller is unauthenticated.</summary>
        UnAuthenticated,

        /// <summary>Indicates the caller is unauthorized to perform the operation.</summary>
        Unauthorized,

        /// <summary>Indicates a generic failure occurred.</summary>
        Failure
    }
}
