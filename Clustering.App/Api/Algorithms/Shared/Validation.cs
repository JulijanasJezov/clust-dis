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
                    var minAverageDistanceOtherCluster = 0;
                    var distance = 0.0;
                    var otherClusterAvgDistances = new double[clusters.Count() - 1];
                    var currentCluster = clusters.Where(s => s.Key == dataPoint.Cluster).Single();

                    foreach (var sameClusterDp in currentCluster)
                    {
                        var singlePointDistance = 0.0;

                        foreach (var property in dataPoint.Properties)
                        {
                            singlePointDistance += Math.Pow(dataPoint.Properties[property.Key] - sameClusterDp.Properties[property.Key], 2);
                        }

                        distance += Math.Sqrt(singlePointDistance);
                    }

                    var clusterIndex = 0;

                    foreach (var cluster in clusters.Where(s => s.Key != dataPoint.Cluster))
                    {
                        var otherClusterDistance = 0.0;

                        foreach (var otherClusterDp in cluster)
                        {
                            var singlePointDistance = 0.0;

                            foreach (var property in dataPoint.Properties)
                            {
                                singlePointDistance += Math.Pow(dataPoint.Properties[property.Key] - otherClusterDp.Properties[property.Key], 2);
                            }

                            otherClusterDistance += Math.Sqrt(singlePointDistance);
                        }

                        otherClusterAvgDistances[clusterIndex] = otherClusterDistance / cluster.Count();
                        clusterIndex++;
                    }

                    averageDistanceSameCluster = distance / currentCluster.Count();
                    minAverageDistanceOtherCluster = Helpers.MinIndex(otherClusterAvgDistances);

                    dataPoint.Silhouette = (otherClusterAvgDistances[minAverageDistanceOtherCluster] - averageDistanceSameCluster) / Math.Max(averageDistanceSameCluster, otherClusterAvgDistances[minAverageDistanceOtherCluster]);
                });

            return clusteredData;
        }
    }
}