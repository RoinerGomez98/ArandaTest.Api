namespace ArandaTest.Application.DTOs
{
    public class UsersDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? Token { get; set; }
        public DateTime Expires { get; set; }
    }

    public class UsersFilterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserClaims
    {
        public string? Document { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Id { get; set; }
    }
}
