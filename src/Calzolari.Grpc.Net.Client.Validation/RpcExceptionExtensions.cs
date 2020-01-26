using Calzolari.Grpc.Domain;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;

namespace Calzolari.Grpc.Net.Client.Validation
{
    public static class RpcExceptionExtensions
    {
        public static IEnumerable<ValidationTrailers> GetValidationErrors(this RpcException exception)
        {
            var validationTrailer = exception.Trailers.FirstOrDefault(x => x.Key == "validation-errors-bin");

            if (validationTrailer != null)
            {
                return validationTrailer.ValueBytes.FromBytes<IEnumerable<ValidationTrailers>>();
            }
            return null;
        }
    }
}