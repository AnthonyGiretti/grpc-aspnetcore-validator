using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Calzolari.Grpc.AspNetCore.Validation
{
    public interface IValidatorErrorMessageHandler
    {
        Task<string> HandleAsync(IList<ValidationFailure> failures);
    }
}