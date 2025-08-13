namespace Contracts.Users
{
    public record RegisterUserRequest(
        string Email,
        string UserName,
        string FirstName,
        string LastName,
        string Password,
        string? PhoneNumber,
        IEnumerable<string> Roles);
}
