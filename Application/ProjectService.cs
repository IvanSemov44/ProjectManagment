using Application.Absrtactions;
using Contracts;
using Domain;

namespace Application
{
    public sealed class ProjectService(ICustomLogger logger, IUnitOfWork unitOfWork)
        : IProjectService
    {
    }
}
