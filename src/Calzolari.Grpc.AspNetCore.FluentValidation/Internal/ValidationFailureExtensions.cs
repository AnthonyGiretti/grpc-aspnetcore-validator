using Calzolari.Grpc.Domain;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace Calzolari.Grpc.AspNetCore.FluentValidation.Internal
{
    internal static class ValidationFailureExtensions
    {
        public static IEnumerable<ValidationTrailers> ToValidationTrailers(this IList<ValidationFailure> failures)
        {
            return failures.Select(x => new ValidationTrailers {
                PropertyName = x.PropertyName,
                AttemptedValue = x.AttemptedValue,
                ErrorMessage = x.ErrorMessage
            }).ToList();
        }
    }
}