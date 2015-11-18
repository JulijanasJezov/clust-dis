using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering.App.Api.Algorithms
{
    public class KMAlgorithm
    {
        private List<KMDataPoint> rawDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> normalizedDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> clusters = new List<KMDataPoint>();
        private int numberOfClusters = 5;

        public List<KMDataPoint> ClusterData(List<KMDataPoint> dataPoints, bool calculateSilhouette = false)
        {
            rawDataToCluster = dataPoints;

            for (int i = 0; i < numberOfClusters; i++)
            {
                clusters.Add(new KMDataPoint() { Cluster = i });
            }

            normalizedDataToCluster = Normalisation.NormaliseData(rawDataToCluster);

            InitializeCentroids();

            var maxIteration = dataPoints.Count * 10;
            var iteration = 0;

            while (iteration < maxIteration)
            {
                var nothingToUpdate = !CalculateClustersMeans() || !UpdateDataPointsClusters();

                if (nothingToUpdate) break;

                iteration++;
            }

            if (calculateSilhouette)
            {
                var silhouetteData = Validation.CalculateSilhouette(normalizedDataToCluster);

                for(var dp = 0; dp < silhouetteData.Count(); dp++)
                {
                    rawDataToCluster[dp].Silhouette = silhouetteData[dp].Silhouette;
                }
            }
            
            return rawDataToCluster;
        }
        
        private void InitializeCentroids()
        {
            Random random = new Random(numberOfClusters);
            for (int i = 0; i < numberOfClusters; ++i)
            {
                normalizedDataToCluster[i].Cluster = rawDataToCluster[i].Cluster = i;
            }

            for (int i = numberOfClusters; i < normalizedDataToCluster.Count; i++)
            {
                normalizedDataToCluster[i].Cluster = rawDataToCluster[i].Cluster = random.Next(0, numberOfClusters);
            }
        }

        private bool CalculateClustersMeans()
        {
            if (Helpers.HasEmptyCluster(normalizedDataToCluster)) return false;

            var groupToComputeMeans = normalizedDataToCluster
                .GroupBy(s => s.Cluster)
                .OrderBy(s => s.Key);

            var clusterIndex = 0;

            foreach (var item in groupToComputeMeans)
            {
                IDictionary<string, double> sumsOfProperties = new Dictionary<string, double>();

                foreach (var property in normalizedDataToCluster.First().Properties)
                {
                    var sumOfProperty = 0.0;

                    foreach (var value in item)
                    {
                        sumOfProperty += value.Properties[property.Key];
                    }

                    sumsOfProperties.Add(property.Key, sumOfProperty);
                }

                IDictionary<string, double> meansOfProperties = new Dictionary<string, double>();

                foreach (var property in rawDataToCluster.First().Properties)
                {
                    var mean = sumsOfProperties[property.Key] / rawDataToCluster.Count();
                    meansOfProperties.Add(property.Key, mean);
                }

                clusters[clusterIndex].Properties = meansOfProperties;
                clusterIndex++;
            }

            return true;
        }

        private bool UpdateDataPointsClusters()
        {
            var changed = false;

            var distances = new double[numberOfClusters];
            
            for (int dataPoint = 0; dataPoint < normalizedDataToCluster.Count; dataPoint++)
            {
                double sum = 0;
                double stdDev = 0;
                for (int clusterIndex = 0; clusterIndex < numberOfClusters; clusterIndex++)
                {
                    distances[clusterIndex] = Helpers.EuclideanDistance(normalizedDataToCluster[dataPoint], clusters[clusterIndex]);
                }

                var closestCluster = Helpers.MinIndex(distances);
                if (closestCluster != normalizedDataToCluster[dataPoint].Cluster)
                {
                    changed = true;
                    normalizedDataToCluster[dataPoint].Cluster = rawDataToCluster[dataPoint].Cluster = closestCluster;
                }

                foreach (var property in rawDataToCluster[dataPoint].Properties)
                {
                    sum += property.Value;
                }

                rawDataToCluster[dataPoint].Mean = sum / normalizedDataToCluster[dataPoint].Properties.Count();

                foreach (var property in rawDataToCluster[dataPoint].Properties)
                {
                    var eachDev = property.Value - rawDataToCluster[dataPoint].Mean;
                    stdDev += Math.Pow(eachDev, 2);
                }

                rawDataToCluster[dataPoint].StandardDeviation = Math.Sqrt(stdDev / normalizedDataToCluster[dataPoint].Properties.Count());

            }

            if (Helpers.HasEmptyCluster(normalizedDataToCluster)) return false;

            return changed;
        }
    }
}