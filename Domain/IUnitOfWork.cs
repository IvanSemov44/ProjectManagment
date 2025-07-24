using Domain.Repositories;

namespace Domain
{
    public interface IUnitOfWork
    {
        IProjectRepository ProjectRepository { get; }
        ISubtaskRepository SubtaskRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);  
    }
}
