using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Grpc.AspNetCore.FluentValidation.SampleRpc;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Grpc.AspNetCore.FluentValidation.Test.Integration
{
    public class CustomMessageHandlerIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public CustomMessageHandlerIntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                {
                    services.AddInlineValidator<HelloRequest>(rules =>
                    {
                        rules.RuleFor(r => r.Name).NotEmpty();
                    });
                    services.AddSingleton<IValidatorErrorMessageHandler>(new CustomMessageHandler());
                    services.AddGrpcValidation();
                }));
        }

        private readonly WebApplicationFactory<Startup> _factory;

        [Fact] 
        public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                await client.SayHelloAsync(new HelloRequest {Name = string.Empty});
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
            Assert.Equal("Validation Error!", rpcException.Status.Detail);
        }

        class CustomMessageHandler : IValidatorErrorMessageHandler
        {
            public Task<string> HandleAsync(IList<ValidationFailure> failures)
            {
                return Task.FromResult("Validation Error!");
            }
        }
    }
}