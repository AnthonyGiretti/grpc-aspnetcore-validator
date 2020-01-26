using FluentValidation.Results;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;

namespace Calzolari.Grpc.AspNetCore.Validation.Internal
{
    internal static class MetadataExtensions
    {
        public static Metadata ToValidationMetadata(this IList<ValidationFailure> failures)
        {
            var metadata = new Metadata();
            if (failures.Any())
            {
                metadata.Add(new Metadata.Entry("validation-errors-bin", failures.ToValidationTrailers().ToBytes()));
            }
            return metadata;
        }
    }
}