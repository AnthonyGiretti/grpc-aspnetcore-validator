using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.AspNetCore.FluentValidation.Internal
{
    internal class ServiceCollectionValidationProvider : IValidatorLocator
    {
        private readonly IServiceProvider _provider;

        public ServiceCollectionValidationProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public bool TryGetValidator<TRequest>(out IValidator<TRequest> result) where TRequest : class
        {
            return (result = _provider.GetService<IValidator<TRequest>>()) != null;
        }
    }
}