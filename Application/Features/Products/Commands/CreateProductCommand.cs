using Application.Common.Interfaces;
using Application.Domain.Entities;
using Application.Infrastructure.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands
{
    public class CreateProductCommand
    {
        public record CreateProductCommandRequest(string Name, double Price, string Description) : IRequest<CreateProductCommandResponse>;
        public record CreateProductCommandResponse(int Id);
        internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommandRequest>
        {
            public CreateProductCommandValidator()
            {
                RuleFor(v => v.Name)
                    .MaximumLength(200)
                    .NotEmpty();
                RuleFor(v => v.Price)
                    .GreaterThan(0)
                    .NotNull();
            }
        }
        public class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("product", async ([FromBody] CreateProductCommandRequest request, IMediator mediator) =>
                {
                    return await mediator.Send(request);
                })
                .WithTags("Product")
                .Produces<int>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            }
        }

        public class CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, IMapper mapper, IProductRepository productRepository) : IRequestHandler<CreateProductCommandRequest, CreateProductCommandResponse>
        {
            private readonly ILogger<CreateProductCommandHandler> _logger = logger;
            private readonly IProductRepository _productRepository = productRepository;
            private readonly IMapper _mapper = mapper;
            public async Task<CreateProductCommandResponse> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
            {
                var productEntity = _mapper.Map<Product>(request);
                var productId = await _productRepository.Add(productEntity);
                var response = new CreateProductCommandResponse(productId);
                return response;
            }
        }
    }
}
