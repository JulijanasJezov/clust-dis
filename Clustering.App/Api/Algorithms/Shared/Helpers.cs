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

        public static double EuclideanDistance(KMDataPoint dataPoint, KMDataPoint mean)
        {
            var distance = 0.0;

            foreach (var property in dataPoint.Properties)
            {
                distance += Math.Pow(dataPoint.Properties[property.Key] - mean.Properties[property.Key], 2);
            }

            return Math.Sqrt(distance);
        }

        public static bool HasEmptyCluster(List<KMDataPoint> data)
        {
            var clusters = data
                .GroupBy(s => s.Cluster)
                .OrderBy(s => s.Key)
                .Select(g => new
                {
                    Cluster = g.Key,
                    Count = g.Count()
                });

            var isEmpty = clusters.Any(s => s.Count == 0) ? true : false;

            return isEmpty;
        }
    }
}