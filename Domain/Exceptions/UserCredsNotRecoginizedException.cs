namespace Domain.Exceptions
{
    public sealed class UserCredsNotRecoginizedException : NotFoundException
    {
        public UserCredsNotRecoginizedException()
            : base($"User credentials don't match.")
        {
        }
    }
}
