namespace Contracts
{
    public interface ICustomLogger
    {
        void LogDebug(string message);
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
