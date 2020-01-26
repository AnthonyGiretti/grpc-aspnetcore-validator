using System;

namespace Calzolari.Grpc.AspNetCore.FluentValidation.Internal
{
    [Serializable]
    internal class ValidationTrailers
    {
        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }

        public object AttemptedValue { get; set; }
    }
}