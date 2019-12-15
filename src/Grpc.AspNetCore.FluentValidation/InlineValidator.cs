using System;
using FluentValidation;

namespace Grpc.AspNetCore.FluentValidation
{
    public class InlineValidator<T> : AbstractValidator<T>
    {
        public InlineValidator(Action<AbstractValidator<T>> configureRules)
        {
            configureRules(this);
        }
    }
}