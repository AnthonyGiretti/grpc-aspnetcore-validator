using System;
using System.Threading.Tasks;
using FluentValidation;
using Grpc.AspNetCore.FluentValidation.SampleRpc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Grpc.AspNetCore.FluentValidation.Test
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                {
                    services.AddValidator<HelloRequestValidator>();
                    services.AddValidatorLocator();
                }));
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty()
        {
            // Given
            using var httpClient = _factory.CreateClientForGrpc();
            var client = new Greeter.GreeterClient(GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = httpClient
            }));

            // When
            async Task Action()
            {
                await client.SayHelloAsync(new HelloRequest {Name = string.Empty});
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
        }

        [Fact]
        public async Task Should_ResponseMessage_When_MessageIsValid()
        {
            // Given
            using var httpClient = _factory.CreateClientForGrpc();
            var client = new Greeter.GreeterClient(GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = httpClient
            }));

            // When
            await client.SayHelloAsync(new HelloRequest
            {
                Name = "Not Empty Name"
            });

            // Then nothing happen.
        }
    }

    public class HelloRequestValidator : AbstractValidator<HelloRequest> 
    {
        public HelloRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty();
        }
    }
}