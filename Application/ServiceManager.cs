using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Domain;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application
{
    public class ServiceManager(
        ICustomLogger logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILinkService linkService,
        UserManager<User> userManager,
        IJwtService jwtService)
        : IServiceManager
    {
        private readonly Lazy<IProjectService> _projectService = new(()
            => new ProjectService(logger, unitOfWork, mapper, linkService));
        private readonly Lazy<ISubtaskService> _subtaskService = new(()
            => new SubtaskService(logger, unitOfWork, mapper, linkService));
        private readonly Lazy<IAuthenticationService> _authenticationService = new(()
            => new AuthenticationService(userManager, mapper, jwtService));

        public IProjectService ProjectService => _projectService.Value;

        public ISubtaskService SubtaskService => _subtaskService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
