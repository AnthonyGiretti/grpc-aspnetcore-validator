using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Linq;
using System.Threading.Tasks;

namespace Calzolari.Grpc.AspNetCore.Validation.Internal
{
    internal class ValidationInterceptor : Interceptor
    {
        private readonly IValidatorLocator _locator;
        private readonly IValidatorErrorMessageHandler _handler;

        public ValidationInterceptor(IValidatorLocator locator, IValidatorErrorMessageHandler handler)
        {
            _locator = locator;
            _handler = handler;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_locator.TryGetValidator<TRequest>(out var validator))
            {
                var results = await validator.ValidateAsync(request);

                if (!results.IsValid && results.Errors.Any())
                {
                    var message = await _handler.HandleAsync(results.Errors);
                    var validationMetadata = results.Errors.ToValidationMetadata();
                    throw new RpcException(new Status(StatusCode.InvalidArgument, message), validationMetadata);
                }
            }
            return await continuation(request, context);
        }
    }
}