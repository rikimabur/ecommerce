using Microsoft.AspNetCore.Routing;

namespace Application.Common.Interfaces
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
