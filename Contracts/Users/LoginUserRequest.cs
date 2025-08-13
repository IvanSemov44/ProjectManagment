namespace Contracts.Users
{
    public record LoginUserRequest(
        string UserNameOrEmail,
        string Password);
}
