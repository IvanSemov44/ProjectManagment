namespace Application.Absrtactions
{
    public interface IServiceManager
    {
        IProjectService ProjectService { get; }
        ISubtaskService SubtaskService { get; }
    }
}
