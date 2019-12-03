using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Grpc.AspNetCore.FluentValidation
{
    public class ValidationInterceptor : Interceptor
    {
        private readonly IValidatorLocator _locator;

        public ValidationInterceptor(IValidatorLocator locator)
        {
            _locator = locator;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_locator.TryGetValidator<TRequest>(out var validator))
            {
                var results = await validator.ValidateAsync(request);

                if (!results.IsValid)
                {
                    var errors = results
                        .Errors
                        .Select(f => $"Property {f.PropertyName} failed validation. Error was {f.ErrorMessage}")
                        .ToList();

                    var message = string.Join("\n", errors);

                    context.Status = new Status(StatusCode.InvalidArgument, message);
                    return ObjectCreator<TResponse>.Build();
                }
            }

            return await continuation(request, context);
        }
    }

    public static class ObjectCreator<T>
    {
        private static readonly Type Type = typeof(T);
        private static readonly Expression[] Ex = {Expression.New(Type)};
        private static readonly BlockExpression Block = Expression.Block(Type, Ex);
        private static readonly Func<T> Builder = Expression.Lambda<Func<T>>(Block).Compile();
        public static T Build()
        {
            return Builder();
        }
    }
}