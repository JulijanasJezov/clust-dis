using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.App.Api.Algorithms
{
    public static class Normalisation
    {
        // Data standardization
        public static List<KMDataPoint> NormaliseData(ref List<KMDataPoint> rawData)
        {
            List<KMDataPoint> normalizedData = new List<KMDataPoint>();

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

                normalizedData.Add(new KMDataPoint(properties));
            }

            return normalizedData;
        }
    }
}