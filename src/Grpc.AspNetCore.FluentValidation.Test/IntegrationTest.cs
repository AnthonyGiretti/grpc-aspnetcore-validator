using System;
using System.Threading.Tasks;
using FluentValidation;
using Grpc.AspNetCore.FluentValidation.SampleRpc;
using Grpc.Net.Client;
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
            _factory = factory;
            // .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            // {
            // services.AddValidator<HelloRequestValidator>();
            // services.AddValidatorLocator();
            // }));
        }

        [Fact]
        public async Task Test1()
        {
            // Given
            using var httpClient = _factory.CreateClient();
            var channel = GrpcChannel.ForAddress("https://localhost/", new GrpcChannelOptions
            {
                HttpClient = httpClient
            });
            var client = new Greeter.GreeterClient(channel);

            // When
            var response = await client.SayHelloAsync(new HelloRequest
            {
                Name = string.Empty
            });
            Assert.NotEmpty(response.Message);
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