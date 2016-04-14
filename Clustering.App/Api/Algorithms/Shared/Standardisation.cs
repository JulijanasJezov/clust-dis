using System.Collections.Generic;
using System.Linq;

namespace Clustering.App.Api.Algorithms
{
    public static class Standardisation
    {
        /// <summary>
        /// Returns a list of standartised data
        /// </summary>
        public static List<KMDataPoint> StandardiseData(ref List<KMDataPoint> rawData)
        {
            var standardisedData = new List<KMDataPoint>();

            var sumsOfProperties = Helpers.CalculatePropertiesSum(rawData, rawData.First().Properties);

            var meansOfProperties = Helpers.CalculatePropertiesMeans(sumsOfProperties, rawData.Count);

            var sdOfProperties = Helpers.CalculateStandardDeviation(rawData, meansOfProperties);

            foreach (var dataPoint in rawData)
            {
                var properties = new Dictionary<string, double>();
                foreach (var property in dataPoint.Properties)
                {
                    properties.Add(property.Key, (dataPoint.Properties[property.Key] - meansOfProperties[property.Key]) / sdOfProperties[property.Key]);
                }

                standardisedData.Add(new KMDataPoint(properties));
            }

            return standardisedData;
        }
    }
}