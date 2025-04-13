using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Calzolari.Grpc.AspNetCore.Validation.Internal
{
    internal class DefaultErrorMessageHandler : IValidatorErrorMessageHandler
    {
        public Task<string> HandleAsync(IList<ValidationFailure> failures)
        {
            var errors = failures
                .Select(GetErrorMessage)
                .ToList();

            return Task.FromResult(string.Join("\n", errors));
        }

        private string GetErrorMessage(ValidationFailure failure)
        {
            var message = new StringBuilder();
            message.Append($"Property {failure.PropertyName} failed validation.");

            if (!string.IsNullOrEmpty(failure.ErrorCode))
                message.Append($" Error code was: {failure.ErrorCode}");

            if (!string.IsNullOrEmpty(failure.ErrorMessage))
                message.Append($" Error was: {failure.ErrorMessage}");

            return message.ToString();
        }
    }
}