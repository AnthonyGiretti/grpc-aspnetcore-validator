using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Calzolari.Grpc.AspNetCore.Validation.Internal
{
    internal class DefaultErrorMessageHandler : IValidatorErrorMessageHandler
    {
        public Task<string> HandleAsync(IList<ValidationFailure> failures)
        {
            var errors = failures
                .Select(f => $"Property {f.PropertyName} failed validation.")
                .ToList();

            return Task.FromResult(string.Join("\n", errors));
        }
    }
}