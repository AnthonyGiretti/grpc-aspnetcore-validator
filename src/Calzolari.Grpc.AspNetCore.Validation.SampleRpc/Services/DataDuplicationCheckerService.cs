using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calzolari.Grpc.AspNetCore.Validation.SampleRpc
{
    public class DataDuplicationCheckerService : DataDuplicationChecker.DataDuplicationCheckerBase
    {
        private readonly InMemoryDbSimulator _inMemoryDb;
        private readonly GrpcRequestState _requestState;

        public DataDuplicationCheckerService(InMemoryDbSimulator inMemoryDb, GrpcRequestState requestState)
        {
            _inMemoryDb = inMemoryDb;
            _requestState = requestState;
        }


        public override async Task<CheckReply> Check(CheckRequest request, ServerCallContext context)
        {
            if (_inMemoryDb.Users.Any(u => u.UserName == request.UserName))
                _requestState.AddError(nameof(request.UserName), "The User Name already exists.");

            if (_inMemoryDb.Users.Any(u => u.Email == request.Email))
                _requestState.AddError(nameof(request.Email), "The Email already exists.");

            await _requestState.ThrowIfNotValidAsync();

            return new CheckReply
            {
                Message = $"Hello {request.UserName}, Email: {request.Email}."
            };
        }
    }

    public class InMemoryDbSimulator
    {
        public InMemoryDbSimulator(IEnumerable<UserRowSimulator> users)
        {
            Users = users.ToList();
        }

        public List<UserRowSimulator> Users { get; }
    }

    public class UserRowSimulator
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}