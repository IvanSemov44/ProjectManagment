using Application.Absrtactions;
using Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application
{
    public class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor contextAccessor) : ILinkService
    {
        public Link GenerateLink(string endpointName, string rel, string method, object? routeValues)
        {
            return new Link(
                linkGenerator.GetUriByName(contextAccessor.HttpContext!, endpointName, routeValues)!,
                rel,
                method);
        }
    }
}
