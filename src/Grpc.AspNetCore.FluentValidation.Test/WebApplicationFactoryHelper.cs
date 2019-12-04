using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.AspNetCore.FluentValidation.SampleRpc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Grpc.AspNetCore.FluentValidation.Test
{
    public static class WebApplicationFactoryHelper
    {
        public static HttpClient CreateClientForGrpc(this WebApplicationFactory<Startup> factory)
        {
            return factory.CreateDefaultClient(new ResponseVersionHandler());
        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;

                return response;
            }
        }
    }
}