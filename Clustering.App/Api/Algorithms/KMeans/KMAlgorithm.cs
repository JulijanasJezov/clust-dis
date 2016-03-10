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
        private int assignEmptyClustersRetries = 0;

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

            var maxIteration = dataPoints.Count;
            var iteration = 0;

            while (iteration < maxIteration)
            {
                var clustersUpdated = CalculateClustersMeans() && UpdateDataPointsClusters();

                if (!clustersUpdated) break;

                iteration++;
            }
            Validation.CalculatePurity(rawDataToCluster);
            if (calculateSilhouette)
            {
                var silhouetteData = Validation.CalculateSilhouette(normalizedDataToCluster);

                for (var dp = 0; dp < silhouetteData.Count(); dp++)
                {
                    rawDataToCluster[dp].Silhouette = silhouetteData[dp].Silhouette;
                }
            }

            rawDataToCluster = Helpers.ComputePCA(ref normalizedDataToCluster, ref rawDataToCluster);

            return rawDataToCluster;
        }

        private void InitializeCentroids()
        {
            IDictionary<string, double> sumsOfProperties = new Dictionary<string, double>();

            foreach (var property in normalizedDataToCluster.First().Properties)
            {
                var sumOfProperty = 0.0;

                for (var dataPoint = 0; dataPoint < normalizedDataToCluster.Count; dataPoint++)
                {
                    sumOfProperty += property.Value;
                }

                sumsOfProperties.Add(property.Key, sumOfProperty);
            }

            IDictionary<string, double> meansOfProperties = new Dictionary<string, double>();

            foreach (var property in rawDataToCluster.First().Properties)
            {
                var mean = sumsOfProperties[property.Key] / rawDataToCluster.Count();
                meansOfProperties.Add(property.Key, mean);
            }

            var meanDataPoint = new KMDataPoint(meansOfProperties);

            var distances = new double[normalizedDataToCluster.Count];

            for (int dp = 0; dp < normalizedDataToCluster.Count; dp++)
            {
                distances[dp] = Helpers.EuclideanDistance(normalizedDataToCluster[dp], meanDataPoint);
            }

            var closestDpIndex = Helpers.MinIndex(distances);

            List<KMDataPoint> currentCentroids = new List<KMDataPoint>();
            currentCentroids.Add(normalizedDataToCluster[closestDpIndex]);
            normalizedDataToCluster[closestDpIndex].Cluster = rawDataToCluster[closestDpIndex].Cluster = 0; // initial centroid

            
            for (int i = 1; i < numberOfClusters; i++)
            {
                var currentCentroidDistances = new double[normalizedDataToCluster.Count];

                for (int dp = 0; dp < normalizedDataToCluster.Count; dp++)
                {
                    foreach(var currentCentroid in currentCentroids)
                    {
                        var euclideanDistance = Helpers.EuclideanDistance(normalizedDataToCluster[dp], currentCentroid);
                        if (currentCentroids.Contains(normalizedDataToCluster[dp]))
                        {
                            currentCentroidDistances[dp] = euclideanDistance = 0;
                        }
                        currentCentroidDistances[dp] += euclideanDistance;
                    }
                }

                var furthestDpIndex = Helpers.MaxIndex(currentCentroidDistances);

                currentCentroids.Add(normalizedDataToCluster[furthestDpIndex]);

                normalizedDataToCluster[furthestDpIndex].Cluster = rawDataToCluster[furthestDpIndex].Cluster = i;
            }
        }

        private bool CalculateClustersMeans()
        {
            var groupToComputeMeans = normalizedDataToCluster
                .GroupBy(s => s.Cluster)
                .Where(s => s.Key != null)
                .OrderBy(s => s.Key);

            var clustersCount = groupToComputeMeans.Count();

            if (clustersCount != numberOfClusters && assignEmptyClustersRetries < 10)
            {
                var numberOfClustersToAssign = numberOfClusters - groupToComputeMeans.Count();
                AssignEmptyClusters(numberOfClustersToAssign);
                assignEmptyClustersRetries++;

                groupToComputeMeans = normalizedDataToCluster
                .GroupBy(s => s.Cluster)
                .Where(s => s.Key != null)
                .OrderBy(s => s.Key);
            };

            Parallel.ForEach(groupToComputeMeans,
                item =>
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
                        var mean = sumsOfProperties[property.Key] / item.Count();
                        meansOfProperties.Add(property.Key, mean);
                    }

                    clusters.Where(s => s.Cluster == item.Key.Value).Single().Properties = meansOfProperties;
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
                        var distance = Helpers.EuclideanDistance(normalizedDataToCluster[dataPoint], clusters[clusterIndex]);

                        // avoid a centroid data point to be assigned to its own cluster consisting only of itself
                        distances[clusterIndex] = distance != 0 ? distance : 100; 
                    }

                    var closestCluster = Helpers.MinIndex(distances);
                    if (closestCluster != normalizedDataToCluster[dataPoint].Cluster)
                    {
                        changed = true;
                        normalizedDataToCluster[dataPoint].Cluster = rawDataToCluster[dataPoint].Cluster = closestCluster;
                    }
                });

            return changed;
        }

        private void AssignEmptyClusters(int numberOfClustersToAssign)
        {
            var groupedClusters = normalizedDataToCluster
                .GroupBy(s => s.Cluster)
                .Where(s => s.Key != null)
                .OrderByDescending(s => s.Count());

            for (int i = 0; i < numberOfClustersToAssign; i++)
            {
                var clusterNumbers = groupedClusters.Select(s => (int)s.Key).ToList();

                var clusterToAssign = Enumerable.Range(0, numberOfClusters).Except(clusterNumbers).First();

                var currentBiggestCluster = groupedClusters.First();

                var sizeOfSelectedCluster = currentBiggestCluster.Count();

                var distances = new double[sizeOfSelectedCluster];

                var meanDataPoint = clusters.Where(s => s.Cluster == currentBiggestCluster.Key).Single();

                var arrayOfCurrentCluster = currentBiggestCluster.ToArray();

                for (int dp = 0; dp < sizeOfSelectedCluster; dp++)
                {
                    distances[dp] = Helpers.EuclideanDistance(arrayOfCurrentCluster[dp], meanDataPoint);
                }

                var furthestDpIndex = Helpers.MaxIndex(distances);

                var newClusterDataPoint = arrayOfCurrentCluster[furthestDpIndex];

                var newClusterDpIndex = normalizedDataToCluster.FindIndex(s => s == newClusterDataPoint);

                normalizedDataToCluster[newClusterDpIndex].Cluster = rawDataToCluster[newClusterDpIndex].Cluster = clusterToAssign;
            }
        }
    }
}