using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clustering.App.Api.Algorithms
{
    public class KMAlgorithm
    {
        private List<KMDataPoint> rawDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> normalizedDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> clusters = new List<KMDataPoint>();
        private int numberOfClusters;

        public List<KMDataPoint> ClusterData(List<KMDataPoint> dataPoints, int numOfClusters, bool calculateSilhouette = false)
        {
            rawDataToCluster = dataPoints;

            numberOfClusters = numOfClusters;

            for (int i = 0; i < numberOfClusters; i++)
            {
                clusters.Add(new KMDataPoint() { Cluster = i });
            }

            normalizedDataToCluster = Normalisation.NormaliseData(ref rawDataToCluster);

            InitializeCentroids();

            var maxIteration = dataPoints.Count * 10;
            var iteration = 0;

            while (iteration < maxIteration)
            {
                var clustersUpdated = CalculateClustersMeans() && UpdateDataPointsClusters();

                if (!clustersUpdated) break;

                iteration++;
            }

            if (calculateSilhouette)
            {
                var silhouetteData = Validation.CalculateSilhouette(ref normalizedDataToCluster);

                for(var dp = 0; dp < silhouetteData.Count(); dp++)
                {
                    rawDataToCluster[dp].Silhouette = silhouetteData[dp].Silhouette;
                }
            }

            rawDataToCluster = Helpers.ComputePCA(ref normalizedDataToCluster, ref rawDataToCluster);

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
            if (Helpers.HasEmptyCluster(ref normalizedDataToCluster)) return false;

            var groupToComputeMeans = normalizedDataToCluster
                .GroupBy(s => s.Cluster)
                .OrderBy(s => s.Key);
            
            Parallel.ForEach(groupToComputeMeans,
                (item, pls, clusterIndex) =>
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

                    clusters[(int)clusterIndex].Properties = meansOfProperties;
                });

            return true;
        }

        private bool UpdateDataPointsClusters()
        {
            var changed = false;
            
            Parallel.For(0, normalizedDataToCluster.Count,
                dataPoint =>
                {
                    var distances = new double[numberOfClusters];

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
                });

            if (Helpers.HasEmptyCluster(ref normalizedDataToCluster)) return false;

            return changed;
        }
    }
}