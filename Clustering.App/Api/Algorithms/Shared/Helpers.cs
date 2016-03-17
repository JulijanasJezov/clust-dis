using Accord.Statistics.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.App.Api.Algorithms
{
    public static class Helpers
    {
        public static int MinIndex(double[] distances)
        {
            var indexOfMinDistance = 0;
            var minDistance = distances[0];

            for (int i = 0; i < distances.Length; ++i)
            {
                if (distances[i] < minDistance)
                {
                    minDistance = distances[i];
                    indexOfMinDistance = i;
                }
            }
            return indexOfMinDistance;
        }

        public static int MaxIndex(double[] distances)
        {
            var indexOfMaxDistance = 0;
            var maxDistance = distances[0];

            for (int i = 0; i < distances.Length; ++i)
            {
                if (distances[i] > maxDistance)
                {
                    maxDistance = distances[i];
                    indexOfMaxDistance = i;
                }
            }
            return indexOfMaxDistance;
        }

        public static double EuclideanDistance(KMDataPoint dataPoint, KMDataPoint mean)
        {
            var distance = 0.0;

            foreach (var property in dataPoint.Properties)
            {
                distance += Math.Pow(dataPoint.Properties[property.Key] - mean.Properties[property.Key], 2);
            }

            return Math.Sqrt(distance);
        }

        public static IDictionary<string, double> CalculatePropertiesSum(List<KMDataPoint> data, IDictionary<string, double> properties)
        {
            IDictionary<string, double> sumsOfProperties = new Dictionary<string, double>();

            foreach (var property in properties)
            {
                var sumOfProperty = 0.0;

                foreach (var dataPoint in data)
                {
                    sumOfProperty += dataPoint.Properties[property.Key];
                }

                sumsOfProperties.Add(property.Key, sumOfProperty);
            }

            return sumsOfProperties;
        }

        public static IDictionary<string, double> CalculatePropertiesMeans(IDictionary<string, double> propertiesSum, int total)
        {
            IDictionary<string, double> meansOfProperties = new Dictionary<string, double>();

            foreach (var property in propertiesSum)
            {
                var mean = property.Value / total;
                meansOfProperties.Add(property.Key, mean);
            }

            return meansOfProperties;
        }

        public static List<KMDataPoint> ComputePCA(ref List<KMDataPoint> normalizedDataToCluster, ref List<KMDataPoint> rawDataToCluster)
        {
            var numberOfDataPoints = normalizedDataToCluster.Count();
            var numberOfProperties = normalizedDataToCluster.First().Properties.Count();
            double[,] propertiesMatrix = new double[numberOfDataPoints, numberOfProperties];

            for (int dataPoint = 0; dataPoint < normalizedDataToCluster.Count; dataPoint++)
            {
                int prop = 0;

                foreach (var property in normalizedDataToCluster[dataPoint].Properties)
                {
                    propertiesMatrix[dataPoint, prop] = property.Value;
                    prop++;
                }

            }

            // Creates the Principal Component Analysis of the given matrix
            var pca = new PrincipalComponentAnalysis(propertiesMatrix, AnalysisMethod.Center);

            // Compute the Principal Component Analysis
            pca.Compute();

            double[,] components = pca.Transform(propertiesMatrix, 2);

            var dpIndex = 0;
            var compIndex = 0;

            foreach (var dataPoint in rawDataToCluster)
            {
                dataPoint.xValue = components[dpIndex, compIndex];
                dataPoint.yValue = components[dpIndex, compIndex + 1];

                dpIndex++;
            }

            return rawDataToCluster;
        }
    }
}