
using SimpleStore.Application.Errors;

namespace SimpleStore.Application.Common
{
    public record Result
    {
        public bool IsSuccess { get; }
        public DomainError? Error { get; }

        protected Result(bool isSuccess, DomainError? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(DomainError error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));

        public static implicit operator Result(DomainError error) => Failure(error);
    }

    public record Result<T> : Result
    {
        public T? Value { get; }
        private Result(T value) : base(true, null) => Value = value;
        private Result(DomainError error) : base(false, error) { }

        public static implicit operator Result<T>(T value) => new(value);

        public static implicit operator Result<T>(DomainError error) => new(error);
    }
}
