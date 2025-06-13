namespace Domain.Dtos
{
    public sealed class LoggedInUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string IsActive { get; set; }

        public List<string> Roles { get; set; }
    }
}
