using Clustering.Model.Seed.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/seed")]
    public class SeedController : BaseController
    {
        [Route("")]
        public IHttpActionResult GetSeedDemoData()
        {
            if (Db.People.Any()) return ApiBadRequest("Database is not empty");

            Task.WaitAll(
                SeedTasks.SeedPeople());

            return ApiOk("Demo data has been seeded");
        }
    }
}