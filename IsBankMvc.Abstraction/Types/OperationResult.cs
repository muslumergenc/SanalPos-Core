using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Types
{
    public class OperationResult<T>
    {
        private OperationResult()
        {
            Status = OperationResultStatus.Pending;
        }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public OperationResultStatus Status { get; set; }
        public static OperationResult<T> Duplicate()
        {
            return new OperationResult<T>
            {
                Status = OperationResultStatus.Duplicate
            };
        }

        public static OperationResult<T> Failed(string? message = null)
        {
            return new OperationResult<T>
            {
                Message = message,
                Status = OperationResultStatus.Failed
            };
        }
        public static OperationResult<T> NotFound()
        {
            return new OperationResult<T>
            {
                Status = OperationResultStatus.NotFound
            };
        }

        public static OperationResult<T> Rejected(string? message = null, T? data = default)
        {
            return new OperationResult<T>
            {
                Message = message,
                Status = OperationResultStatus.Rejected,
                Data = data
            };
        }

        public static OperationResult<T> Success(T? data = default)
        {
            return new OperationResult<T>
            {
                Data = data,
                Status = OperationResultStatus.Success
            };
        }

        public static OperationResult<T> UnAuthorized()
        {
            return new OperationResult<T>
            {
                Status = OperationResultStatus.UnAuthorized
            };
        }

        public OperationResult<TX> To<TX>(TX? model = default)
        {
            return new OperationResult<TX>
            {
                Data = model,
                Message = Message,
                Status = Status
            };
        }
    }
}
