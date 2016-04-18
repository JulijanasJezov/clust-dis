using Clustering.Model.Seed.Tasks;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/seed")]
    public class SeedController : BaseController
    {
        /// <summary>
        /// Seeds the database with sample data if the database is empty
        /// </summary>
        [Route("")]
        public IHttpActionResult GetSeedDemoData()
        {
            if (Db.People.Any()) return ApiBadRequest("Database is not empty");

            Task.WaitAll(
                SeedTasks.SeedPeople());
            Task.WaitAll(
                SeedTasks.SeedDiseases());
            Task.WaitAll(
                SeedTasks.SeedDiseaseProperties());
            Task.WaitAll(
                SeedTasks.SeedPersonDiseaseProperties());

            return ApiOk("Demo data has been seeded");
        }
    }
}