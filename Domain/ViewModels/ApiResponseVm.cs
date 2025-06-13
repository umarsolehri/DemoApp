namespace Domain.ViewModels
{
    public sealed class ApiResponseVm<T>
    {
        public ApiResponseVm() { }

        public T? Data { get; set; }
        public bool Status { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static ApiResponseVm<T> GetResponse(T? data, bool status, ResponseEnum type, string message = null, List<string> Errors = null)
        {
            ApiResponseVm<T> response = new ApiResponseVm<T>();

            response.Data = data;
            response.Status = status;

            switch (type)
            {
                case ResponseEnum.Success:
                    response.Message = string.IsNullOrEmpty(message) ? ResponseEnum.Success.GetDescription() : message;
                    response.Type = ResponseEnum.Success.ToString();
                    response.Icon = "success";
                    response.Errors = Errors ?? new List<string>();
                    break;
                case ResponseEnum.Duplicate:
                    response.Message = string.IsNullOrEmpty(message) ? ResponseEnum.Duplicate.GetDescription() : message;
                    response.Type = ResponseEnum.Duplicate.ToString();
                    response.Icon = "duplicate";
                    response.Errors = Errors ?? new List<string>();
                    break;
                case ResponseEnum.Failed:
                    response.Message = string.IsNullOrEmpty(message) ? ResponseEnum.Failed.GetDescription() : message;
                    response.Type = ResponseEnum.Failed.ToString();
                    response.Icon = "error";
                    response.Errors = Errors ?? new List<string>();
                    break;
                case ResponseEnum.NotFound:
                    response.Message = string.IsNullOrEmpty(message) ? ResponseEnum.NotFound.GetDescription() : message;
                    response.Type = ResponseEnum.NotFound.ToString();
                    response.Icon = "notFound";
                    response.Errors = Errors ?? new List<string>();
                    break;
                case ResponseEnum.Deleted:
                    response.Message = string.IsNullOrEmpty(message) ? ResponseEnum.Deleted.GetDescription() : message;
                    response.Type = ResponseEnum.Deleted.ToString();
                    response.Icon = "delete";
                    response.Errors = Errors ?? new List<string>();
                    break;
            }

            return response;
        }
    }

    public sealed class QueryListVm<T>
    {
        public IEnumerable<T> Result { get; set; } = Enumerable.Empty<T>();
        public int TotalRecords { get; set; }
    }
}
