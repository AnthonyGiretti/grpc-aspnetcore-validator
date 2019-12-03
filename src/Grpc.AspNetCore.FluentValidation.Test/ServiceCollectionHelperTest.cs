using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Grpc.AspNetCore.FluentValidation.Test
{
    public class ServiceCollectionHelperTest
    {
        [Fact]
        public void RegisterValidatorTest()
        {
            // Given
            var services = new ServiceCollection();

            // When
            services.AddValidator<TestValidator>();
            var provider = services.BuildServiceProvider();

            // Then
            provider.GetRequiredService<IValidator<TestMessage>>();
        }
    }


    public class TestValidator : AbstractValidator<TestMessage>
    {
    }

    public class TestMessage
    {
        public string Message { get; set; }
    }
}