using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.App.Api.Algorithms
{
    public static class Normalisation
    {
        // Gaussian normalisation
        public static List<KMDataPoint> NormaliseData(ref List<KMDataPoint> rawData)
        {
            List<KMDataPoint> normalizedData = new List<KMDataPoint>();

            IDictionary<string, double> sumsOfProperties = Helpers.CalculatePropertiesSum(rawData, rawData.First().Properties);

            IDictionary<string, double> meansOfProperties = Helpers.CalculatePropertiesMeans(sumsOfProperties, rawData.Count);

            IDictionary<string, double> sumsPowOfProperties = new Dictionary<string, double>();

            foreach (var property in rawData.First().Properties)
            {
                var sumPowOfProperty = 0.0;

                foreach (var dataPoint in rawData)
                {
                    sumPowOfProperty += Math.Pow(dataPoint.Properties[property.Key] - meansOfProperties[property.Key], 2);
                }

                sumsPowOfProperties.Add(property.Key, sumPowOfProperty);
            }

            IDictionary<string, double> sdOfProperties = new Dictionary<string, double>();

            foreach (var property in rawData.First().Properties)
            {
                var sd = sumsPowOfProperties[property.Key] / rawData.Count();
                sdOfProperties.Add(property.Key, sd);
            }

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