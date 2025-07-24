using Domain;
using Domain.Repositories;
using Infrastructure.Repositories;

namespace Infrastructure
{
    public sealed class UnitOfWork(ApplicationDbContext context)
        : IUnitOfWork
    {
        private readonly Lazy<IProjectRepository> _projectRepository = new(()
            => new ProjectRepository(context));
        private readonly Lazy<ISubtaskRepository> _subtaskRepository = new(()
            => new SubtastRepository(context));

        public IProjectRepository ProjectRepository => _projectRepository.Value;

        public ISubtaskRepository SubtaskRepository => _subtaskRepository.Value;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await context.SaveChangesAsync(cancellationToken);
    }
}
