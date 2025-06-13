using Domain.ViewModels;

namespace API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler, IDisposable
    {
        private readonly IWebHostEnvironment _environment;
        private bool _disposed;

        public GlobalExceptionHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = new ApiResponseVm<object>();
            var statusCode = HttpStatusCode.InternalServerError;
            var isDevelopment = _environment.IsDevelopment();

            switch (exception)
            {
                case ValidationException validationException:
                    var validationErrors = validationException.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();

                    response = ApiResponseVm<object>.GetResponse(
                        null,
                        false,
                        ResponseEnum.Failed,
                        "Validation failed",
                        validationErrors
                    );
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case BadRequestException badRequestException:
                    response = ApiResponseVm<object>.GetResponse(
                        null,
                        false,
                        ResponseEnum.Failed,
                        badRequestException.Message,
                        GetErrorDetails(badRequestException, isDevelopment)
                    );
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case NotFoundException notFoundException:
                    response = ApiResponseVm<object>.GetResponse(
                        null,
                        false,
                        ResponseEnum.NotFound,
                        notFoundException.Message,
                        GetErrorDetails(notFoundException, isDevelopment)
                    );
                    statusCode = HttpStatusCode.NotFound;
                    break;

                //case Domain.Exceptions.ApplicationException appException:
                //    response = ApiResponseVm<object>.GetResponse(
                //        null,
                //        false,
                //        ResponseEnum.Failed,
                //        appException.Message,
                //        GetErrorDetails(appException, isDevelopment)
                //    );
                //    statusCode = HttpStatusCode.BadRequest;
                //    break;

                default:
                    response = ApiResponseVm<object>.GetResponse(
                        null,
                        false,
                        ResponseEnum.Failed,
                        isDevelopment ? exception.Message : "An unexpected error occurred",
                        GetErrorDetails(exception, isDevelopment)
                    );
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }

        private List<string> GetErrorDetails(Exception exception, bool isDevelopment)
        {
            var errors = new List<string>();

            if (isDevelopment)
            {
                errors.Add(exception.GetType().Name);
                errors.Add($"Stack Trace: {exception.StackTrace}");

                if (exception.InnerException != null)
                {
                    errors.Add($"Inner Exception: {exception.InnerException.GetType().Name}");
                    errors.Add($"Inner Exception Message: {exception.InnerException.Message}");
                    errors.Add($"Inner Exception Stack Trace: {exception.InnerException.StackTrace}");
                }

                errors.Add($"Source: {exception.Source}");
                if (exception.TargetSite != null)
                {
                    errors.Add($"Target Site: {exception.TargetSite.Name}");
                }
            }
            else
            {
                errors.Add(exception.Message);
            }

            return errors;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GlobalExceptionHandler()
        {
            Dispose(false);
        }
    }
}
