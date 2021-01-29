using System;
using Calzolari.Grpc.AspNetCore.Validation.SampleRpc;
using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Calzolari.Grpc.AspNetCore.Validation.Test.Integration
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

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty_ForServerStreaming()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                using var stream = client.SayHelloServerStreaming(new HelloRequest { Name = string.Empty });
                await foreach (var message in stream.ResponseStream.ReadAllAsync())
                {
                }
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
            Assert.Equal("Validation Error!", rpcException.Status.Detail);
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty_ForClientStreaming()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                using var request = client.SayHelloClientStreaming();

                await request.RequestStream.WriteAsync(new HelloRequest {Name = "Alice"});
                await request.RequestStream.WriteAsync(new HelloRequest {Name = "Bob"});
                await request.RequestStream.WriteAsync(new HelloRequest {Name = string.Empty});
                await request.RequestStream.CompleteAsync();

                var result = await request;
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
            Assert.Equal("Validation Error!", rpcException.Status.Detail);
        }

        [Fact]
        public async Task Should_NotThrow_When_Valid_ForClientStreaming()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());

            // When
            async Task<HelloReply> Action()
            {
                using var request = client.SayHelloClientStreaming();

                await request.RequestStream.WriteAsync(new HelloRequest { Name = "Alice" });
                await request.RequestStream.WriteAsync(new HelloRequest { Name = "Bob" });
                await request.RequestStream.WriteAsync(new HelloRequest { Name = "Charlie" });
                await request.RequestStream.CompleteAsync();

                return await request;
            }

            // Then
            var reply = await Action();
            Assert.Equal("Hello Alice, Bob, Charlie", reply.Message);
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty_ForDuplexStreaming()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                using var request = client.SayHelloDuplexStreaming();

                await request.RequestStream.WriteAsync(new HelloRequest { Name = "Alice" });
                await request.RequestStream.WriteAsync(new HelloRequest { Name = "Bob" });
                await request.RequestStream.WriteAsync(new HelloRequest { Name = string.Empty });
                await request.RequestStream.CompleteAsync();

                await foreach (var response in request.ResponseStream.ReadAllAsync())
                {
                }
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
            Assert.Equal("Validation Error!", rpcException.Status.Detail);
        }

        [Fact]
        public async Task Should_NotThrow_When_Valid_ForDuplexStreaming()
        {
            // Given
            var client = new Greeter.GreeterClient(_factory.CreateGrpcChannel());
            var names = new[] {"Alice", "Bob", "Charlie"};

            // When
            async Task<IList<string>> Action()
            {
                using var request = client.SayHelloDuplexStreaming();

                foreach (var name in names)
                {
                    await request.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }
                await request.RequestStream.CompleteAsync();

                var messages = new List<string>();
                await foreach (var response in request.ResponseStream.ReadAllAsync())
                {
                    messages.Add(response.Message);
                }

                return messages;
            }

            // Then
            var messages = await Action();
            Assert.Collection(messages,
                m => Assert.Equal("Hello Alice", m),
                m => Assert.Equal("Hello Bob", m),
                m => Assert.Equal("Hello Charlie", m)
            );
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