using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Clustering.App.Api.Shared.ApiResults
{
    public class ApiBadRequestResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _request;
        private readonly string _message;
        private readonly IDictionary<string, string> _errors;

        public ApiBadRequestResult(HttpRequestMessage request)
        {
            _request = request;
        }

        public ApiBadRequestResult(HttpRequestMessage request, string message)
        {
            _request = request;
            _message = message;
        }

        public ApiBadRequestResult(HttpRequestMessage request, IDictionary<string, string> errors)
        {
            _request = request;
            _errors = errors;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            object responseData;

            if (_errors != null)
            {
                responseData = new { status = HttpStatusCode.BadRequest, errors = _errors };
            }
            else if (_message != null)
            {
                responseData = new { status = HttpStatusCode.BadRequest, message = _message };
            }
            else
            {
                responseData = new { status = HttpStatusCode.BadRequest };
            }

            var response = _request.CreateResponse(HttpStatusCode.BadRequest, responseData);

            return await Task.FromResult(response);
        }
    }
}