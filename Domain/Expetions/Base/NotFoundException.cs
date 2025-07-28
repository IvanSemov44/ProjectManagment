namespace Domain.Expetions.Base
{
    public abstract class NotFoundException(string message)
        : Exception(message)
    {
    }
}
