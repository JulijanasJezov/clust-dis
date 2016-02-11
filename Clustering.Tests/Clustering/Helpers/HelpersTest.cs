using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clustering.App.Api.Algorithms;
using System.Collections.Generic;

namespace Clustering.Tests.Clustering
{
    [TestClass]
    public class HelpersTest
    {
        [TestMethod]
        public void TestMinIndex()
        {
            double[] testArray = { 2.3, 2.4, 1.1, 0.5, 1.2, 5.5 };

            int expected = 3;
            int actual = Helpers.MinIndex(testArray);

            Assert.AreEqual(expected, actual, "The index of min number is correct");

            double[] testArray1 = { 2.3, 2.4, 1.1, 0.5, 1.2, 5.5, 0.0000000000000001, 50.222222, 1111111111111122.123 };

            expected = 6;
            actual = Helpers.MinIndex(testArray1);

            Assert.AreEqual(expected, actual, "The index of min number is correct");

            double[] testArray2 = { 2.3, 2.4, 1.1, 0.5, 1.2, 5.5, -234.0234000234 };

            expected = 6;
            actual = Helpers.MinIndex(testArray2);

            Assert.AreEqual(expected, actual, "The index of min number is correct");

            double[] testArray3 = new double[5];

            expected = 0;
            actual = Helpers.MinIndex(testArray3);

            Assert.AreEqual(expected, actual, "Return 0 index when array is empty");
        }

        [TestMethod]
        public void TestEuclideanDistance()
        {
            // Euclidean Distance Formula = sqrRoot(Pow(Abs[a-x]) + Pow(Abs[b-y]) + Pow(Abs[c-z]) ....)

            var properties = new Dictionary<string, double>();

            properties.Add("prop1", 1);

            var datapoint = new KMDataPoint(properties);

            var clusterMeans = new Dictionary<string, double>();

            clusterMeans.Add("prop1", 51);

            var cluster = new KMDataPoint(clusterMeans);

            double expected = 50;
            double actual = Helpers.EuclideanDistance(datapoint, cluster);

            Assert.AreEqual(expected, actual, "One dimention euclidean distance is correct");

            var properties1 = new Dictionary<string, double>();

            properties1.Add("prop1", 3);
            properties1.Add("prop2", 34);
            properties1.Add("prop3", 22);
            properties1.Add("prop4", 120);
            properties1.Add("prop5", 11);
            properties1.Add("prop6", 43);

            var datapoint1 = new KMDataPoint(properties1);

            var clusterMeans1 = new Dictionary<string, double>();

            clusterMeans1.Add("prop1", 8);
            clusterMeans1.Add("prop2", 26);
            clusterMeans1.Add("prop3", 16);
            clusterMeans1.Add("prop4", 80);
            clusterMeans1.Add("prop5", 5);
            clusterMeans1.Add("prop6", 70);

            var cluster1 = new KMDataPoint(clusterMeans1);

            expected = 49.8998997994986;
            actual = Helpers.EuclideanDistance(datapoint1, cluster1);

            Assert.AreEqual(expected, actual, "Multi dimention euclidean distance is correct confirmed by manual calculation");
        }
    }
}
