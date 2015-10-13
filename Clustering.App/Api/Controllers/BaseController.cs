using Clustering.App.Api.Shared.ApiResults;
using Clustering.Model;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    public class BaseController : ApiController
    {
        private ClusteringContext _context;
        protected ClusteringContext Db
        {
            get { return _context ?? (_context = new ClusteringContext()); }
        }

        public IHttpActionResult ApiOk(object data)
        {
            return new ApiOkResult(Request, data);
        }

        public IHttpActionResult ApiOk()
        {
            return new ApiOkResult(Request);
        }

        public IHttpActionResult ApiBadRequest(string message)
        {
            return new ApiBadRequestResult(Request, message);
        }

        public IHttpActionResult ApiBadRequest()
        {
            return new ApiBadRequestResult(Request);
        }
    }
}
