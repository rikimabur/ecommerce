using Application.Common.Interfaces;
using Application.Domain.Entities;
using Application.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands
{
    public class GetProducts
    {
        public record GetProductsQueryRequest() : IRequest<GetProductsQueryResponse>;
        public record GetProductsQueryResponse(IEnumerable<Product> products);
        public class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("products", async (IMediator mediator) =>
                {
                    return await mediator.Send(new GetProductsQueryRequest());
                })
                .WithTags("Products")
                .Produces<int>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            }
        }

        public class GetProductsQueryHandler(ILogger<GetProductsQueryHandler> logger, IProductRepository productRepository) : IRequestHandler<GetProductsQueryRequest, GetProductsQueryResponse>
        {
            private readonly ILogger<GetProductsQueryHandler> _logger = logger;
            private readonly IProductRepository _productRepository = productRepository;
            public async Task<GetProductsQueryResponse> Handle(GetProductsQueryRequest request, CancellationToken cancellationToken)
            {
                var products = await _productRepository.GetAll();
                var response = new GetProductsQueryResponse(products);
                return response;
            }
        }
    }
}
