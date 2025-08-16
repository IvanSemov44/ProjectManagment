using Domain.Models;

namespace Application.Absrtactions
{
    public interface IJwtService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
        string GenerateRefreshToken();
    }
}
