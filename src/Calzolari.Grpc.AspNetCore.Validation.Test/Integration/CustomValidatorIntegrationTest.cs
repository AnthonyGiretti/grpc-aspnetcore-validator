using Calzolari.Grpc.AspNetCore.Validation.SampleRpc;
using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Xunit;

namespace Calzolari.Grpc.AspNetCore.Validation.Test.Integration
{
    public class CustomValidatorIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public CustomValidatorIntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                {
                    //services.AddValidator<HelloRequestValidator>();
                    services.AddValidators();
                    services.AddGrpcValidation();
                }));
        }

        private readonly WebApplicationFactory<Startup> _factory;

        [Fact]
        public async Task Should_ResponseMessage_When_MessageIsValid()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());

            // When
            await client.SayHelloAsync(new HelloRequest
            {
                Name = "Not Empty Name"
            });

            // Then nothing happen.
        }

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
        }

        public class HelloRequestValidator : AbstractValidator<HelloRequest>
        {
            public HelloRequestValidator()
            {
                RuleFor(request => request.Name).NotEmpty();
            }
        }
    }
}