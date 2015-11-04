using Clustering.App.Api.Shared.Algorithms;
using Clustering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/cluster")]
    public class ClusteringController : BaseController
    {
        [Route("")]
        public IHttpActionResult PostClusterData(ClusterDataApiModel clusterData)
        {
            var people = Db.People
                .Where(s => s.PersonDiseaseProperties
                            .Where(a => a.DiseaseProperty.DiseaseId == clusterData.ClusterDiseaseId)
                            .Any())
                .ToList();

            var clusterProperties = Db.DiseaseProperties
                .Where(s => clusterData.ClusterPropertyIds.Contains(s.DiseasePropertyId));

            var clusterPropertiesCount = clusterData.IncludeAge ? clusterProperties.Count() + 1 : clusterProperties.Count();

            var dataPoints = new List<KMDataPoint>();

            foreach (var person in people)
            {
                var properties = new Dictionary<string, double>();

                if (clusterData.IncludeAge)
                {
                    var age = DateTime.Now.Year - person.DateOfBirth.Year;
                    if (DateTime.Now.Month < person.DateOfBirth.Month || (DateTime.Now.Month == person.DateOfBirth.Month && DateTime.Now.Day < person.DateOfBirth.Day)) age--;

                    properties.Add("Age", age);
                }

                foreach (var property in clusterProperties)
                {
                    int? score = person.PersonDiseaseProperties
                        .Where(s => s.DiseasePropertyId == property.DiseasePropertyId)
                        .Select(s => s.Score)
                        .SingleOrDefault();

                    if (score != null)
                    {
                        properties.Add(property.Name, score.Value);
                    }
                }

                // Ignore people with null properties
                if (properties.Count() == clusterPropertiesCount)
                {
                    dataPoints.Add(new KMDataPoint(properties, person.FirstName, person.LastName));
                }

            }

            var km = new KMAlgorithm();

            var clusterGroupAssignedData = km.ClusterData(dataPoints, clusterData.CalculateSilhouette);

            var clusteredData = clusterGroupAssignedData
                .GroupBy(s => s.Cluster)
                .Select(s => new ClusterApiModel
                {
                    Name = "Cluster " + s.Key,
                    DataPoints = s.ToList(),
                    Total = s.Count()
                })
                .ToList();

            clusteredData = CalculatePropertiesRange(clusteredData);

            return ApiOk(clusteredData);
        }

        private List<ClusterApiModel> CalculatePropertiesRange(List<ClusterApiModel> clusters)
        {
            foreach (var cluster in clusters)
            {
                cluster.PropertiesDetails = new List<PropertyDetails>();

                foreach(var property in cluster.DataPoints.First().Properties)
                {
                    var name = property.Key;
                    var min = cluster.DataPoints.Min(s => s.Properties[property.Key]);
                    var max = cluster.DataPoints.Max(s => s.Properties[property.Key]);
                    var avg = cluster.DataPoints.Sum(s => s.Properties[property.Key]) / cluster.DataPoints.Count();

                    var propertyRange = new PropertyDetails
                    {
                        Name = name,
                        MinValue = min,
                        MaxValue = max,
                        AverageValue = Math.Ceiling(avg * 100) / 100    // Round up to .00
                    };

                    cluster.PropertiesDetails.Add(propertyRange);
                }
            }
            return clusters;
        }
    }
}