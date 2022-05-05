using Calzolari.Grpc.AspNetCore.Validation.SampleRpc;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Calzolari.Grpc.AspNetCore.Validation.Test.Integration
{
    public class GrpcRequestStateIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public GrpcRequestStateIntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                {
                    services.AddScoped(s =>
                    {
                        return new InMemoryDbSimulator(new List<UserRowSimulator>()
                        {
                            new UserRowSimulator()
                            {
                                UserName = "TestName",
                                Email = "TestEmail",
                            }
                        });
                    });
                    services.AddGrpcValidation();
                }));
        }

        private readonly WebApplicationFactory<Startup> _factory;

        [Fact]
        public async Task Should_ResponseMessage_When_UserNameAndEmailAreNotDuplicate()
        {
            // Given
            var client = new DataDuplicationChecker.DataDuplicationCheckerClient(_factory.CreateGrpcChannel());

            // When
            await client.CheckAsync(new CheckRequest
            {
                UserName = "New Name",
                Email = "New Email"
            });

            // Then nothing happen.
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_NameIsDuplicate()
        {
            // Given
            var client = new DataDuplicationChecker.DataDuplicationCheckerClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                await client.CheckAsync(new CheckRequest
                {
                    UserName = "TestName",
                    Email = "New Email"
                });
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_EmailIsDuplicate()
        {
            // Given
            var client = new DataDuplicationChecker.DataDuplicationCheckerClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                await client.CheckAsync(new CheckRequest
                {
                    UserName = "New Name",
                    Email = "TestEmail"
                });
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_BothNameAndEmailAreDuplicate()
        {
            // Given
            var client = new DataDuplicationChecker.DataDuplicationCheckerClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                await client.CheckAsync(new CheckRequest
                {
                    UserName = "TestName",
                    Email = "TestEmail"
                });
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
        }
    }
}