using System;
using FluentValidation;

namespace Calzolari.Grpc.AspNetCore.FluentValidation
{
    public class InlineValidator<T> : AbstractValidator<T>
    {
        public InlineValidator(Action<AbstractValidator<T>> configureRules)
        {
            configureRules(this);
        }
    }
}