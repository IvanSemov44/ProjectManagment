using Domain.Expetions.Base;

namespace Domain.Expetions
{
    public sealed class RefreshTokenBadRequestException()
        : BadRequestException("Invalid or expired refresh token.")
    {
    }
}
