using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Calzolari.Grpc.AspNetCore.FluentValidation
{
    public interface IValidatorErrorMessageHandler
    {
        Task<string> HandleAsync(IList<ValidationFailure> failures);
    }
}