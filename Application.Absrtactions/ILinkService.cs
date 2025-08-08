using Contracts.Common;

namespace Application.Absrtactions
{
    public interface ILinkService
    {
        Link GenerateLink(string endpointName, string rel, string method, object? routeValues);
    }
}
