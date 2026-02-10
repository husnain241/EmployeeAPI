namespace EmployeeAPI.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;

        // Helper methods for easy use
        public static Result<T> Success(T data, string message = "Success")
            => new Result<T> { IsSuccess = true, Data = data, Message = message };

        public static Result<T> Failure(string message)
            => new Result<T> { IsSuccess = false, Message = message };
    }
}
