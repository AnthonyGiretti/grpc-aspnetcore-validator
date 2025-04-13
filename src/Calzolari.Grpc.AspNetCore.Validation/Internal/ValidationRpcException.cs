using Grpc.Core;
namespace Calzolari.Grpc.AspNetCore.Validation.Internal;

internal class ValidationRpcException : RpcException
{
    public ValidationRpcException(Status status, Metadata trailers) : base(status, trailers)
    {
    }
}
