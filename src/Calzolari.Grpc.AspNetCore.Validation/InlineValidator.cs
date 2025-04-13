using System;
using FluentValidation;

namespace Calzolari.Grpc.AspNetCore.Validation;

public class InlineValidator<T> : AbstractValidator<T>
{
    public InlineValidator(Action<AbstractValidator<T>> configureRules)
    {
        configureRules(this);
    }
}