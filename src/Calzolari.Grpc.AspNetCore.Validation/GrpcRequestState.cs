using Calzolari.Grpc.AspNetCore.Validation.Internal;
using FluentValidation.Results;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calzolari.Grpc.AspNetCore.Validation
{
    /// <summary>
    /// Provides the ability to record custom validation errors through the lifecycle of the gRPC request.
    /// </summary>
    public class GrpcRequestState
    {
        private readonly IValidatorErrorMessageHandler _handler;
        private readonly List<ValidationFailure> _failures = new List<ValidationFailure>();


        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcRequestState"/> class.
        /// </summary>
        public GrpcRequestState(IValidatorErrorMessageHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Gets a value that indicates whether the current <see cref="GrpcRequestState"/> instance has errors or not.
        /// </summary>
        public bool IsValid => _failures.Count == 0;


        /// <summary>
        /// Adds the specified <paramref name="errorMessage"/> to the <see cref="ValidationFailure.ErrorMessage"/> instance
        /// that is associated with the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The property name of the <see cref="ValidationFailure.PropertyName"/> to add errors to.</param>
        /// <param name="errorMessage">The error message to add.</param>
        public void AddError(string propertyName, string errorMessage)
        {
            _failures.Add(new ValidationFailure(propertyName, errorMessage));
        }


        /// <summary>
        /// Throws the <see cref="RpcException"/> associated with the errors recorded using <see cref="AddError(string, string)"/> method, no exception will be thrown if there are no recorded errors.
        /// </summary>
        /// <param name="statusCode">The status code to use in the returned <see cref="RpcException"/>.</param>
        public async Task ThrowIfNotValidAsync(StatusCode statusCode = StatusCode.InvalidArgument)
        {
            if (IsValid)
                return;

            var message = await _handler.HandleAsync(_failures);

            var validationMetadata = _failures.ToValidationMetadata();

            _failures.Clear();

            throw new RpcException(new Status(statusCode, message), validationMetadata);
        }


        /// <summary>
        /// Removes all failures from this instance of <see cref="GrpcRequestState"/>.
        /// </summary>
        public void Clear() => _failures.Clear();
    }
}
