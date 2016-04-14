using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clustering.App.Api.Algorithms;
using System.Collections.Generic;

namespace Clustering.Tests.Clustering
{
    [TestClass]
    public class StandardisationTest
    {
        [TestMethod]
        public void TestStandardisation()
        {
            // Standardisation = (propertyValue - propertyMean) / propertyStandardDeviation

            var properties = new Dictionary<string, double>();

            properties.Add("prop1", 54);
            properties.Add("prop2", 32);

            var datapoint = new KMDataPoint(properties);

            var properties1 = new Dictionary<string, double>();

            properties1.Add("prop1", 38);
            properties1.Add("prop2", 24);

            var datapoint1 = new KMDataPoint(properties1);

            var list = new List<KMDataPoint>();

            list.Add(datapoint);
            list.Add(datapoint1);

            var standardisedProp = new Dictionary<string, double>();

            standardisedProp.Add("prop1", 0.125);
            standardisedProp.Add("prop2", 0.25);

            var standardisedProp2 = new Dictionary<string, double>();

            standardisedProp2.Add("prop1", -0.125);
            standardisedProp2.Add("prop2", -0.25);

            var expected = new List<KMDataPoint>
            {
                new KMDataPoint(standardisedProp),
                new KMDataPoint(standardisedProp2)
            };

            var actual = Standardisation.StandardiseData(ref list);

            Assert.AreEqual(0.125, actual[0].Properties["prop1"]);
            Assert.AreEqual(0.25, actual[0].Properties["prop2"]);
            Assert.AreEqual(-0.125, actual[1].Properties["prop1"]);
            Assert.AreEqual(-0.25, actual[1].Properties["prop2"]);
        }
    }
}
