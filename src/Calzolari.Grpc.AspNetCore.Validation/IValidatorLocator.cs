using FluentValidation;

namespace Calzolari.Grpc.AspNetCore.Validation;

public interface IValidatorLocator
{
    bool TryGetValidator<TRequest>(out IValidator<TRequest> result) where TRequest : class;
}