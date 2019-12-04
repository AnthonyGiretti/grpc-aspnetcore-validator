using FluentValidation;

namespace Grpc.AspNetCore.FluentValidation
{
    public interface IValidatorLocator
    {
        bool TryGetValidator<TRequest>(out IValidator<TRequest> result) where TRequest : class;
    }
}