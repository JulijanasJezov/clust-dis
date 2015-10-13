using Clustering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api.Shared.Algorithms
{
    public class KMAlgorithm
    {
        private List<KMDataPoint> _rawDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> _normalizedDataToCluster = new List<KMDataPoint>();
        private List<KMDataPoint> _clusters = new List<KMDataPoint>();
        private int _numberOfClusters = 5;

        public List<KMDataPoint> RunKMeansAlgorithm(List<KMDataPoint> dataPoints)
        {
            _rawDataToCluster = dataPoints;

            for (int i = 0; i < _numberOfClusters; i++)
            {
                _clusters.Add(new KMDataPoint() { Cluster = i });
            }

            NormaliseData();

            bool _changed = true;
            bool _success = true;
            InitializeCentroids();

            int maxIteration = dataPoints.Count * 10;
            int _threshold = 0;
            while (_success == true && _changed == true && _threshold < maxIteration)
            {
                ++_threshold;
                
                _success = UpdateDataPointMeans();
                _changed = UpdateClusterMembership();
            }

            return _rawDataToCluster;
        }

        private void NormaliseData()
        {
            IDictionary<string, double> sumsOfProperties = new Dictionary<string, double>();

            foreach (var property in _rawDataToCluster.First().Properties)
            {
                var sumOfProperty = 0.0;

                foreach (var dataPoint in _rawDataToCluster)
                {
                    sumOfProperty += dataPoint.Properties[property.Key];
                }

                sumsOfProperties.Add(property.Key, sumOfProperty);
            }

            IDictionary<string, double> meansOfProperties = new Dictionary<string, double>();

            foreach (var property in _rawDataToCluster.First().Properties)
            {
                var mean = sumsOfProperties[property.Key] / _rawDataToCluster.Count();
                meansOfProperties.Add(property.Key, mean);
            }

            IDictionary<string, double> sumsPowOfProperties = new Dictionary<string, double>();

            foreach (var property in _rawDataToCluster.First().Properties)
            {
                var sumPowOfProperty = 0.0;

                foreach (var dataPoint in _rawDataToCluster)
                {
                    sumPowOfProperty += Math.Pow(dataPoint.Properties[property.Key] - meansOfProperties[property.Key], 2);
                }

                sumsPowOfProperties.Add(property.Key, sumPowOfProperty);
            }

            IDictionary<string, double> sdOfProperties = new Dictionary<string, double>();

            foreach (var property in _rawDataToCluster.First().Properties)
            {
                var sd = sumsPowOfProperties[property.Key] / _rawDataToCluster.Count();
                sdOfProperties.Add(property.Key, sd);
            }

            foreach (var dataPoint in _rawDataToCluster)
            {
                var properties = new Dictionary<string, double>();
                foreach (var property in dataPoint.Properties)
                {
                    properties.Add(property.Key, (dataPoint.Properties[property.Key] - meansOfProperties[property.Key]) / sdOfProperties[property.Key]);
                }

                _normalizedDataToCluster.Add(new KMDataPoint(properties));
            }
        }

        private void InitializeCentroids()
        {
            Random random = new Random(_numberOfClusters);
            for (int i = 0; i < _numberOfClusters; ++i)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = i;
            }

            for (int i = 0; i < _normalizedDataToCluster.Count; i++)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = random.Next(0, _numberOfClusters);
            }
        }

        private bool UpdateDataPointMeans()
        {
            if (IsEmptyCluster(_normalizedDataToCluster)) return false;

            var groupToComputeMeans = _normalizedDataToCluster
                .GroupBy(s => s.Cluster)
                .OrderBy(s => s.Key);

            int clusterIndex = 0;

            foreach (var item in groupToComputeMeans)
            {
                IDictionary<string, double> sumsOfProperties = new Dictionary<string, double>();

                foreach (var property in _normalizedDataToCluster.First().Properties)
                {
                    var sumOfProperty = 0.0;

                    foreach (var value in item)
                    {
                        sumOfProperty += value.Properties[property.Key];
                    }

                    sumsOfProperties.Add(property.Key, sumOfProperty);
                }

                IDictionary<string, double> meansOfProperties = new Dictionary<string, double>();

                foreach (var property in _rawDataToCluster.First().Properties)
                {
                    var mean = sumsOfProperties[property.Key] / _rawDataToCluster.Count();
                    meansOfProperties.Add(property.Key, mean);
                }

                _clusters[clusterIndex].Properties = meansOfProperties;
                clusterIndex++;
            }

            return true;
        }

        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[_numberOfClusters];
            
            for (int i = 0; i < _normalizedDataToCluster.Count; ++i)
            {
                for (int k = 0; k < _numberOfClusters; ++k)
                    distances[k] = ElucidanDistance(_normalizedDataToCluster[i], _clusters[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != _normalizedDataToCluster[i].Cluster)
                {
                    changed = true;
                    _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = newClusterId;
                }
            }

            if (changed == false)
                return false;

            if (IsEmptyCluster(_normalizedDataToCluster)) return false;

            return true;
        }

        private int MinIndex(double[] distances)
        {
            int _indexOfMin = 0;
            double _smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < _smallDist)
                {
                    _smallDist = distances[k];
                    _indexOfMin = k;
                }
            }
            return _indexOfMin;
        }

        private double ElucidanDistance(KMDataPoint dataPoint, KMDataPoint mean)
        {
            var _diffs = 0.0;

            foreach (var property in dataPoint.Properties)
            {
                _diffs += Math.Pow(dataPoint.Properties[property.Key] - mean.Properties[property.Key], 2);
            }

            return Math.Sqrt(_diffs);
        }

        private bool IsEmptyCluster(List<KMDataPoint> data)
        {
            var emptyCluster = data
                .GroupBy(s => s.Cluster)
                .OrderBy(s => s.Key)
                .Select(g => new
                {
                    Cluster = g.Key,
                    Count = g.Count()
                });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}