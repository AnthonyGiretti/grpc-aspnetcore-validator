using Grpc.AspNetCore.FluentValidation.Internal;
using Grpc.AspNetCore.Server;

namespace Grpc.AspNetCore.FluentValidation
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