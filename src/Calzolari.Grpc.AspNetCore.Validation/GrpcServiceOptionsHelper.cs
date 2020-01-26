using Calzolari.Grpc.AspNetCore.Validation.Internal;
using Grpc.AspNetCore.Server;

namespace Calzolari.Grpc.AspNetCore.Validation
{
    public static class GrpcServiceOptionsHelper
    {
        public static GrpcServiceOptions EnableMessageValidation(this GrpcServiceOptions options)
        {
            options.Interceptors.Add<ValidationInterceptor>();
            return options;
        }
    }
}