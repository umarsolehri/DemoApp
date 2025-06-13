namespace Application.Common.Validators
{
    public abstract class CommandValidator<TCommand, TDto> : AbstractValidator<TCommand>, IDisposable
        where TCommand : class
        where TDto : class
    {
        protected readonly IValidator<TDto> _dtoValidator;
        private bool _disposed;

        protected CommandValidator(IValidator<TDto> dtoValidator)
        {
            _dtoValidator = dtoValidator;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dtoValidator is IDisposable disposableValidator)
                    {
                        disposableValidator.Dispose();
                    }

                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CommandValidator()
        {
            Dispose(false);
        }
    }
}