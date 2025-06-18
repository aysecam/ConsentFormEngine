namespace ConsentFormEngine.Core.Utilities
{
    public class Result
    {
        public bool Success { get; }
        public string Message { get; }

        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class DataResult<T> : Result
    {
        public T Data { get; }

        public DataResult(T data, bool success, string message)
            : base(success, message)
        {
            Data = data;
        }
    }
}
