﻿using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Filters
{
    public class ValidationFilter<TRequest> : IEndpointFilter
    {
        private readonly IValidator<TRequest> validator;

        public ValidationFilter(IValidator<TRequest> validator)
        {
            this.validator = validator;
        }

        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var request = context.Arguments.OfType<TRequest>().First();

            var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                return TypedResults.ValidationProblem(result.ToDictionary());
            }

            return await next(context);
        }
    }

    public static class ValidationExtensions
    {
        public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        {
            return builder.AddEndpointFilter<ValidationFilter<TRequest>>()
                .ProducesValidationProblem();
        }
    }
}
