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

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, 
                                                                                      ServerCallContext context, 
                                                                                      UnaryServerMethod<TRequest, TResponse> continuation)
        {
            await ValidateRequest(request);
            return await continuation(request, context);
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, 
                                                                                     IServerStreamWriter<TResponse> responseStream, 
                                                                                     ServerCallContext context, 
                                                                                     ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await ValidateRequest(request);
            await continuation(request, responseStream, context);
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, 
                                                                                     ServerCallContext context, 
                                                                                     ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                await ValidateRequest(message);
            }

            return await continuation(requestStream, context);
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, 
                                                                                     IServerStreamWriter<TResponse> responseStream, 
                                                                                     ServerCallContext context, 
                                                                                     DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                await ValidateRequest(message);
            }

            await continuation(requestStream, responseStream, context);
        }

        private async Task ValidateRequest<TRequest>(TRequest request) where TRequest : class
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
        }
    }
}