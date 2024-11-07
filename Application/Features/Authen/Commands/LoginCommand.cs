using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Security;
using Application.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Application.Features.Products.Commands
{
    public class LoginCommand
    {
        public record LoginCommandRequest(string UserName, string Password) : IRequest<LoginCommandResponse>;
        public record LoginCommandResponse();
        internal sealed class LoginCommandValidator : AbstractValidator<LoginCommandRequest>
        {
            public LoginCommandValidator()
            {
                RuleFor(v => v.UserName)
                    .NotEmpty();
                RuleFor(v => v.Password)
                    .NotEmpty();
            }
        }
        public class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("login", Handle)
                    .WithTags("login")
                    .WithRequestValidation<LoginCommandRequest>()
                    .Produces<int>(StatusCodes.Status201Created)
                    .Produces(StatusCodes.Status400BadRequest);

            }
            private static async Task<Ok<LoginCommandResponse>> Handle([FromBody]LoginCommandRequest request, IUserRepository userRepository, IPasswordHasher passwordHasher, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
            {
                Domain.Entities.User? user = await userRepository.GetByUserName(request.UserName);
                if (user is null)
                {
                    throw new ValidationException("The user was not found");
                }
                bool verified = passwordHasher.Verify(request.UserName, request.Password);
                if (!verified)
                {
                    throw new ValidationException("The password is incorrect");
                }
                var response = new LoginCommandResponse();
                return TypedResults.Ok(response);
            }
        }

        //public class LoginCommandHandler(ILogger<LoginCommandHandler> logger, IMapper mapper, IUserRepository userRepository, IPasswordHasher passwordHasher) : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
        //{
        //    private readonly ILogger<LoginCommandHandler> _logger = logger;
        //    private readonly IUserRepository _userRepository = userRepository;
        //    private readonly IMapper _mapper = mapper;
        //    public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        //    {
        //        Domain.Entities.User? user = await _userRepository.GetByUserName(request.UserName);
        //        if(user is null)
        //        {
        //            throw new ValidationException("The user was not found");
        //        }
        //        bool verified = passwordHasher.Verify(request.UserName, request.Password);
        //        if (!verified)
        //        {
        //            throw new ValidationException("The password is incorrect");
        //        }
        //        return new LoginCommandResponse();
        //    }
        //}
    }
}
