using Application.Absrtactions;
using Contracts;
using Domain;

namespace Application
{
    public sealed class SubtaskService(ICustomLogger logger, IUnitOfWork unitOfWork)
        : ISubtaskService
    {
    }
}
