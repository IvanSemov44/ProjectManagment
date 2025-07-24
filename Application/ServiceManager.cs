using Application.Absrtactions;
using Contracts;
using Domain;

namespace Application
{
    public class ServiceManager(ICustomLogger logger, IUnitOfWork unitOfWork)
        : IServiceManager
    {
        private readonly Lazy<IProjectService> _projectService = new(()
            => new ProjectService(logger, unitOfWork));
        private readonly Lazy<ISubtaskService> _subtaskService = new(()
            => new SubtaskService(logger, unitOfWork));
        public IProjectService ProjectService => _projectService.Value;

        public ISubtaskService SubtaskService => _subtaskService.Value;
    }
}
