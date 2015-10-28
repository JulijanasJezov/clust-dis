using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/diseases")]
    public class DiseaseController : BaseController
    {
        [Route("")]
        public IHttpActionResult GetDiseases()
        {
            var diseases = Db.Diseases.ToList();

            return ApiOk(diseases);
        }

        [Route("{diseaseId}/properties")]
        public IHttpActionResult GetDiseaseProperties(int diseaseId)
        {
            var diseaseProperties = Db.DiseaseProperties
                .Where(s => s.DiseaseId == diseaseId);

            if (!diseaseProperties.Any())
            {
                return ApiNotFound();
            }

            return ApiOk(diseaseProperties);
        }
    }
}