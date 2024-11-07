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
    public class GetProductById
    {
        public record GetProductByIdQueryRequest(int Id) : IRequest<GetProductByIdQueryResponse>;
        public record GetProductByIdQueryResponse(Product product);
        public class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("product/{id}", async ([AsParameters] int id, IMediator mediator) =>
                {
                    return await mediator.Send(new GetProductByIdQueryRequest(id));
                })
                .WithSummary("Get Product By Id")
                .WithTags(nameof(GetProductById))
                .Produces<int>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            }
        }

        public class GetProductByIdQueryHandler(ILogger<GetProductByIdQueryHandler> logger, IProductRepository productRepository) : IRequestHandler<GetProductByIdQueryRequest, GetProductByIdQueryResponse>
        {
            private readonly ILogger<GetProductByIdQueryHandler> _logger = logger;
            private readonly IProductRepository _productRepository = productRepository;
            public async Task<GetProductByIdQueryResponse> Handle(GetProductByIdQueryRequest request, CancellationToken cancellationToken)
            {
                var productInfo = await _productRepository.GetById(request.Id);
                var response = new GetProductByIdQueryResponse(productInfo);
                return response;
            }
        }
    }
}
