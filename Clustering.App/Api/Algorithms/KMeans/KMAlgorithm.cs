﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clustering.App.Api.Algorithms
{
    public class KMAlgorithm
    {
        private List<KMDataPoint> rawDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> standardisedDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> clusters = new List<KMDataPoint>();
        private int numberOfClusters;
        private int assignEmptyClustersRetries = 0;

        /// <summary>
        /// Clusters and returns the data passed in
        /// </summary>
        public List<KMDataPoint> ClusterData(List<KMDataPoint> dataPoints, int numOfClusters, bool calculateSilhouette = false)
        {
            rawDataToCluster = dataPoints;

            numberOfClusters = numOfClusters;

            // Create the number of clusters specified
            for (int i = 0; i < numberOfClusters; i++)
            {
                clusters.Add(new KMDataPoint() { Cluster = i });
            }

            // Standartise the data's parameters for a fair comparison
            standardisedDataToCluster = Standardisation.StandardiseData(ref rawDataToCluster);

            InitialiseCentroids();

            var maxIteration = dataPoints.Count;
            var iteration = 0;

            // Calculate clusters means and reassign the data points to the nearest clusters until there's nothing left to reassign
            while (iteration < maxIteration)
            {
                var clustersUpdated = CalculateClustersMeans() && UpdateDataPointsClusters();

                if (!clustersUpdated) break;

                iteration++;
            }

            if (calculateSilhouette)
            {
                var silhouetteData = Validation.CalculateSilhouette(standardisedDataToCluster);

                for (var dp = 0; dp < silhouetteData.Count(); dp++)
                {
                    rawDataToCluster[dp].Silhouette = silhouetteData[dp].Silhouette;
                }
            }

            rawDataToCluster = Helpers.ComputePCA(ref standardisedDataToCluster, ref rawDataToCluster);

            return rawDataToCluster;
        }

        /// <summary>
        /// Initialises the centroids for the number of clusters specified
        /// </summary>
        private void InitialiseCentroids()
        {
            var farthestDistanceOfTwo = 0.0;
            var twoFarthestDatapoints = new int[2];

            // Find the two data point with the highest Euclidean distance
            for (int dp1 = 0; dp1 < standardisedDataToCluster.Count; dp1++)
            {
                for (int dp2 = dp1 + 1; dp2 < standardisedDataToCluster.Count; dp2++)
                {
                    var euclideanDistance = Helpers.EuclideanDistance(standardisedDataToCluster[dp1], standardisedDataToCluster[dp2]);

                    if (farthestDistanceOfTwo < euclideanDistance)
                    {
                        farthestDistanceOfTwo = euclideanDistance;
                        twoFarthestDatapoints[0] = dp1;
                        twoFarthestDatapoints[1] = dp2;
                    }
                }
            }

            List<KMDataPoint> currentCentroids = new List<KMDataPoint>();
            currentCentroids.Add(standardisedDataToCluster[twoFarthestDatapoints[0]]);
            currentCentroids.Add(standardisedDataToCluster[twoFarthestDatapoints[1]]);

            // First initial centroids with the highest distance between the two
            standardisedDataToCluster[twoFarthestDatapoints[0]].Cluster = rawDataToCluster[twoFarthestDatapoints[0]].Cluster = 0;
            standardisedDataToCluster[twoFarthestDatapoints[1]].Cluster = rawDataToCluster[twoFarthestDatapoints[1]].Cluster = 1;

            // Assign initial centroids for all remaining clusters
            for (int i = 2; i < numberOfClusters; i++)
            {
                var currentCentroidDistances = new double[standardisedDataToCluster.Count];

                for (int dp = 0; dp < standardisedDataToCluster.Count; dp++)
                {
                    foreach (var currentCentroid in currentCentroids)
                    {
                        var euclideanDistance = Helpers.EuclideanDistance(standardisedDataToCluster[dp], currentCentroid);

                        // Prevent same data point being assigned to multiple initial clusters
                        if (currentCentroids.Contains(standardisedDataToCluster[dp]))
                        {
                            currentCentroidDistances[dp] = euclideanDistance = 0;
                        }
                        currentCentroidDistances[dp] += euclideanDistance;
                    }
                }

                var furthestDpIndex = Helpers.MaxIndex(currentCentroidDistances);

                currentCentroids.Add(standardisedDataToCluster[furthestDpIndex]);

                standardisedDataToCluster[furthestDpIndex].Cluster = rawDataToCluster[furthestDpIndex].Cluster = i;
            }
        }

        /// <summary>
        /// Calculates and updates each clusters mean
        /// </summary>
        private bool CalculateClustersMeans()
        {
            var groupToComputeMeans = standardisedDataToCluster
                .GroupBy(s => s.Cluster)
                .Where(s => s.Key != null)
                .OrderBy(s => s.Key);

            var clustersCount = groupToComputeMeans.Count();

            // Recreate a cluster if there's empty one
            if (clustersCount != numberOfClusters && assignEmptyClustersRetries < 10)
            {
                var numberOfClustersToAssign = numberOfClusters - groupToComputeMeans.Count();
                AssignEmptyClusters(numberOfClustersToAssign);
                assignEmptyClustersRetries++;

                groupToComputeMeans = standardisedDataToCluster
                .GroupBy(s => s.Cluster)
                .Where(s => s.Key != null)
                .OrderBy(s => s.Key);
            };

            Parallel.ForEach(groupToComputeMeans,
                item =>
                {
                    var sumsOfProperties = Helpers.CalculatePropertiesSum(item.ToList(), standardisedDataToCluster.First().Properties);

                    var meansOfProperties = Helpers.CalculatePropertiesMeans(sumsOfProperties, item.Count());

                    clusters.Where(s => s.Cluster == item.Key.Value).Single().Properties = meansOfProperties;
                });

            return true;
        }

        /// <summary>
        /// Assigns each data point to the nearest cluster based on the Euclidean Distance
        /// </summary>
        private bool UpdateDataPointsClusters()
        {
            var changed = false;

            Parallel.For(0, standardisedDataToCluster.Count,
                dataPoint =>
                {
                    var distances = new double[numberOfClusters];

                    for (int clusterIndex = 0; clusterIndex < numberOfClusters; clusterIndex++)
                    {
                        var distance = Helpers.EuclideanDistance(standardisedDataToCluster[dataPoint], clusters[clusterIndex]);

                        // Avoid a centroid data point to be assigned to its own cluster consisting only of itself
                        distances[clusterIndex] = distance != 0 ? distance : 100;
                    }

                    var closestCluster = Helpers.MinIndex(distances);
                    if (closestCluster != standardisedDataToCluster[dataPoint].Cluster)
                    {
                        changed = true;
                        standardisedDataToCluster[dataPoint].Cluster = rawDataToCluster[dataPoint].Cluster = closestCluster;
                    }
                });

            return changed;
        }

        /// <summary>
        /// Creates the new clusters out of the largest cluster's members that are the furthest away from its centroid
        /// </summary>
        private void AssignEmptyClusters(int numberOfClustersToAssign)
        {
            var groupedClusters = standardisedDataToCluster
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

                var newClusterDpIndex = standardisedDataToCluster.FindIndex(s => s == newClusterDataPoint);

                standardisedDataToCluster[newClusterDpIndex].Cluster = rawDataToCluster[newClusterDpIndex].Cluster = clusterToAssign;
            }
        }
    }
}