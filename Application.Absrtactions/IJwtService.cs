using Domain;

namespace Application.Absrtactions
{
    public interface IJwtService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}
