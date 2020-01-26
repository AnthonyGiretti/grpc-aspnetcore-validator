using FluentValidation.Results;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Linq;
using System.Threading.Tasks;

namespace Calzolari.Grpc.AspNetCore.FluentValidation.Internal
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
                    results.Errors.Add(new ValidationFailure("field", "value"));
                    results.Errors.ToList().ForEach(error =>
                    {
                        context.ResponseTrailers.Add(new Metadata.Entry("ValidationErrors", error.ErrorMessage + " atemptedValue " + error.AttemptedValue));
                    });
                    throw new RpcException(new Status(StatusCode.InvalidArgument, message));
                }
            }

            return await continuation(request, context);
        }
    }
}
