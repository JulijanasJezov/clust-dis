using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clustering.App.Api.Algorithms;
using System.Collections.Generic;
using System;

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
        public void TestMaxIndex()
        {
            double[] testArray = { 2.3, 2.4, 1.1, 0.5, 1.2, 5.5 };

            int expected = 5;
            int actual = Helpers.MaxIndex(testArray);

            Assert.AreEqual(expected, actual, "The index of max number is correct");

            double[] testArray1 = { 2.3, 2.4, 1.1, 0.5, 1.2, 5.5, 0.0000000000000001, 50.222222, 1111111111111122.123 };

            expected = 8;
            actual = Helpers.MaxIndex(testArray1);

            Assert.AreEqual(expected, actual, "The index of max number is correct");

            double[] testArray2 = { 2.3, 2.4, 1.1, 0.5, 1.2, 5.5, -234.0234000234 };

            expected = 5;
            actual = Helpers.MaxIndex(testArray2);

            Assert.AreEqual(expected, actual, "The index of max number is correct");

            double[] testArray3 = new double[5];

            expected = 0;
            actual = Helpers.MaxIndex(testArray3);

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

        [TestMethod]
        public void TestCalculatePropertiesSum()
        {
            var properties = new Dictionary<string, double>();

            properties.Add("prop1", 3);
            properties.Add("prop2", 34);
            properties.Add("prop3", 22);
            properties.Add("prop4", 120);
            properties.Add("prop5", 11);
            properties.Add("prop6", 43);

            var properties1 = new Dictionary<string, double>();

            properties1.Add("prop1", 64);
            properties1.Add("prop2", 31);
            properties1.Add("prop3", 43);
            properties1.Add("prop4", 30);
            properties1.Add("prop5", 44);
            properties1.Add("prop6", 111);

            var datapoint = new KMDataPoint(properties);
            var datapoint1 = new KMDataPoint(properties1);

            var list = new List<KMDataPoint>();
            list.Add(datapoint);
            list.Add(datapoint1);

            var expected = new Dictionary<string, double>();
            expected.Add("prop1", 67);
            expected.Add("prop2", 65);
            expected.Add("prop3", 65);
            expected.Add("prop4", 150);
            expected.Add("prop5", 55);
            expected.Add("prop6", 154);

            var actual = Helpers.CalculatePropertiesSum(list, datapoint.Properties);

            Assert.AreEqual(expected["prop1"], actual["prop1"], "The sum of properties is correct");
            Assert.AreEqual(expected["prop2"], actual["prop2"], "The sum of properties is correct");
            Assert.AreEqual(expected["prop3"], actual["prop3"], "The sum of properties is correct");
            Assert.AreEqual(expected["prop4"], actual["prop4"], "The sum of properties is correct");
            Assert.AreEqual(expected["prop5"], actual["prop5"], "The sum of properties is correct");
            Assert.AreEqual(expected["prop6"], actual["prop6"], "The sum of properties is correct");
        }

        [TestMethod]
        public void TestCalculatePropertiesMeans()
        {
            var sums = new Dictionary<string, double>();
            sums.Add("prop1", 68);
            sums.Add("prop2", 76);
            sums.Add("prop3", 64);
            sums.Add("prop4", 150);
            sums.Add("prop5", 1020);
            sums.Add("prop6", 222);

            var actual = Helpers.CalculatePropertiesMeans(sums, 2);

            Assert.AreEqual(34, actual["prop1"], "The mean of properties is correct");
            Assert.AreEqual(38, actual["prop2"], "The mean of properties is correct");
            Assert.AreEqual(32, actual["prop3"], "The mean of properties is correct");
            Assert.AreEqual(75, actual["prop4"], "The mean of properties is correct");
            Assert.AreEqual(510, actual["prop5"], "The mean of properties is correct");
            Assert.AreEqual(111, actual["prop6"], "The mean of properties is correct");
        }

        [TestMethod]
        public void TestCalculateStandardDeviation()
        {
            var properties = new Dictionary<string, double>();

            properties.Add("prop1", 3);
            properties.Add("prop2", 34);
            properties.Add("prop3", 22);
            properties.Add("prop4", 120);
            properties.Add("prop5", 11);
            properties.Add("prop6", 43);

            var properties1 = new Dictionary<string, double>();

            properties1.Add("prop1", 64);
            properties1.Add("prop2", 31);
            properties1.Add("prop3", 43);
            properties1.Add("prop4", 30);
            properties1.Add("prop5", 44);
            properties1.Add("prop6", 111);

            var datapoint = new KMDataPoint(properties);
            var datapoint1 = new KMDataPoint(properties1);

            var list = new List<KMDataPoint>();
            list.Add(datapoint);
            list.Add(datapoint1);

            var means = new Dictionary<string, double>();
            means.Add("prop1", 34);
            means.Add("prop2", 38);
            means.Add("prop3", 32);
            means.Add("prop4", 75);
            means.Add("prop5", 1020);
            means.Add("prop6", 111);

            var actual = Helpers.CalculateStandardDeviation(list, means);

            Assert.AreEqual(31, Math.Round(actual["prop1"]), "The standard deviation of properties is correct");
            Assert.AreEqual(6, Math.Round(actual["prop2"]), "The standard deviation of properties is correct");
            Assert.AreEqual(11, Math.Round(actual["prop3"]), "The standard deviation of properties is correct");
            Assert.AreEqual(45, Math.Round(actual["prop4"]), "The standard deviation of properties is correct");
            Assert.AreEqual(993, Math.Round(actual["prop5"]), "The standard deviation of properties is correct");
            Assert.AreEqual(48, Math.Round(actual["prop6"]), "The standard deviation of properties is correct");
        }
    }
}
