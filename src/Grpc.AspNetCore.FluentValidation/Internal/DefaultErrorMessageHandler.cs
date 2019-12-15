using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Grpc.AspNetCore.FluentValidation.Internal
{
    internal class DefaultErrorMessageHandler : IValidatorErrorMessageHandler
    {
        public Task<string> HandleAsync(IList<ValidationFailure> failures)
        {
            var errors = failures
                .Select(f => $"Property {f.PropertyName} failed validation. Error was {f.ErrorMessage}")
                .ToList();

            return Task.FromResult(string.Join("\n", errors));
        }
    }
}