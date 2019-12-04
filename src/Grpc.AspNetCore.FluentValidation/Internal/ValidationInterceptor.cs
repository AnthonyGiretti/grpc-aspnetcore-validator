using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Grpc.AspNetCore.FluentValidation.Internal
{
    internal class ValidationInterceptor : Interceptor
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
                    return ObjectCreator<TResponse>.Empty;
                }
            }

            return await continuation(request, context);
        }
    }
}