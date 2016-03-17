using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Clustering.App.Api.Algorithms
{
    public static class Validation
    {
        public static List<KMDataPoint> CalculateSilhouette(List<KMDataPoint> clusteredData)
        {
            var clusters = clusteredData.GroupBy(s => s.Cluster);

            Parallel.ForEach(clusteredData,
                dataPoint =>
                {
                    var averageDistanceSameCluster = 0.0;
                    var minAvgDistanceOtherClusterIndex = 0;
                    var totalDistanceSameCluster = 0.0;
                    var otherClusterAvgDistances = new double[clusters.Count() - 1];
                    var currentCluster = clusters.Where(s => s.Key == dataPoint.Cluster).Single();

                    foreach (var sameClusterDp in currentCluster)
                    {
                        totalDistanceSameCluster += Helpers.EuclideanDistance(dataPoint, sameClusterDp);
                    }

                    var clusterIndex = 0;

                    foreach (var cluster in clusters.Where(s => s.Key != dataPoint.Cluster))
                    {
                        var otherClusterTotalDistance = 0.0;

                        foreach (var otherClusterDp in cluster)
                        {
                            otherClusterTotalDistance += Helpers.EuclideanDistance(dataPoint, otherClusterDp);
                        }

                        otherClusterAvgDistances[clusterIndex] = otherClusterTotalDistance / cluster.Count();
                        clusterIndex++;
                    }

                    averageDistanceSameCluster = totalDistanceSameCluster / currentCluster.Count();
                    minAvgDistanceOtherClusterIndex = Helpers.MinIndex(otherClusterAvgDistances);

                    dataPoint.Silhouette = (otherClusterAvgDistances[minAvgDistanceOtherClusterIndex] - averageDistanceSameCluster) / 
                        Math.Max(averageDistanceSameCluster, otherClusterAvgDistances[minAvgDistanceOtherClusterIndex]);
                });

            return clusteredData;
        }
    }
}