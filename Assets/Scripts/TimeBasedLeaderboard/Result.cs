namespace TimeBasedLeaderboard {
    public class Result {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        protected Result(bool isSuccess, string errorMessage) {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true, null);

        public static Result Failure(string failureReason) => new(false, failureReason);
    }

    public class Result<T> : Result {
        public T Value { get; }

        private Result(T value, bool isSuccess, string? errorMessage = null)
            : base(isSuccess, errorMessage) {
            Value = value;
        }

        public static Result<T> Success(T value) => new(value, true);

        public new static Result<T> Failure(string errorMessage) => new(default!, false, errorMessage);
    }
}