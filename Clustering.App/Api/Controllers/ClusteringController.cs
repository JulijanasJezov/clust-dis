using Clustering.App.Api.Shared.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/clustering")]
    public class ClusteringController : BaseController
    {
        [Route("test")]
        public IHttpActionResult GetAgeDepressionClusters()
        {
            var people = Db.People
                .Where(s => s.DepressionLevel.HasValue)
                .ToList();

            var dataPoints = new List<KMDataPoint>();

            foreach (var person in people)
            {
                var age = DateTime.Now.Year - person.DateOfBirth.Year;
                if (DateTime.Now.Month < person.DateOfBirth.Month || (DateTime.Now.Month == person.DateOfBirth.Month && DateTime.Now.Day < person.DateOfBirth.Day)) age--;

                var properties = new Dictionary<string, double>()
                {
                    {"x", age },
                    {"y", (double)person.DepressionLevel }
                };

                dataPoints.Add(new KMDataPoint(properties, person.FirstName, person.LastName));
            }

            var km = new KMAlgorithm();

            var clusterGroupAssignedData = km.ClusterData(dataPoints);

            var clusteredData = clusterGroupAssignedData
                .GroupBy(s => s.Cluster)
                .Select(s => new
                {
                    Name = "Cluster " + s.Key,
                    DataPoints = s
                });

            return ApiOk(clusteredData);
        }
    }
}