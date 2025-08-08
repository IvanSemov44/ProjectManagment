using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Domain;

namespace Application
{
    public class ServiceManager(ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper, ILinkService linkService)
        : IServiceManager
    {
        private readonly Lazy<IProjectService> _projectService = new(()
            => new ProjectService(logger, unitOfWork, mapper, linkService));
        private readonly Lazy<ISubtaskService> _subtaskService = new(()
            => new SubtaskService(logger, unitOfWork, mapper, linkService));
        public IProjectService ProjectService => _projectService.Value;

        public ISubtaskService SubtaskService => _subtaskService.Value;
    }
}
