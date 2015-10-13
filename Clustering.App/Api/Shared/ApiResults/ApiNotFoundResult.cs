using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Clustering.App.Api.Shared.ApiResults
{
    public class ApiNotFoundResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _request;
        private readonly string _message;

        public ApiNotFoundResult(HttpRequestMessage request)
            : this(request, null)
        {
        }

        public ApiNotFoundResult(HttpRequestMessage request, string message)
        {
            _request = request;
            _message = message;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = (_message == null) ?
                _request.CreateResponse(HttpStatusCode.NotFound, new { status = HttpStatusCode.NotFound }) :
                _request.CreateResponse(HttpStatusCode.NotFound, new { message = _message, status = HttpStatusCode.NotFound });

            return await Task.FromResult(response);
        }
    }
}