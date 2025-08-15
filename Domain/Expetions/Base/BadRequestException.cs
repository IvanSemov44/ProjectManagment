namespace Domain.Expetions.Base
{
    public abstract class BadRequestException(string message)
        : Exception(message)
    {
    }
}
